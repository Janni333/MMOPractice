using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour , IPointerClickHandler
{
	public Image shopitemBack;
	public Image shopitemIcon;
	public Text shopitemPrice;
	public Text shopitemName;
	public Selectable shopitemSelect;
	public int shopitemId;
	public bool isSelected
	{
		set 
		{
			this.selectColor(value);
		}
	}
	Color normal = new Color(1f, 1f, 1f);
	Color selected = new Color(0.7f, 1f, 1f);


    public ItemDefine itemDef;
	public ShopItemDefine shopitemDef;

	public UIShop thisShop;

	// Use this for initialization
	void Start () {
		if (shopitemSelect == null)
		{
			shopitemSelect = this.GetComponent<Selectable>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetInfo(int id, ShopItemDefine def)
	{
		shopitemDef = def;
		shopitemId = id;
		itemDef = DataManager.Instance.Items[def.ItemID];
		isSelected = false;

		this.shopitemIcon.overrideSprite = Resloader.Load<Sprite>(itemDef.Icon);
		this.shopitemPrice.text = def.Price.ToString();
		this.shopitemName.text = itemDef.Name + " * " + def.count.ToString();
	}

    public void OnPointerClick(PointerEventData eventData)
    {
		this.thisShop.shopSelected = this;
    }

	private void selectColor(bool value)
	{
		if (value)
		{
			this.shopitemBack.color = selected;
		}
		else
		{
			this.shopitemBack.color = normal;
		}
	}
}
