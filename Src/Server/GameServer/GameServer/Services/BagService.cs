using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class BagService : Singleton<BagService>
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);
        }

        public void Init()
        { }

        void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest request)
        {
            Character character = sender.Session.Character;

            if(request.BagInfo != null)
            {
                character.Tcharacter.Bag.Items = request.BagInfo.Items;
                //DBService.Instance
            }
        }
    }
}
