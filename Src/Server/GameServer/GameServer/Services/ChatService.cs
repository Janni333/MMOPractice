using Common;
using GameServer.Entities;
using GameServer.Managers;
using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class ChatService : Singleton<ChatService>
    {
        public ChatService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ChatRequest>(this.OnChat);
        }

        public void Init()
        {
            ChatManager.Instance.Init();
        }

        private void OnChat(NetConnection<NetSession> sender, ChatRequest request)
        {
            Character character = sender.Session.Character;

            Log.InfoFormat("【ChatService】收到ChatRequest: char:{0} Channel:{1} Message:{2}", character.ID, request.Message.Channel, request.Message.Message);

            if (request.Message.Channel == ChatChannel.Private)
            {// 处理私聊
                var chatTo = SessionManager.Instance.GetSession(request.Message.ToId);
                if (chatTo == null)
                {// 私聊对象不在线
                    sender.Session.Response.Chat = new ChatResponse();
                    sender.Session.Response.Chat.Result = Result.Failed;
                    sender.Session.Response.Chat.Errormsg = "对方不在线";
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResponse();
                }
                else
                {// 私聊对象在线
                    // 向对方发送消息
                    if (chatTo.Session.Response.Chat == null)
                    {
                        chatTo.Session.Response.Chat = new ChatResponse();
                    }
                    // 填充信息
                    request.Message.FromId = character.ID;
                    request.Message.FormName = character.Name;
                    chatTo.Session.Response.Chat.Result = Result.Success;
                    chatTo.Session.Response.Chat.privateMessages.Add(request.Message);
                    chatTo.SendResponse();

                    // 向自己回复
                    if (sender.Session.Response.Chat == null)
                    {
                        sender.Session.Response.Chat = new ChatResponse();
                    }
                    sender.Session.Response.Chat.Result = Result.Success;
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResponse();
                }
            }
            else
            {// 处理非私聊
                sender.Session.Response.Chat = new ChatResponse();
                sender.Session.Response.Chat.Result = Result.Success;
                ChatManager.Instance.AddMessage(character, request.Message);
                sender.SendResponse();
            }
        }
    }
}
