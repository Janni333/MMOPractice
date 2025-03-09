using Common.Data;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Assets.Scripts.Model
{
    class User: Singleton<User>
    {
        private NUserInfo userinfo;

        public NUserInfo UserInfo
        {
            get 
            { return userinfo; }
        }
        public void SetInfo(NUserInfo userInfo)
        {
            this.userinfo = userInfo;
        }

        public Character currentCharacter { get; set; }

        public NCharacterInfo currentCharacterInfo { get; set; }
        public NTeamInfo currentTeam { get; set; }
        public MapDefine currentMapdefine { get; set; }

        public PlayerInputController currentCharacterObj { get; set; }

        public int currentRide = 0;
        internal void Ride(int id)
        {
            if (currentRide != id)
            {
                currentRide = id;
                currentCharacterObj.SendEntityEvent(EntityEvent.Ride, currentRide);
            }
            else
            {
                currentRide = 0;
                currentCharacterObj.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }



        internal void AddGold(int value)
        {
            this.currentCharacterInfo.Gold -= value;
        }

        internal void DeleteGold(int value)
        {
            this.currentCharacterInfo.Gold -= value;
        }
    }
}
