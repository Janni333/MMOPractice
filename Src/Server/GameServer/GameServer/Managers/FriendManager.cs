using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class FriendManager
    {
        public Character Owner;

        public List<NFriendInfo> friends = new List<NFriendInfo>();

        public bool friendChanged; 
        public FriendManager(Character owner)
        {
            this.Owner = owner;
            this.InitFriends();
        }


        #region Manage Functions
        //服务端初始化：数据库——>Character
        public void InitFriends()
        {
            this.friends.Clear();
            foreach (var friend in this.Owner.Tcharacter.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        //网络初始化：Character——>NCharacterInfo.Friends
        public void GetFriendInfos(List<NFriendInfo> friendlist)
        {
            foreach (var cha in friends)
            {
                friendlist.Add(cha);
            }
        }

        //添加好友
        public void AddFriend(Character character)
        {
            TCharacterFriend newfriend = new TCharacterFriend()
            {
                FriendID = character.ID,
                FriendName = character.Info.Name,
                Class = (int)character.Info.Class,
                Level = character.Info.Level,
            };
            this.Owner.Tcharacter.Friends.Add(newfriend);
            friendChanged = true;
        }


        //移除好友
        public void RemoveFriend(int ID)
        { 
        }


        //更新好友列表中某角色的（状态）信息
        private void UpdateFriendInfo(NCharacterInfo ncharacterinfo, int status)
        {
            foreach (var friend in this.friends)
            {
                if (friend.Friendinfo.Id == ncharacterinfo.Id)
                {
                    friend.Satus = status;
                    break;
                }
            }
            this.friendChanged = true;
        }

        //离线通知
        public void OfflineNotify()
        {
            foreach (var friendinfo in this.friends)
            {
                var friendcha = CharacterManager.Instance.GetCharacter(friendinfo.Friendinfo.Id);
                if (friendcha != null)
                {
                    friendcha.friendManager.UpdateFriendInfo(this.Owner.Info, 0);
                }
            }
        }
        #endregion

        #region Tool Functions

        //从数据库获取信息
        internal NFriendInfo GetFriendInfo(TCharacterFriend Tfriend)
        {
            NFriendInfo friendinfo = new NFriendInfo();
            friendinfo.Friendinfo = new NCharacterInfo();
            friendinfo.Id = Tfriend.Id;


            Character friendCha = CharacterManager.Instance.GetCharacter(Tfriend.FriendID);
            if (friendCha == null)
            {//离线则用数据库数据填充
                friendinfo.Friendinfo.Id = Tfriend.FriendID;
                friendinfo.Friendinfo.Name = Tfriend.FriendName;
                friendinfo.Friendinfo.Level = Tfriend.Level;
                friendinfo.Friendinfo.Class = (CharacterClass)Tfriend.Class;
                friendinfo.Satus = 0;
            }
            else
            {//在线则用角色数据填充
                friendinfo.Friendinfo = friendCha.GetBasicInfo();
                friendinfo.Satus = 1;

                //更新数据库数据
                if (Tfriend.Level != friendinfo.Friendinfo.Level)
                {
                    Tfriend.Level = friendinfo.Friendinfo.Level;
                }

                //反向更新对方好友数据
                friendCha.friendManager.UpdateFriendInfo(this.Owner.Info, 1);
            }

            return friendinfo;
        }

        //
        public NFriendInfo GetFriendInfo(int id)
        {
            foreach (var friend in this.friends)
            {
                if(friend.Friendinfo.Id == id)
                    return friend;
            }
            return null;
        }
        #endregion

        internal void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                this.InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                friendChanged = false;
            }
        }
    }
}
