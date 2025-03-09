using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Guild
    {
        public TGuild Data;
        public int Id { get { return this.Data.Id; } }
        public string Name { get { return this.Data.Name; } }
        public double timestamp;

        public Guild(TGuild guild)
        {
            this.Data = guild; 
        }

        public NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            { 
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };

            if (from != null)
            {
                info.Memders.AddRange(GetMemberInfos());
                if (from.ID == this.Data.LeaderID)
                    info.Applies.AddRange(GetApplyInfos());
            }
            return info;
        }

        public List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach (var apply in this.Data.Applies)
            {
                if (apply.Result != (int)ApplyResult.None) continue;
                applies.Add(new NGuildApplyInfo() 
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Name = apply.Name,
                    Class = apply.Class,
                    Level = apply.Level,
                    Result = (ApplyResult)apply.Result,
                });
            }

            return applies;
        }

        public List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Tcharacter.Level;
                    member.Name = character.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                }

                members.Add(memberInfo);
            }

            return members;

            
        }

        public NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.Id,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };
        }

        TGuildMember GetDBMember(int characterId)
        {
            foreach (var member in this.Data.Members)
            {
                if(member.CharacterId == characterId)
                    return member;
            }
            return null;
        }

        public bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null)
            {
                return false;
            }

            var dbApply = DBService.Instance.Entities.TGuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level ;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.TGuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);

            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        public bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);  // 结果为0为未审批
            if (oldApply == null)
            {
                return false;
            }

            oldApply.Result = (int)apply.Result;
            if (apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            }

            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }

        public void AddMember(int characterId, string name, int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbmember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int) title,
                JoinTime = now,
                LastTime = now,
            };

            this.Data.Members.Add(dbmember);

            var character = CharacterManager.Instance.GetCharacter(characterId);
            if (character != null)
                character.Tcharacter.GuildId = this.Id;
            else
            {
                // DBService.Instance.Entities.Database.ExecuteSqlCommand("UPDATE Characters SET GuildId = @p0 WHERE CharacterId = @p1", this.Id, characterId);
                TCharacter dbChar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                dbChar.GuildId = this.Id;
            }
            timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character character)
        {
            Log.InfoFormat("Leave Guild : {0:{1}}", character.ID, character.Name);
            timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(Character from, NetMessageResponse message)
        {
            if (message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.Guild = this.GuildInfo(from);
            }
        }

        internal void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(sourceId);
            switch (command)
            {
                case GuildAdminCommand.Promote:
                    target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    target.Title = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Transfer:
                    target.Title = (int)GuildTitle.President;
                    source.Title = (int)GuildTitle.None;
                    this.Data.LeaderID = targetId;
                    this.Data.LeaderName = target.Name;
                    break;
                case GuildAdminCommand.Kickout:
                    // To
                    break;
            }

            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
        }
    }
}
