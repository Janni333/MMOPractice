using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem {
	public Text membername;
	public Image classicon;
	public Image leaderIcon;
	public Image background;

	public int idx;
	public NCharacterInfo memberinfo;

    void Start()
    {
        this.background.enabled = false;
    }
    public override void OnSelected(bool selected)
    {
        this.background.enabled = selected ? true : false;
    }

    internal void SetMemberInfo(int idx, NCharacterInfo item, bool isLeader)
    {
        this.idx = idx;
        this.memberinfo = item;
        if(this.membername != null) this.membername.text = this.memberinfo.Level.ToString().PadRight(4) + this.memberinfo.Name;
        //if(this.classicon != null) this.classicon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.memberinfo.Class - 1];
        if (this.leaderIcon != null) this.leaderIcon.gameObject.SetActive(isLeader);
    }
}
