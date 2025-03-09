using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : UIWindow {
    public Text descript;
    public GameObject itemPrefab;
    public ListView listMain;
    public UISkillItem selectedItem;

    void Start()
    {
        RefreshUI();
        this.listMain.OnSelectItem += this.OnItemSelected;
    }

    private void OnDestroy()
    { }

    public void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UISkillItem;
        this.descript.text = this.selectedItem.item.Define.Description;
    }

    private void RefreshUI()
    {
        ClearItems();
        InitItems();
    }

    private void ClearItems()
    {

    }

    /// <summary>
    /// 初始化技能列表
    /// </summary>
    private void InitItems()
    {
        var Skills = User.Instance.currentCharacter.SkillMgr.Skills;
        foreach (var skill in Skills)
        {
            if (skill.Define.Type == Common.Battle.SkillType.Skill)
            {
                GameObject go = Instantiate(itemPrefab, this.listMain.transform);
                UISkillItem ui = go.GetComponent<UISkillItem>();
                ui.SetItem(skill, this, false);
                this.listMain.AddItem(ui);
            }
        }
    }

    void ClearItem()
    {
        this.listMain.RemoveAll();
    }
}
