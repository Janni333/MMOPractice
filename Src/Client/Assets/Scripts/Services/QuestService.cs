using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class QuestService : Singleton<QuestService>, IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }
        public void Init()
        { }


        public void SendQuestAccept(int questid)
        {
            Debug.LogFormat("QuestService发送任务接收请求：quest:{0}", questid);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = questid;

            NetClient.Instance.SendMessage(message);
        }
        private void OnQuestAccept(object sender, QuestAcceptResponse response)
        {
            Debug.LogFormat("QuestService接收任务接收响应：Result:{0} Err:{1}", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccept(response.Questinfo);
            }
            else
            {
                MessageBox.Show("任务接取失败", "Error", MessageBoxType.Error);
            }
        }

        public void SendQuestSubmit(int questid)
        {
            Debug.LogFormat("QuestService发送任务提交请求：quest:{0}", questid);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = questid;

            NetClient.Instance.SendMessage(message);

        }
        private void OnQuestSubmit(object sender, QuestSubmitResponse response)
        {
            Debug.LogFormat("QuestService接收任务提交响应：Result:{0} Err:{1}", response.Result, response.Errormsg);
            if (response.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmit(response.Questinfo);
            }
            else
            {
                MessageBox.Show("任务完成失败", "错误", MessageBoxType.Error);
            }
        }

    }
}
