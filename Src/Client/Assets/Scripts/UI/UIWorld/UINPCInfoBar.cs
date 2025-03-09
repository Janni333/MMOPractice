using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UINPCInfoBar : MonoBehaviour
{
    //Info
    public Text Name;

    //QuestStatus
    public Image[] Status;
    public NPCQuestStatus npcStatus;

    void Start()
    {
        
    }

    void Update()
    { }
    public void SetNpcQuest(NPCQuestStatus status)
    {
        this.npcStatus = status;
        for (int i = 0; i < 4; i++)
        {
            if (this.Status[i] != null)
            {
                this.Status[i].gameObject.SetActive(i == (int)status);
            }
        }
    }

    public void SetNpcName(string name)
    {
        this.Name.text = name;
    }
}
