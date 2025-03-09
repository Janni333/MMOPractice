using Managers;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow {

	public GameObject itemPrefab;
	public ListView listMain;

	// Use this for initialization
	void Start () {
		GuildService.Instance.OnGuildUpdate += UpdateList;
		// GuildService.Instance.SendGuildLeaveRequest();
		this.UpdateList();
	}

	void OnDestroy()
	{
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }

    public void UpdateList()
    {
		ClearList();
		InitItems();
    }

    private void ClearList()
    {
		this.listMain.RemoveAll();
    }

    private void InitItems()
    {
		foreach (var item in GuildManager.Instance.guildInfo.Applies)
		{
			if (item.Result == 0)
			{
                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
                ui.SetItemInfo(item);
                this.listMain.AddItem(ui);
            }
		}
    }

    // Update is called once per frame
    void Update () {
		
	}
}
