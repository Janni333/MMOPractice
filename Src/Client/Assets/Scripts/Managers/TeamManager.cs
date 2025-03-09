using Assets.Scripts.Model;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class TeamManager: Singleton<TeamManager>
    {
        public void Init()
        { }

        public void UpdateTeamInfo(NTeamInfo teamInfo)
        {
            User.Instance.currentTeam = teamInfo;
            this.RefreshTeamUI(teamInfo != null);
        }

        public void RefreshTeamUI(bool show)
        {
            if (UIMain.Instance != null)
            {
                UIMain.Instance.ShowTeamUI(show);
            }
        }
    }
}
