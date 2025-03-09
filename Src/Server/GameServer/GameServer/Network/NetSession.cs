using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        public IPostResponser PostResponser { get; set; }

        private NetMessage response;
        public NetMessageResponse Response
        {
            get 
            {
                if (this.response == null)
                {
                    this.response = new NetMessage();
                }
                if (this.response.Response == null)
                {
                    this.response.Response = new NetMessageResponse();
                }
                return this.response.Response;
            }
        }
        
        internal void Disconnect()
        {
            this.PostResponser = null;
            if (Character != null)
            {
                UserService.Instance.CharacterLeaveGame(Character);
            }
        }

        public byte[] GetResponse()
        {
            //后处理
            if (response != null)
            {
                if (PostResponser != null)
                {
                    this.PostResponser.PostProcess(Response);
                }
                byte[] data = PackageHandler.PackMessage(this.response);
                this.response = null;
                return data;
            }
            return null;
        }
    }
}
