using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class FriendManager : Singleton<FriendManager>
    {
        public List<NFriendInfo> Friends;

        public void Init(List<NFriendInfo> friendinfos)
        {
            this.Friends = friendinfos;
        }
    }
}
