using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * 240405
 *	点击创建逻辑
 */

public class UICharacterCreate : MonoBehaviour {
	public GameObject SelectPanel;

	public Text Description;
	public Button CreateButton;
	public InputField Name;
	public UICharacterView Chars;

	// Use this for initialization
	void Start () {
		UserService.Instance.OnCreateCharacter += OnCreate;
	}

	void OnEnable()
	{
		this.InitCreate();
	}
    // Update is called once per frame
    void Update () {
	}
	public void InitCreate()
	{
		Chars.Index = 0;
	}
	public void OnClickCreate()
	{
		//校验输入
		if (Name.text == null)
		{
			MessageBox.Show("请输入角色名称", "Character Create Info");
		}

		//调用服务层发送消息
		int charClass = this.Chars.Index + 1;
		UserService.Instance.SendCreateCharacter(Name.text, (CharacterClass)charClass);
	}

	private void OnCreate(Result result, string msg)
	{
		//反馈
		MessageBox.Show(msg, string.Format("{0}", result));

		//初始化角色选择
		if (result == Result.Success)
		{
			this.gameObject.SetActive(false);
			this.SelectPanel.SetActive(true);
		}

	}

	public void OnClickBack()
	{
		this.gameObject.SetActive(false);
		this.SelectPanel.SetActive(true);
	}
}
