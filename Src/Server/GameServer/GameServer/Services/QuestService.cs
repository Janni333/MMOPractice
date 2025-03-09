using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class QuestService : Singleton<QuestService>
    {
        public QuestService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccept);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);
        }

        public void Init()
        { }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest request)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("QuestService接收角色任务接收请求:Character:{0}{1} Quest:{2}", cha.EntityId, cha.Info.Name, request.QuestId);

            sender.Session.Response.questAccept = new QuestAcceptResponse();
            sender.Session.Response.questAccept.Result = cha.questManager.AcceptQuest(sender, request.QuestId);

            sender.SendResponse();
        }
        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
            Character cha = sender.Session.Character;
            Log.InfoFormat("QuestService接收角色任务提交请求:Character:{0}{1} Quest:{2}", cha.EntityId, cha.Info.Name, request.QuestId);

            sender.Session.Response.questSubmit = new QuestSubmitResponse();
            sender.Session.Response.questSubmit.Result = cha.questManager.SubmitQuest(sender, request.QuestId);

            sender.SendResponse();

        }
    }
}
