
using Assets.Scripts.Model;
using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    class FriendService : Singleton<FriendService>, IDisposable
    {
        public UnityAction OnFriendUpdate;

        public void Init()
        { }

        public FriendService()
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddReq);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddRes);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendListRes);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemoveRes);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddReq);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddRes);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendListRes);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemoveRes);
        }

        internal void SendFriendAddRequest(int friendid, string friendname)
        {
            Debug.LogFormat("FriendService发送添加好友请求:FriendId:{0} FriendName:{1}", friendid, friendname);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.currentCharacterInfo.Id;
            message.Request.friendAddReq.FromName = User.Instance.currentCharacterInfo.Name;
            message.Request.friendAddReq.ToId = friendid;
            message.Request.friendAddReq.ToName = friendname;

            NetClient.Instance.SendMessage(message);
        }

        internal void SendFriendAddResponse(bool accept, FriendAddRequest request)
        {
            Debug.LogFormat("FriendService发送添加好友响应:FriendId:{0} FriendName:{1} reslut:{2}", request.FromId, request.FromName, accept);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRes = new FriendAddResponse();
            message.Request.friendAddRes.Request = request;
            message.Request.friendAddRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.friendAddRes.Errormsg = accept ? "对方同意了你的好友申请" : "对方拒绝了你的好友申请";

            NetClient.Instance.SendMessage(message);
        }

        private void OnFriendAddReq(object sender, FriendAddRequest request)
        {
            Debug.LogFormat("FriendService接收添加好友请求:Friend : {0}", request.FromName);

            var confirm = MessageBox.Show(string.Format("{0}请求添加你位好友", request.FromName), "好友请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendFriendAddResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendFriendAddResponse(false, request);
            };
        }

        private void OnFriendAddRes(object sender, FriendAddResponse response)
        {
            Debug.LogFormat("FriendService接收添加好友响应:Result:{0}", response.Result);

            if (response.Result == Result.Success)
            {
                MessageBox.Show(string.Format("{0}接受了你的好友请求", response.Request.ToName), "添加好友成功");
            }
            else
            {
                MessageBox.Show(response.Errormsg, "添加好友失败");
            }
        }

        internal void SendFriendRemoveRequest(int ID, int friendID)
        {
            Debug.LogFormat("FriendService发送移除好友请求：Friend：{0}", friendID);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRem = new FriendRemoveRequest();
            message.Request.friendRem.friendId = friendID;
            message.Request.friendRem.Id = ID;

            NetClient.Instance.SendMessage(message);
        }

        private void OnFriendRemoveRes(object sender, FriendRemoveResponse response)
        {
            Debug.LogFormat("FriendService接收移除好友响应:Friend:{0} ", response.Id);

            if (response.Result == Result.Success)
            {
                MessageBox.Show("删除好友成功", "删除好友");
            }
            else
            {
                MessageBox.Show("删除好友失败", "删除好友", MessageBoxType.Error);
            }
        }

        private void OnFriendListRes(object sender, FriendListResponse response)
        {
            Debug.Log("FriendService接收好友列表响应");

            FriendManager.Instance.Friends = response.Friends;
            if (this.OnFriendUpdate != null)
            {
                this.OnFriendUpdate();
            }
        }
    }
}
