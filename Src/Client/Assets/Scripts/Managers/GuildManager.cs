using Assets.Scripts.Model;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    internal class GuildManager : Singleton<GuildManager>
    {
        public NGuildInfo guildInfo;
        public NGuildMemberInfo myMemberInfo;

        public bool HasGuild
        {
            get { return this.guildInfo != null; }
        }

        internal void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
            if (guild == null)
            {
                myMemberInfo = null;
                return;
            }
            foreach (var mem in guild.Memders)
            {
                if (mem.characterId == User.Instance.currentCharacterInfo.Id)
                {
                    myMemberInfo = mem;
                    break;
                }
            }
        }

        internal void ShowGuild()   
        {
            if (this.HasGuild)
                UIManager.Instance.Show<UIGuild>();
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow window, UIWindow.WindowResult result)
        {
            if (result == UIWindow.WindowResult.Yes)
                UIManager.Instance.Show<UIGuildPopCreate>();
            else if (result == UIWindow.WindowResult.No)
                UIManager.Instance.Show<UIGuildList>();
        }
    }
}
