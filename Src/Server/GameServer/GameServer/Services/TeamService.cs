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
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamInviteLeave);
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }

        /// <summary>
        /// 收到申请组队请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest message)
        {
            //Log
            Log.InfoFormat("[S]TeamService接收TeamInviteRequest : From:{0} {1} To:{2} {3}", message.FromId, message.FromName, message.ToId, message.ToName);

            NetConnection<NetSession> target = SessionManager.Instance.GetSession(message.ToId);
            if (target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方不在线";
                sender.SendResponse();
                return;
            }
            if (target.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方已在其他队伍中";
                sender.SendResponse();
                return;
            }
            Log.InfoFormat("[S]TeamService转发TeamInvitequest");
            target.Session.Response.teamInviteReq = message;
            target.SendResponse();
        }
        /// <summary>
        /// 收到回复组队请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse message)
        {
            Log.InfoFormat("[S]TeamService接收回复组队响应：From{0} {1}", message.Request.ToId, message.Request.ToName);
            sender.Session.Response.teamInviteRes = message;
            NetConnection<NetSession> requester = SessionManager.Instance.GetSession(message.Request.FromId);
            if (requester == null)
            {
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "请求者已经下线";
            }
            else
            {
                TeamManager.Instance.AddTeamMember(requester.Session.Character, sender.Session.Character);
                requester.Session.Response.teamInviteRes = message;
                requester.SendResponse();
            }
            sender.SendResponse();
        }

        private void OnTeamInviteLeave(NetConnection<NetSession> sender, TeamLeaveRequest message)
        {

        }
    }
}
