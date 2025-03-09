using Assets.Scripts.Model;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 240406
 *	创建界面逻辑
 * 240407
 *	选择界面逻辑
 */

/*
 * 增删改
 *	1 生成角色列表逻辑
 */

public class UICharacterSelect : MonoBehaviour {
	public GameObject CreatePanel;

	public Transform CharList;
	public GameObject CharItem;
	public UICharacterView CharView;

	//1 管理列表元素用
	List<GameObject> UICharItemList = new List<GameObject>();
	int selectedIndex;

	// Use this for initialization
	void Start () {
		
	}

	void OnEnable()
	{
		this.IniSelect();
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void IniSelect()
	{
		//修改1 生成列表逻辑
		//清理原有UI列表
		if (this.UICharItemList != null)
		{
			foreach (var item in this.UICharItemList)
			{
				Destroy(item);
			}
			this.UICharItemList.Clear();
		}
		//遍历User所有角色生成UI列表
		for (int i = 0; i < User.Instance.UserInfo.Player.Characters.Count; i++)
		{
			GameObject charitem = Instantiate(CharItem, CharList);
			UICharItem charItem = charitem.GetComponent<UICharItem>();
			charItem.info = User.Instance.UserInfo.Player.Characters[i];
			Button button = charitem.GetComponent<Button>();
			int idx = i;
			button.onClick.AddListener(() => { OnSelectcharItem(idx); });
			this.UICharItemList.Add(charitem);
			//激活
			charitem.SetActive(true);
		}
		//初始化角色列表选择状态
		if (UICharItemList == null)
		{
			this.CharView.Index = -1;
		}
		else 
		{
			this.OnSelectcharItem(0);
		}
	}

	public void OnSelectcharItem(int idx)
	{
		selectedIndex = idx;
		CharView.Index = (int)this.UICharItemList[idx].GetComponent<UICharItem>().info.Class - 1;
		for (int i = 0; i < this.UICharItemList.Count; i++)
		{
			this.UICharItemList[i].GetComponent<UICharItem>().Selected(i == idx);
		}
	}

	public void OnClickCreate()
	{
		this.gameObject.SetActive(false);
		this.CreatePanel.SetActive(true);
	}

	public void OnClickEnter()
	{
		if (this.selectedIndex >= 0)
		{
			UserService.Instance.SendGameEnter(selectedIndex);
		}
	}
}
