using Common;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Team
    {
        public int Id;
        public Character Leader;
        public List<Character> Members = new List<Character>();

        public double timestamp;

        public Team(Character leader)
        {
            this.AddMember(leader);
        }

        public void AddMember(Character member)
        {
            Log.InfoFormat("Member {0}:{1} Join Team", member.ID, member.Info.Name);
            if (this.Members.Count == 0)
            {
                this.Leader = member;
            }
            this.Members.Add(member);
            member.Team = this;
            timestamp = TimeUtil.timestamp;
        }

        public void MemberLeave(Character member)
        {
            Log.InfoFormat("Member {0}:{1} Leave Team", member.ID, member.Info.Name);
            this.Members.Remove(member);
            if (member == this.Leader)
            {
                if (this.Members.Count > 0)
                {
                    this.Leader = this.Members[0];
                }
                else
                {
                    this.Leader = null;
                }
            }
            member.Team = null;
            timestamp = TimeUtil.timestamp;

        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.teamInfo == null)
            {
                message.teamInfo = new TeamInfoResponse();
                message.teamInfo.Result = Result.Success;
                message.teamInfo.Teaminfo = new NTeamInfo();
                message.teamInfo.Teaminfo.Id = this.Id;
                message.teamInfo.Teaminfo.Leader = this.Leader.ID;
                foreach (var member in this.Members)
                {
                    message.teamInfo.Teaminfo.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}
