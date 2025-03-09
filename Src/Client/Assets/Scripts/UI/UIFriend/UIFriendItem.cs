using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem {
	public NFriendInfo friendInfo;

	public Image friendBackground;
	public Image friendAvatar;
	public Text friendName;
	public Text friendLevel;
	public Text friendClass;
	public Text friendStatus;

	Color normalcolor = new Color(1f, 0.76f, 0.39f);
	Color selectedcolor = new Color(1f, 0.58f, 0.32f);
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    internal void SetFriendInfo(NFriendInfo friend)
    {
		this.friendInfo = friend;

		this.friendName.text = friend.Friendinfo.Name;
		this.friendLevel.text = string.Format("LV{0}", friend.Friendinfo.Level);
		//this.friendClass.text = friend.Friendinfo.Class.ToString();

		this.friendStatus.text = friend.Satus == 1 ? "Online" : "Offline";
    }

    public override void OnSelected(bool selected)
    {
		this.friendBackground.color = selected ? selectedcolor : normalcolor;
    }


	public void OnClickChat()
	{
		MessageBox.Show("功能暂未开放");
	}
}
