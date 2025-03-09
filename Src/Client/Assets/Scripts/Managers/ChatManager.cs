using Assets.Scripts.Model;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public enum LocalChannel
        {
            All = 0,
            Local = 1,
            World = 2,
            Team = 3,
            Guild = 4,
            Private = 5,
        }

        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            ChatChannel.Local | ChatChannel.World | ChatChannel.Guild | ChatChannel.Team | ChatChannel.Private | ChatChannel.System,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private,
        };

        public LocalChannel sendChannel;
        internal LocalChannel displayChannel;
        public ChatChannel SendChannel
        {
            get 
            {
                switch (sendChannel)
                {
                    case LocalChannel.Local: return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                    case LocalChannel.Private: return ChatChannel.Private;
                }
                return ChatChannel.Local;
            }
        }


        public int PrivateID = 0;
        public string PrivateName = "";
        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
        };

        public Action OnChat { get; internal set; }


        public void Init()
        {
            foreach (var message in this.Messages)
            {
                message.Clear();
            }
        }

        internal void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateID = targetId;
            this.PrivateName = targetName;

            this.sendChannel = LocalChannel.Private;
            if (this.OnChat != null)
                this.OnChat();
        }

        internal bool SetSendChannel(LocalChannel channel)
        {
            if (channel == LocalChannel.Team)
            {
                if (User.Instance.currentTeam == null)
                {
                    this.AddSystemMessage("你当前不在队伍中");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)
            {
                if (User.Instance.currentCharacterInfo.Guild == null)
                {
                    this.AddSystemMessage("你当前不在公会中");
                    return false;
                }
            }

            this.sendChannel = channel;
            Debug.LogFormat("【ChatManager】Set Channel:{0}", this.sendChannel);
            return true;
        }

        internal void SendChat(string content, int toId = 0, string toName = "")
        {
            ChatService.Instance.SendChat(this.SendChannel, content, toId, toName);
        }


        #region 收
        // 添加从服务端收到的消息
        public void AddMessages(ChatChannel channel, List<ChatMessage> messages)
        {
            for (int ch = 0; ch < 6; ch++)
            {
                if ((this.ChannelFilter[ch] & channel) == channel)
                {
                    this.Messages[ch].AddRange(messages);
                }
            }
            if (this.OnChat != null)
                this.OnChat();
        }

        public void AddSystemMessage(string message, string from = "")
        {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage()
            {
                Channel = ChatChannel.System,
                Message = message,
                FormName = from
            });

            if (this.OnChat != null)
            {
                this.OnChat();
            }
        }

        public string GetCurrentMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var message in this.Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(message));
            }
            return sb.ToString();
        }

        private string FormatMessage(ChatMessage message)
        {
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("[Local]{0}{1}", FormFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[World]{0}{1}</color>", FormFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("<color=yellow>[System]{0}</color>", message.Message);
                case ChatChannel.Private:
                    return string.Format("<color=magenta>[Private]{0}{1}</color>", FormFromPlayer(message), message.Message);
                case ChatChannel.Team:
                    return string.Format("<color=green>[Team]{0}{1}</color>", FormFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=blue>[Guild]{0}{1}</color>", FormFromPlayer(message), message.Message);
            }
            return "";
        }

        private object FormFromPlayer(ChatMessage message)
        {
            if (message.FromId == User.Instance.currentCharacterInfo.Id)
                return "<a name=\"\"class=\"player\">[you]</a>";
            else
                return string.Format("<a name=\"c:{0}:{1}\"class=\"player\">[{1}]</a>", message.FromId, message.FormName);
        }
        #endregion
    }
}
