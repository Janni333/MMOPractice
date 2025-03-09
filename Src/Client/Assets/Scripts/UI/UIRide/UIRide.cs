using Assets.Scripts.Model;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow {
	public Text description;
	public GameObject itemPrefab;
	public ListView listMain;
	private UIRideItem selectedItem;

	// Use this for initialization
	void Start () {
		RefreshUI();
		this.listMain.OnSelectItem += this.OnItemSelected;
	}

	void OnDestroy()
	{

	}

    private void OnItemSelected(ListView.ListViewItem item)
    {
		this.selectedItem = item as UIRideItem;
		this.description.text = this.selectedItem.item.itemdef.Description;
    }

    private void RefreshUI()
    {
		ClearItems();
		InitItems();
    }

    private void InitItems()
    {
		foreach(var kv in ItemManager.Instance.Items)
		{
			// 筛选显示范围：坐骑且职业正确
			if (kv.Value.itemdef.Type == ItemType.Ride &&
				(kv.Value.itemdef.LimitClass == CharacterClass.None || kv.Value.itemdef.LimitClass == User.Instance.currentCharacterInfo.Class)
				)
			{
				GameObject go = Instantiate(itemPrefab, this.listMain.transform);
				UIRideItem ui = go.GetComponent<UIRideItem>();
				ui.SetRideItem(kv.Value, this, false);
				this.listMain.AddItem(ui);
			}
		}
    }

    private void ClearItems()
    {
		this.listMain.RemoveAll();
    }

	public void DoRide()
	{
		if (selectedItem == null)
		{
			MessageBox.Show("Please choose a Ride", "Info");
			return;
		}
		User.Instance.Ride(this.selectedItem.item.ItemID);
    }
}
