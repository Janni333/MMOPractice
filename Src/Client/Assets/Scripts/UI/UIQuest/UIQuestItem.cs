using Entities;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem 
{
	public Text itemTitle;
	public Image Background;
	Color normalbg = new Color(1f, 1f, 1f);
	Color selectedbg = new Color(0.5f, 0.5f, 0.5f);

	public Quest itemQuest;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnSelected(bool selected)
    {
		this.Background.color = selected ? selectedbg : normalbg;
    }

	public void SetItemInfo(Quest quest)
	{
		this.itemQuest = quest;
		this.itemTitle.text = quest.questDefine.Name;
	}
}
