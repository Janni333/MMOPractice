using Assets.Scripts.Model;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class StatusService : Singleton<StatusService>
    {
        public delegate void statusHandler(NStatusInfo statusinfo);
        public Dictionary<StatusType, statusHandler> statusHandlerMap = new Dictionary<StatusType, statusHandler>();

        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }

        public void Init()
        { }

        public void RegisterHandler(StatusType type, statusHandler handler)
        {
            if (this.statusHandlerMap.ContainsKey(type))
            {
                this.statusHandlerMap[type] += handler;
            }
            else
            {
                this.statusHandlerMap.Add(type, handler);
            }
        }

        private void OnStatusNotify(object sender, StatusNotify message)
        {
            foreach (NStatusInfo notify in message.statusInfos)
            {
                this.notifyHandle(notify);
            }
        }

        private void notifyHandle(NStatusInfo notify)
        {
            //Log
            Debug.LogFormat("StatusService 接收并处理StatusNotify: Type:{0} Action:{1} Id:{2} Value:{3}", notify.Type, notify.Action, notify.Id, notify.Value);

            if (notify.Type == StatusType.Gold)
            {
                if (notify.Action == StatusAction.Add)
                {
                    User.Instance.AddGold(notify.Value);
                }
                if (notify.Action == StatusAction.Delete)
                {
                    User.Instance.DeleteGold(notify.Value);
                }
            }

            statusHandler handler = null;
            if (this.statusHandlerMap.TryGetValue(notify.Type, out handler))
            {
                handler(notify);
            }
        }
    }
}
