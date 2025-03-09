using Assets.Scripts.Model;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour {

	public Text questTitle;

	public Text questTarget;
	public Text questDescription;
	public Text overview;
	public Text questGold;
	public Text questExp;

	public Image questReward;
	public UIRewardItem questRewardItem;

	public Button navButton;
	private int npc = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetInfo(Quest quest)
	{
		this.questTitle.text = string.Format("[{0}]{1}", quest.questDefine.Type, quest.questDefine.Name);

		if (this.overview != null) this.overview.text = quest.questDefine.OverView;

		if(this.questDescription != null)
		{
            if (quest.questInfo == null)
            {
                this.questDescription.text = quest.questDefine.Dialog;
            }
            else
            {
                if (quest.questInfo.Status == SkillBridge.Message.QuestStatus.Complete)
                {
                    this.questDescription.text = quest.questDefine.DialogFinish;
                }
            }
        }
		

		questGold.text = quest.questDefine.RewardGold.ToString();
		questExp.text = quest.questDefine.RewardExp.ToString();

		if (quest.questInfo == null)
		{
			this.npc = quest.questDefine.AcceptNPC;
		}
		else if (quest.questInfo.Status == SkillBridge.Message.QuestStatus.Complete)
		{
			this.npc = quest.questDefine.SubmitNPC;
		}

		this.navButton.gameObject.SetActive(this.npc > 0);

		foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
		{
			fitter.SetLayoutVertical();
			fitter.SetLayoutHorizontal();
		}
	}

	public void OnClickNav()
	{
		Vector3 pos = NPCManager.Instance.GetNpcPosition(this.npc);
		User.Instance.currentCharacterObj.StartNav(pos);
		UIManager.Instance.Close<UIQuest>();
	}
}
