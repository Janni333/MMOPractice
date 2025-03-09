using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{
    public Text Name;
    public Text Description;
    public Text Count;

    internal NGuildInfo Info;

    internal void SetGuildInfo(NGuildInfo item)
    {
        this.Info = item;
        if (this.Name != null) { this.Name.text = item.GuildName; }
        if (this.Description != null) { this.Description.text = item.Notice; }
        if (this.Count != null) { this.Count.text = item.memberCount.ToString(); }
    }
}
