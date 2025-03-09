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
    class ChatService : Singleton<ChatService>, IDisposable
    {
        internal void Init()
        {
        }

        public ChatService() 
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChat);
        }

        private void OnChat(object sender, ChatResponse message)
        {
            Debug.Log("【ChatService】收到chatresponse");

            if (message.Result == Result.Success)
            {
                ChatManager.Instance.AddMessages(ChatChannel.Local, message.localMessages);
                ChatManager.Instance.AddMessages(ChatChannel.World, message.worldMessages);
                ChatManager.Instance.AddMessages(ChatChannel.System, message.systemMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Private, message.privateMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Team, message.teamMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Guild, message.guildMessages);
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }
        }

        internal void SendChat(ChatChannel sendChannel, string content, int toId, string toName)
        {
            Debug.Log("【ChatService】发送chatrequest");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage()
            {
                Channel = sendChannel,
                Message = content,
                ToId = toId,
                ToName = toName
            };

            NetClient.Instance.SendMessage(message);

        }

        
    }
}
