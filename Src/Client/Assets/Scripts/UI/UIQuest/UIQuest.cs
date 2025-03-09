using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : UIWindow 
{
	public Text questTitle;
	public Button buttonClose;
	public GameObject itemPrefab;

	public ListView questMain;
	public ListView questBranch;

	public UIQuestInfo questInfo;

	private bool ShowAvailable = true;

	// Use this for initialization
	void Start () {
		questBranch.OnSelectItem += this.OnSelectItem;
		questMain.OnSelectItem += this.OnSelectItem;
		SetInfo();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetInfo()
	{
		foreach (var quest in QuestManager.Instance.allQuests.Values)
		{
			if (ShowAvailable)
			{
				if (quest.questInfo != null)
					continue;
			}
			else
			{
				if (quest.questInfo == null)
					continue;
			}

			GameObject uiquestitem = Instantiate(itemPrefab, quest.questDefine.Type == Common.Data.QuestType.Main ? questMain.transform : questBranch.transform);
			UIQuestItem ui = uiquestitem.GetComponent<UIQuestItem>();
			ui.SetItemInfo(quest);
			if (quest.questDefine.Type == Common.Data.QuestType.Main)
				this.questMain.AddItem(ui);
			if (quest.questDefine.Type == Common.Data.QuestType.Branch)
				this.questBranch.AddItem(ui);
		}
	}

	void OnSelectItem(ListView.ListViewItem item)
	{
		UIQuestItem uiitem = item as UIQuestItem;
		this.questInfo.SetInfo(uiitem.itemQuest);
	}

	void ClearAllList()
	{
		questMain.RemoveAll();
		questBranch.RemoveAll();
	}
}
