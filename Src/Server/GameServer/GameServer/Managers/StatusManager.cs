using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class StatusManager
    {
        public Character Owner;
        public List<NStatusInfo> statusInfo;
        public bool hasStatus
        {
            get { return this.statusInfo.Count > 0; }
        }

        public StatusManager(Character cha)
        {
            this.Owner = cha;
            this.statusInfo = new List<NStatusInfo>();
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.statusNotify == null)
            {
                message.statusNotify = new StatusNotify();
            }
            foreach (var status in this.statusInfo)
            {
                message.statusNotify.statusInfos.Add(status);
            }
            this.statusInfo.Clear();
        }

        #region Set Status
        //
        public void AddnewStatus(int id, int value, StatusType type, StatusAction action)
        {
            NStatusInfo newStatus = new NStatusInfo()
            {
                Id = id,
                Value = value,
                Type = type,
                Action = action
            };
            this.statusInfo.Add(newStatus);
        }
        //Item
        public void AddItemStatus(int itemid, int count, StatusAction action)
        {
            this.AddnewStatus(itemid, count, StatusType.Item, action);
        } 

        //Gold
        public void AddGoldStatus(int goldchange)
        {
            if (goldchange > 0)
            {
                this.AddnewStatus(0, goldchange, StatusType.Gold, StatusAction.Add);
            }
            else 
            {
                this.AddnewStatus(0, -goldchange, StatusType.Gold, StatusAction.Add);
            }
        }

        internal void AddExpChange(int expchange)
        {
            this.AddnewStatus(0, expchange, StatusType.Exp, StatusAction.Add);
        }

        internal void AddLevelUp(int levelchange)
        {
            this.AddnewStatus(0, levelchange, StatusType.Level, StatusAction.Add);
        }
        #endregion
    }
}
