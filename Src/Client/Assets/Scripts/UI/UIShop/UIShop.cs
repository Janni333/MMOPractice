using Assets.Scripts.Model;
using Common.Data;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow {
    public ShopDefine shopDef;
    
    public Text shopName;
    public Text Gold;

    public Transform shopContent;
    public GameObject UIShopItem;
    public List<UIShopItem> UIShopItems;

    private UIShopItem shopselected;
    public UIShopItem shopSelected
    {
        get { return this.shopselected; }
        set 
        { 
            this.shopselected = value;
            this.selectEf(value);
        }
    }

    void Start () {
	}

    #region Init Shop
    public void InitShop(int shopid)
    {
        this.SetInfo(shopid);
        StartCoroutine(SetShopItem(shopid));
    }

    public void SetInfo(int shopid)
    {
        this.shopDef = DataManager.Instance.Shops[shopid];
        this.shopName.text = this.shopDef.Name;
        this.Gold.text = User.Instance.currentCharacterInfo.Gold.ToString();
    }


    IEnumerator SetShopItem(int shopid)
    {
        foreach (var shopitem in DataManager.Instance.ShopItems[shopid])
        {
            GameObject uishopitem = Instantiate(this.UIShopItem, this.shopContent);
            UIShopItem ui = uishopitem.GetComponent<UIShopItem>();
            ui.thisShop = this;
            ui.SetInfo(shopitem.Key, shopitem.Value);
            this.UIShopItems.Add(ui);
        }
        yield return null;
    }
    #endregion

    #region Items Interaction
    private void selectEf(UIShopItem value)
    {
        foreach (var item in this.UIShopItems)
        {
            item.isSelected = (item.shopitemId == value.shopitemId);
        }
    }
    #endregion

    #region Buy Item
    public void OnClickBuy()
    {
        if (this.shopselected == null)
        {
            MessageBox.Show("请选取要购买的商品");
            return;
        }

        MessageBox.Show("确定要购买此商品吗", "Confirm", MessageBoxType.Confirm, "购买", "Cancle").OnYes = () =>
          {
              ShopManager.Instance.ItemBuy(this.shopDef.ID, this.shopselected.shopitemId);
          };
        
    }
    #endregion

}