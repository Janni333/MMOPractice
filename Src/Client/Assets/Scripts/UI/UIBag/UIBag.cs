using Assets.Scripts.Model;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class UIBag : UIWindow
{
    public GameObject UIBagItem;

    public Button closeButton;
    public Text moneyText;
    public UIBagView bagView;

    public List<Transform> bagContents = new List<Transform>();
    public List<Image> bagSlots;

    void Start()
    {
        // 获取槽位
        if (this.bagSlots == null || this.bagSlots.Count == 0)
        {
            bagSlots = new List<Image>();
            for(int i = 0; i < this.bagContents.Count; i ++)
            {
                bagSlots.AddRange(this.bagContents[i].GetComponentsInChildren<Image>(true));
            }
        }
        BagManager.Instance.Reset();
        StartCoroutine(InitBag());
    }
    void OnReset()
    {
        BagManager.Instance.Reset();
        this.ClearBag();
        StartCoroutine(InitBag());
    }


    IEnumerator InitBag()
    {
        for(int i = 0; i < BagManager.Instance.bagItems.Length; i ++)
        {
            if (BagManager.Instance.bagItems[i].ItemId > 0)
            {
                GameObject uibagitem = Instantiate(this.UIBagItem, bagSlots[i].transform);
                UIBagItem ui = uibagitem.GetComponent<UIBagItem>();
                ui.setUIinfo(BagManager.Instance.bagItems[i]);
            }
        }
        this.SetMoney();
        yield return null;
    }

    public void SetMoney()
    {
        this.moneyText.text = User.Instance.currentCharacterInfo.Gold.ToString();
    }

    public void ClearBag()
    {
        for (int i = 0; i < this.bagSlots.Count; i++)
        {
            if (bagSlots[i].transform.childCount > 0)
            {
                Destroy(bagSlots[i].transform.GetChild(0).gameObject);
            }
        }
    }
}
