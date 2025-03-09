using Common.Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class UIBagItem : MonoBehaviour
{
    public Image itemIcon;
    public Text itemCount;

    public void setUIinfo(BagItem item)
    {
        ItemDefine itemdef = DataManager.Instance.Items[item.ItemId];
        itemIcon.overrideSprite = Resloader.Load<Sprite>(itemdef.Icon);
        itemCount.text = item.ItemCount.ToString();
    }
}