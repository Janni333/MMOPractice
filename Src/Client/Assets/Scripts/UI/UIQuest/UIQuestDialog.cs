using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIQuestDialog : UIWindow
{
    public Quest quest;

    public UIQuestInfo questInfo;
    public GameObject buttonSubmit;
    public GameObject buttonAccept;
    public void SetDia(Quest quest)
    {
        this.quest = quest;
        SetInfo(quest);
        if (quest.questInfo == null)
        {
            buttonAccept.gameObject.SetActive(true);
            buttonSubmit.gameObject.SetActive(false);
        }
        else 
        {
            if (quest.questInfo.Status == QuestStatus.Complete)
            {
                buttonAccept.gameObject.SetActive(false);
                buttonSubmit.gameObject.SetActive(true);
            }
            else
            {
                buttonAccept.gameObject.SetActive(false);
                buttonSubmit.gameObject.SetActive(false);
            }
        }
    }

    void SetInfo(Quest quest)
    {
        if (this.questInfo != null)
        {
            questInfo.SetInfo(quest);
        }
    }
}
