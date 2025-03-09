using Assets.Scripts.Model;
using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class NPCController : MonoBehaviour
{
    public int NpcId;
    public NPCDefine NpcDef;
    NPCQuestStatus NpcStatus;

    bool isInteractive = false;

    void Start()
    {
        isInteractive = true;
        this.NpcDef = DataManager.Instance.NPCs[NpcId];
        //回头补：动画协程

        NPCManager.Instance.UpdateNpcPosition(this.NpcId, this.transform.position);

        //Info
        SetNpcInfo();
        QuestManager.Instance.OnQuestStatusChanged += this.OnStatusChange;
    }

    #region Quest
    void OnStatusChange(Quest quest)
    {
        this.SetNpcInfo();
    }

    void SetNpcInfo()
    {
        NpcStatus = QuestManager.Instance.GetQuestStatusbyNPC(NpcId);
        UINPCInfoBar ui = UIWorldManager.Instance.AddNpcinfobar(this.transform);
        ui.SetNpcName(NpcDef.Name);
        ui.SetNpcQuest(NpcStatus);
    }
    #endregion

    #region interact
    void OnMouseDown()
    {
        if (Vector3.Distance(this.transform.position, User.Instance.currentCharacterObj.transform.position) > 2f)
        {
            User.Instance.currentCharacterObj.StartNav(this.transform.position);
        }
        Interact();
    }

    void Interact()
    {
        if (isInteractive)
        {
            isInteractive = false;
            StartCoroutine(doInteraction());
        }
    }

    IEnumerator doInteraction()
    {
        yield return faceToPlayer();
        NPCManager.Instance.NpcInteract(this.NpcDef, this.NpcStatus);

        yield return new WaitForSeconds(3f);
        isInteractive = true;
    }

    IEnumerator faceToPlayer()
    {
        Vector3 faceto = (User.Instance.currentCharacterObj.transform.position - this.transform.position).normalized;
        while (Math.Abs(Vector3.Angle(this.transform.forward,faceto)) > 5)
        {
            this.transform.forward = Vector3.Lerp(this.transform.forward, faceto, Time.deltaTime * 5f);
            yield return null;
        }
    }

    #endregion
}
