using Battle;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UISkillItem : ListView.ListViewItem
{
    public Text title;
    public Text level;
    public Image icon;

    public Image background;
    public Sprite normalBg;
    public Sprite selectBg;

    public Skill item;

    public override void OnSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectBg : normalBg;
    }
    public void SetItem(Skill item, UISkill owner, bool equiped)
    {
        this.item = item;

        if (this.title != null) this.title.text = item.Define.Name;
        if (this.level != null) this.level.text = item.Info.Level.ToString() ;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }
}
