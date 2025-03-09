using Battle;
using Common.Battle;
using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Image overlay;
    public Text cdText;
    Skill skill;


    void Start()
    {
        overlay.enabled = false;
        cdText.enabled = false;
    }

    void Update()
    {
        if (this.skill.CD > 0)
        {
            if(!overlay.enabled) overlay.enabled = true;
            if(!cdText.enabled) cdText.enabled = true;

            overlay.fillAmount = this.skill.CD / this.skill.Define.CD;
            this.cdText.text = ((int)Math.Ceiling(this.skill.CD)).ToString();
        }
        else
        {
            if(overlay.enabled) overlay.enabled = false;
            if (this.cdText.enabled) this.cdText.enabled = false;
        }
    }


    internal void SetSkill(Skill value)
    {
        this.skill = value;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SkillResult  result = this.skill.CanCast(BattleManager.Instance.CurrentTarget);
        switch (result)
        {
            case SkillResult.InvalidTarget:
                MessageBox.Show("Skill[" + this.skill.Define.Name + "] Invalid Target");
                return;
            case SkillResult.OutOfMp:
                MessageBox.Show("Skill[" + this.skill.Define.Name + "] Out Of MP");
                return;
            case SkillResult.CoolDown:
                MessageBox.Show("Skill[" + this.skill.Define.Name + "] Cooldown");
                return;
        }

        BattleManager.Instance.CastSkill(this.skill);
    }
}
