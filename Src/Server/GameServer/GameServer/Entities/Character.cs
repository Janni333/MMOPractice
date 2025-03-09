using Common;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Character : Creature , IPostResponser
    {
        public TCharacter Tcharacter;
        public ItemManager itemManager;
        public StatusManager statusManager;
        public QuestManager questManager;
        public FriendManager friendManager;

        public Team Team;
        public double TeamUpdateTS;

        public Guild Guild;
        public double GuildUpdateTS;

        public Chat chat;
        
        public Character(CharacterType type, TCharacter Tdata) : 
            base(type, Tdata.TID, Tdata.Level, new Vector3Int(Tdata.MapPosX, Tdata.MapPosY, Tdata.MapPosZ), new Vector3Int(100,0,0))
        {
            this.Tcharacter = Tdata;
            this.ID = Tdata.ID;

            this.Info.Class = (CharacterClass)Tdata.Class;
            this.Info.Exp = Tdata.Exp;
            this.Info.Id = Tdata.ID;
            this.Info.mapId = Tdata.MapID;
            this.Info.Gold = Tdata.Gold;
            this.Info.Ride = 0;
            this.Info.Name = Tdata.Name;

            this.itemManager = new ItemManager(this);
            itemManager.GetItemInfos(this.Info.Items);

            this.statusManager = new StatusManager(this);

            this.Info.Bag = new NBagInfo();
            this.Info.Bag.Unlocked = this.Tcharacter.Bag.Unlocked;
            this.Info.Bag.Items = this.Tcharacter.Bag.Items;

            this.questManager = new QuestManager(this);
            this.questManager.GetQuestInfos(this.Info.Quests);

            this.friendManager = new FriendManager(this);
            this.friendManager.GetFriendInfos(this.Info.Friends);

            this.Guild = GuildManager.Instance.GetGuild(this.Tcharacter.GuildId);

            this.chat = new Chat(this);

            this.Info.attrDynamic = new NAttributeDynamic();
            this.Info.attrDynamic.Hp = Tdata.HP;
            this.Info.attrDynamic.Mp = Tdata.MP;
        }

        public long Exp
        {
            get { return this.Tcharacter.Exp; }
            private set
            {
                if (this.Tcharacter.Exp == value)
                    return;
                this.statusManager.AddExpChange((int)(value - this.Tcharacter.Exp));
                this.Tcharacter.Exp = value;
                this.Info.Exp = value;
            }
        }

        public int Level
        {
            get { return this.Tcharacter.Level; }
            private set
            {
                if (this.Tcharacter.Level == value)
                    return;
                this.statusManager.AddLevelUp((int)(value - this.Tcharacter.Level));
                this.Tcharacter.Level = value;
                this.Info.Level = value;
            }
        }

        public long gold
        {
            get { return this.Tcharacter.Gold; }
            set
            {
                if (this.Tcharacter.Gold == value)
                {
                    return;
                }
                
                statusManager.AddGoldStatus((int)(value - this.Tcharacter.Gold));
                this.Tcharacter.Gold = value;
                this.Info.Gold = value;
            }
        }

        public int Ride
        {
            get { return this.Info.Ride; }
            set
            {
                if (this.Info.Ride == value)
                    return;
                this.Info.Ride = value;

            }
        }


        internal void AddExp(int exp)
        {
            this.Exp += exp;
            this.CheckLevelUp();
        }

        void CheckLevelUp()
        {
            long needExp = (long)Math.Pow(this.Level, 3) * 10 + this.Level * 40 + 50;
            if (this.Exp > needExp)
            {
                this.LevelUp();
            }
        }

        void LevelUp()
        {
            this.Level += 1;
            Log.InfoFormat("Character[{0}:{1}] LevelUp:{2}", this.ID, this.Info.Name, this.Level);
            CheckLevelUp();
        }


        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()
            {
                Id = this.ID,
                Name = this.Info.Name,
                Class = this.Info.Class,
                Level = this.Info.Level
            };
        }

        internal void Clear()
        {
            this.friendManager.OfflineNotify();
        }



        public void PostProcess(NetMessageResponse message)
        {
            //Status
            if (this.statusManager.hasStatus)
            {
                this.statusManager.PostProcess(message);
            }
            //Friend
            this.friendManager.PostProcess(message);
            //Team
            if (this.Team != null)
            {
                Log.InfoFormat("PostProcess > Team : CharacterID: {0}:{1} {2}<{3}", this.ID, this.Info.Name, TeamUpdateTS, this.Team.timestamp);
                if (TeamUpdateTS < this.Team.timestamp)
                {
                    this.TeamUpdateTS = Team.timestamp;
                    this.Team.PostProcess(message);
                }
            }
            //Guild
            if (this.Guild != null)
            {
                Log.InfoFormat("PostProcess > Guild : CharacterID: {0}:{1} {2}<{3}", this.ID, this.Info.Name, GuildUpdateTS, this.Guild.timestamp);
                if (this.Info.Guild == null)  // 中途创建公会
                {
                    this.Info.Guild = this.Guild.GuildInfo(this);
                    if (message.mapCharacterEnter != null)  // 是第一次登录
                        GuildUpdateTS = Guild.timestamp;
                }
                if (GuildUpdateTS < this.Guild.timestamp && message.mapCharacterEnter == null)
                {
                    GuildUpdateTS = Guild.timestamp;
                    this.Guild.PostProcess(this, message);
                }
            }
            // Chat
            this.chat.PostProcess(message);
        }
    }
}
