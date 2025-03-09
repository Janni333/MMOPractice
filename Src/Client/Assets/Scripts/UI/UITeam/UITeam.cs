using Assets.Scripts.Model;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : MonoBehaviour {
    public Text teamTitle;
    public UITeamItem[] Members;
    public ListView list;


    void OnEnable()
    {
        UpdateTeamUI();
    }
    void Start()
    {
        if (User.Instance.currentTeam == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        foreach (var item in Members)
        {
            this.list.AddItem(item);
        }
    }

    public void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
        {
            UpdateTeamUI();
        }
    }

    public void UpdateTeamUI()
    {
        NTeamInfo myTeam = User.Instance.currentTeam;
        if (User.Instance.currentTeam == null) return;

        this.teamTitle.text = string.Format("My Team {0}/5", myTeam.Members.Count);
        for (int i = 0; i < 5; i++)
        {
            if (i < myTeam.Members.Count)
            {
                this.Members[i].SetMemberInfo(i, myTeam.Members[i], myTeam.Members[i].Id == myTeam.Leader);
                this.Members[i].gameObject.SetActive(true);
            }
            else
            {
                this.Members[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnClickLeave()
    { 
    }
}
