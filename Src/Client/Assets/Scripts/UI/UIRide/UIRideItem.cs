using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIRideItem : ListView.ListViewItem
{
    public Image icon;
    public Text title;
    public Text level;

    public Image background;
    public Sprite nornalBg;
    public Sprite selectedBg;

    public Item item;

    public override void OnSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : nornalBg;
    }

    void Start()
    { }

    public void SetRideItem(Item item, UIRide owner, bool equiped)
    {
        this.item = item;

        if (this.title != null) this.title.text = this.item.itemdef.Name;
        if (this.level != null) this.level.text = this.item.itemdef.Level.ToString();
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.itemdef.Icon);
    }

}
