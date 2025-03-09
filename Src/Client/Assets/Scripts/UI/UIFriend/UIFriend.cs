using Assets.Scripts.Model;
using Managers;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriend : UIWindow {
	public GameObject itemPrefab;

	public ListView friendList;
	public Transform friendRoot;
	public UIFriendItem selectedItem;

	void Start () {
		//Event
		FriendService.Instance.OnFriendUpdate += this.Refresh;
		this.friendList.OnSelectItem += this.OnFriendSelect;

		//Init
		this.Refresh();
	}


    #region Init
    public void Refresh()
	{
		this.ClearInfo();
		this.InitInfo();
	}

	public void ClearInfo()
	{
		this.friendList.RemoveAll();
	}

	public void InitInfo()
	{
		foreach (var friend in FriendManager.Instance.Friends)
		{
			GameObject go = Instantiate(this.itemPrefab, this.friendList.transform);
			UIFriendItem itemui = go.GetComponent<UIFriendItem>();
			itemui.SetFriendInfo(friend);
			this.friendList.AddItem(itemui);
			go.SetActive(true);
		}
	}
	#endregion

	#region Event

	//	选中好友
	private void OnFriendSelect(ListView.ListViewItem friendItem)
	{
		this.selectedItem = friendItem as UIFriendItem;
	}

	//	添加好友
	public void OnClickAdd()
	{
		InputBox.Show("Please Input the friend ID", "Friend Add", "Add Friend", "Close").OnSubmit += this.OnAddSubmit;
	}
	//	提交好友申请
	public bool OnAddSubmit(string input, out string tips)
	{
        tips = "";
		int friendid = 0;
		string friendname = "";
		if (!int.TryParse(input, out friendid))
			friendname = input;
		if (friendid == User.Instance.currentCharacterInfo.Id || friendname == User.Instance.currentCharacterInfo.Name)
		{
			tips = "不能添加自己";
			return false;
		}

		FriendService.Instance.SendFriendAddRequest(friendid, friendname);
        return true;
	}
	//	移除好友
	public void OnClickRemove()
	{
		if (this.selectedItem == null)
		{
			MessageBox.Show("请选择要删除的好友");
			return;
		}
		MessageBox.Show(string.Format("确定要删除好友{0}么", selectedItem.friendInfo.Friendinfo.Name), "Remove Friend", MessageBoxType.Confirm, "Remove", "Cancel").OnYes = () =>
		{
			FriendService.Instance.SendFriendRemoveRequest(selectedItem.friendInfo.Id, selectedItem.friendInfo.Friendinfo.Id);
		};
	}

	// 邀请好友组队
	public void OnClickInviteTeam()
	{
		// 校验
		if (this.selectedItem == null)
		{
			MessageBox.Show("Please select a friend");
			return;
		}
		if (this.selectedItem.friendInfo.Satus == 0)
		{
			MessageBox.Show("Please select a online friend");
			return;
		}

		MessageBox.Show(string.Format("确定要邀请玩家{0}组队", this.selectedItem.friendInfo.Friendinfo.Name), "Confirm", MessageBoxType.Confirm, "Confirm", "Cancle").OnYes = () =>
		{
			 TeamService.Instance.SendTeamInviteRequest(selectedItem.friendInfo.Friendinfo.Id, selectedItem.friendInfo.Friendinfo.Name);
		}; 
	}
	#endregion
}
