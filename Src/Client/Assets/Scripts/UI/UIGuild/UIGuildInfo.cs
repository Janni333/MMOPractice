using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour
{
    public Text guildName;
    public Text guildID;
    public Text guildLeader;
    public Text guildNotice;
    public Text guildmembercount;

    private NGuildInfo info;
    public NGuildInfo Info
    {
        get { return this.info; }
        set 
        {
            this.info = value;
            this.UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (this.info == null)
        {
            this.guildName.text = "None";
            this.guildID.text = "ID: 0";
            this.guildLeader.text = "Leader: None";
            this.guildNotice.text = "None";
            this.guildmembercount.text = string.Format("Members: 0 / 50");
        }
        else
        {
            this.guildName.text = this.info.GuildName;
            this.guildID.text = "ID:" + this.info.Id;
            this.guildLeader.text = "Leader:" + this.info.leaderName;
            this.guildNotice.text = this.info.Notice;
            this.guildmembercount.text = string.Format("Members: {0} / 50", this.info.memberCount);
        }
    }
}
