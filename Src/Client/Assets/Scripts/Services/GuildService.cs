using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Managers;

namespace Services
{
    internal class GuildService : Singleton<GuildService>, IDisposable
    {

        // UI监听事件
        internal UnityAction<bool> OnGuildCreateResult;
        internal UnityAction<List<NGuildInfo>> OnGuildListResult;
        internal UnityAction OnGuildUpdate;

        internal void Init()
        {
        }

        public GuildService() 
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        #region On
        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if(message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "Guild");
            }
            else
                MessageBox.Show("离开公会失败", "Guild", MessageBoxType.Error);
        }

        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("GuildService收到公会响应: {0} {1}:{2}", message.Result, message.Guild.Id, message.Guild.GuildName);
            GuildManager.Instance.Init(message.Guild);
            if (this.OnGuildUpdate != null)
                this.OnGuildUpdate();
        }

        private void OnGuildJoinResponse(object sender, GuildJoinResponse message)
        {
            Debug.LogFormat("GuildService收到加入公会响应: {0}", message.Result);
            if (message.Result == Result.Success)
                MessageBox.Show("加入公会成功", "Guild");
            else
                MessageBox.Show("加入公会失败", "Guild");
        }

        private void OnGuildJoinRequest(object sender, GuildJoinRequest message)
        {
            var confirm = MessageBox.Show(string.Format("{0} 申请加入公会", message.Apply.Name), "公会申请", MessageBoxType.Confirm, "Accept", "Deny");
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true, message);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinResponse(false, message);
            };
        }

        private void OnGuildList(object sender, GuildListResponse message)
        {
            if (OnGuildListResult != null)
                this.OnGuildListResult(message.Guilds);
        }

        private void OnGuildCreate(object sender, GuildCreateResponse message)
        {
            Debug.LogFormat("GuildService收到创建公会响应: {0}", message.Result);
            if (OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(message.Result == Result.Success);
            }
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(message.guildInfo);
                MessageBox.Show(string.Format("{0}公会创建成功", message.guildInfo.GuildName), "公会");
            }
            else
                MessageBox.Show(string.Format("{0}公会创建失败", message.guildInfo.GuildName), "公会");

        }

        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            Debug.LogFormat("GuildService收到管理公会响应: {0} {1}", message.Command, message.Result);
            MessageBox.Show(string.Format("执行操作：{0} 结果：{1} {2}", message.Command, message.Result, message.Errormsg));
        }
        #endregion

        #region Send
        internal void SendGuildCreate(string guildname, string notice)
        {
            Debug.LogFormat("GuildService发送创建公会请求: name:{0}", guildname);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildname;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        internal void SendGuildJoinRequest(int id)
        {
            Debug.LogFormat("GuildService发送加入公会请求: id:{0}", id);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = id;
            NetClient.Instance.SendMessage(message);
        }

        public void SendGuildJoinResponse(bool accept, GuildJoinRequest request)
        {
            Debug.LogFormat("GuildService发送加入公会回应");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = request.Apply;
            message.Request.guildJoinRes.Apply.Result = accept? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message); 
        }

        internal void SendGuildListResquest()
        {
            Debug.LogFormat("GuildService发送公会列表请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

        public void SendGuildLeaveRequest()
        {
            Debug.LogFormat("GuildService发送离开公会请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        public void SendGuildJoinApply(bool accept, NGuildApplyInfo apply)
        {
            Debug.LogFormat("GuildService发送加入公会响应");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = apply;
            message.Request.guildJoinRes.Apply.Result = accept? ApplyResult.Accept:ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        public void SendAdminCommand(GuildAdminCommand command, int id)
        {
            Debug.LogFormat("GuildService发送管理公会请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.Target = id;
            NetClient.Instance.SendMessage(message);
        }

        #endregion
    }
}
