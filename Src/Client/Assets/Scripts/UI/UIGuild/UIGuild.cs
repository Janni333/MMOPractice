using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow {
	public GameObject itemPrefab;
	public ListView listMain;
	public UIGuildInfo uiInfo;
	public UIGuildMemberItem selectedItem;

	public GameObject panelAdmin;
	public GameObject panelLeader;

	// Use this for initialization
	void Start () {
		GuildService.Instance.OnGuildUpdate += UpdateUI;
		this.listMain.OnSelectItem += this.OnGuildMemberSelected;
		this.UpdateUI();
	}

    void OnDestroy()
    {
		GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
		this.selectedItem = item as UIGuildMemberItem;
    }

    void UpdateUI()
	{
		this.uiInfo.Info = GuildManager.Instance.guildInfo;

        ClearList();
		InitItems();

		this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
		this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);
	}

    private void InitItems()
    {
		foreach (var item in GuildManager.Instance.guildInfo.Memders)
		{
			GameObject go = Instantiate(itemPrefab, this.listMain.transform);
			UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
			ui.SetGuildMemberInfo(item);
			this.listMain.AddItem(ui);
		}
    }

    private void ClearList()
    {
		this.listMain.RemoveAll();
    }

	public void OnClickAppliesList()
	{
		UIManager.Instance.Show<UIGuildApplyList>();
	}

	public void OnClickLeave()
	{
		MessageBox.Show("待扩展");
	}

	public void OnClickExpel()
	{
		if (selectedItem == null)
		{
			MessageBox.Show("请选择要踢出的成员");
			return;
		}
		MessageBox.Show(string.Format("要将【{0}】提出公会吗?", this.selectedItem.Info.Info.Name), "踢出公会", MessageBoxType.Confirm, "Yes", "Cancel").OnYes = () =>
		{
			GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.Info.Info.Id);
		};
	}

	public void OnClickPromote()
	{
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要晋升的成员");
            return;
        }
		if (selectedItem.Info.Title != GuildTitle.None)
		{
            MessageBox.Show("对方已经身份尊贵");
            return;
        }
        MessageBox.Show(string.Format("要将【{0}】晋升为副会长吗?", this.selectedItem.Info.Info.Name), "公会晋升", MessageBoxType.Confirm, "Yes", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
        };
    }

	public void OnClickDepose()
	{
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要罢免的成员");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.None)
        {
            MessageBox.Show("对方已经身份低贱");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("你不可罢免会长");
            return;
        }
        MessageBox.Show(string.Format("要罢免【{0}】的职务吗?", this.selectedItem.Info.Info.Name), "公会罢免", MessageBoxType.Confirm, "Yes", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.selectedItem.Info.Info.Id);
        };
    }

	public void OnClickTransfer()
	{
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要转让的成员");
            return;
        }
        MessageBox.Show(string.Format("要将会长转让给【{0}】吗?", this.selectedItem.Info.Info.Name), "会长转让", MessageBoxType.Confirm, "Yes", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickSetNotice()
    {
        MessageBox.Show("待扩展");
    }
}
