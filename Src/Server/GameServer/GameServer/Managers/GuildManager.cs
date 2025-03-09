using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        public HashSet<string> GuildNames = new HashSet<string>();

        public void Init()
        {
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.TGuilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }
        private void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }

        internal bool CheckNameExisted(string guildName)
        {
            return this.GuildNames.Contains(guildName); 
        }

        internal bool CreateGuild(string guildName, string guildNotice, Character leader)
        {
            DateTime now = DateTime.Now;
            TGuild dbguild = DBService.Instance.Entities.TGuilds.Create();

            dbguild.Name = guildName;
            dbguild.Notice = guildNotice;
            dbguild.LeaderID = leader.ID;
            dbguild.LeaderName = leader.Name;
            dbguild.CreateTime = now;
            DBService.Instance.Entities.TGuilds.Add(dbguild);

            Guild guild = new Guild(dbguild);
            guild.AddMember(leader.ID, leader.Name, leader.Tcharacter.Class, leader.Tcharacter.Level, GuildTitle.President);
            leader.Guild = guild;
            DBService.Instance.Save();
            leader.Tcharacter.GuildId = dbguild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);

            return true;
        }

        internal Guild GetGuild(int guildId)
        {
            if(guildId == 0)
                return null;
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result = new List<NGuildInfo>();
            foreach (var kv in this.Guilds)
            {
                result.Add(kv.Value.GuildInfo(null));
            }
            return result;
        }
    }
}
