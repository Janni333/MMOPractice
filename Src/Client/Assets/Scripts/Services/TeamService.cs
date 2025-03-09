using Assets.Scripts.Model;
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
    class TeamService : Singleton<TeamService>, IDisposable
    {
        public void Init()
        { }

        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        #region Send
        public void SendTeamInviteRequest(int friendid, string friendname)
        {
            Debug.LogFormat("TeamService发送邀请组队请求:From:{0}{1} To:{2}{3}", friendid, friendname, User.Instance.currentCharacterInfo.Id, User.Instance.currentCharacterInfo.Name);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.currentCharacterInfo.Id;
            message.Request.teamInviteReq.FromName = User.Instance.currentCharacterInfo.Name;
            message.Request.teamInviteReq.ToId = friendid;
            message.Request.teamInviteReq.ToName = friendname;

            NetClient.Instance.SendMessage(message);
        }

        public void SendTeamInviteResponse(bool accept, TeamInviteRequest req)
        {
            Debug.LogFormat("TeamService发送邀请组队响应：From:{0}{1} To:{2}{3} Result:{4}", req.FromId, req.FromName, req.ToId, req.ToName, accept);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteRes = new TeamInviteResponse();
            message.Request.teamInviteRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.teamInviteRes.Errormsg = accept ? "对方接受了你的组队邀请" : "对方拒绝了你的组队邀请";
            message.Request.teamInviteRes.Request = req;

            NetClient.Instance.SendMessage(message);
        }

        public void SendTeamLeave()
        { }
        #endregion

        #region On
        private void OnTeamInviteRequest(object sender, TeamInviteRequest message)
        {
            Debug.Log("TeamService接收组队邀请请求");
            var confirm = MessageBox.Show(string.Format("玩家【{0} {1}】邀请你组队，是否同意", message.FromId, message.FromName), "Confirm", MessageBoxType.Confirm, "OK", "Cancle");
            confirm.OnYes = () =>
            {
                SendTeamInviteResponse(true, message);
            };
            confirm.OnNo = () =>
            {
                SendTeamInviteResponse(false, message);
            };
        }
        private void OnTeamInviteResponse(object sender, TeamInviteResponse message)
        {
            Debug.Log("TeamService接收组队邀请回复");
            if (message.Result == Result.Success)
            {
                MessageBox.Show(string.Format("玩家【{0} {1}】同意了你的组队请求", message.Request.ToId, message.Request.ToName));
            }
            else
            {
                MessageBox.Show(string.Format("玩家【{0} {1}】拒绝了你的组队请求", message.Request.ToId, message.Request.ToName));
            }
        }

        private void OnTeamInfo(object sender, TeamInfoResponse message)
        {
            Debug.Log("TeamService接收TeamInfo响应");
            TeamManager.Instance.UpdateTeamInfo(message.Teaminfo);
        }

        private void OnTeamLeave(object sender, TeamLeaveResponse message)
        {
        }
        #endregion
    }
}
