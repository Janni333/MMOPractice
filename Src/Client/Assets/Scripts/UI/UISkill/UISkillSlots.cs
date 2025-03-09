using Assets.Scripts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UISkillSlots : MonoBehaviour
{
    public UISkillSlot[] slots;

    void Start()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        // todo 
        var Skills = User.Instance.currentCharacter.SkillMgr.Skills;
        int skillIdx = 0;
        foreach (var skill in Skills)
        {
            slots[skillIdx].SetSkill(skill);
            skillIdx++;
        }
    }
}
