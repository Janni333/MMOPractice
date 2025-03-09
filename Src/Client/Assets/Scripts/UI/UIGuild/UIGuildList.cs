using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;

internal class UIGuildList: UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public UIGuildInfo uiInfo;
    public UIGuildItem selectedItem;

    void Start()
    {
        this.listMain.OnSelectItem += this.OnGuildMemberSelected;
        this.uiInfo.Info = null;

        GuildService.Instance.OnGuildListResult += UpdateGuildList;
        GuildService.Instance.SendGuildListResquest();
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        IniteItems(guilds);
    }

    private void IniteItems(List<NGuildInfo> guilds)
    {
        foreach (var item in guilds)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        this.listMain.RemoveAll();
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }


    public void OnClickJoin()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("Please select a guild");
            return;
        }
        MessageBox.Show(string.Format("确定要加入公会【{0}】么？", selectedItem.Info.GuildName), "Apply", MessageBoxType.Confirm, "Yes", "Cancel").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info.Id);
        };
    }
}