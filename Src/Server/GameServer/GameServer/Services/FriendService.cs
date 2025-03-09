using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class FriendService : Singleton<FriendService>
    {
        public void Init()
        { }
        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddReq);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddRes);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }

        private void OnFriendAddReq(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            Character fromCha = sender.Session.Character;
            Log.InfoFormat("FriendService接收好友添加请求：From:{0} To:{1}", request.FromName, request.ToName);

            if (request.ToId == 0)
            {
                foreach (var cha in CharacterManager.Instance.Characters)
                {
                    if (cha.Value.Info.Name == request.ToName)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }

            NetConnection<NetSession> friendsession = null;
            if (request.ToId > 0)
            {
                if (fromCha.friendManager.GetFriendInfo(request.ToId) != null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "好友已存在";
                    sender.SendResponse();
                    return;
                }
                friendsession = SessionManager.Instance.GetSession(request.ToId);
            }
            if (friendsession == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "玩家不存在或已离线";
                sender.SendResponse();
                Log.InfoFormat("FriendService发送好友添加响应：好友不存在或已离线");
                return;
            }

            Log.InfoFormat("FriendService发送好友添加申请：From：{0} To：{1}", request.FromName, request.ToName);
            friendsession.Session.Response.friendAddReq = request;
            friendsession.SendResponse();
        }

        private void OnFriendAddRes(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character fromCha = sender.Session.Character;
            Log.InfoFormat("FriendService接收好友添加响应:From:{0}", fromCha.Info.Name);

            //离线 填充回应者消息
            //在线 按成功/失败情况填充消息
            NetConnection<NetSession> requestSession = SessionManager.Instance.GetSession(response.Request.FromId);
            if (requestSession == null)
            {
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "请求玩家已下线";
                sender.SendResponse();
            }
            else 
            {
                if (response.Result == Result.Success)
                {//添加成功
                    fromCha.friendManager.AddFriend(requestSession.Session.Character);
                    requestSession.Session.Character.friendManager.AddFriend(fromCha);
                    DBService.Instance.Save();
                    response.Result = Result.Success;
                    response.Errormsg = "成功添加好友";
                    requestSession.Session.Response.friendAddRes = response;
                    sender.SendResponse();
                    requestSession.SendResponse();
                }
                else
                {//添加失败
                    requestSession.Session.Response.friendAddRes = new FriendAddResponse();
                    requestSession.Session.Response.friendAddRes.Result = Result.Failed;
                    requestSession.Session.Response.friendAddRes.Errormsg = "对方拒绝了你的好友请求";
                    requestSession.SendResponse();
                }
            }
        }

        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
