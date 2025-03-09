using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*240331
 *	注册逻辑 OnClickOk
 *	注册返回逻辑 OnRegister 
 */
public class UIRegister : MonoBehaviour {
	public InputField username;
	public InputField password;
	public InputField password2;

	public GameObject LoginPanel;

	// Use this for initialization
	void Start () {
		UserService.Instance.OnRegister = this.OnRegister;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void OnClickOk()
	{		
		/*Data
		 *	输入校验
		 *	数据发送
		 */
		if (username.text == null)
		{
			MessageBox.Show("Please input your username", "Register Info");
			return;
		}
		if (password.text == null)
		{
			MessageBox.Show("Please input your password", "Register Info");
			return;
		}
		if (password2.text == null)
		{
			MessageBox.Show("Please confirm your password", "Register Info");
			return;
		}
		if (password.text != password2.text)
		{
			MessageBox.Show("Please confirm your password", "Register Info");
			return;
		}
		else
		{
			UserService.Instance.SendRegister(username.text, password.text);
			//UI
			this.gameObject.SetActive(false);
			this.LoginPanel.SetActive(true);
		}
	}

	public void OnClickBack()
	{
		this.gameObject.SetActive(false);
		this.LoginPanel.SetActive(true);
	}

	public void OnRegister(Result result, string msg)
	{
		MessageBox.Show(msg, string.Format("{0}", result));

		if (result == Result.Success)
		{
			this.gameObject.SetActive(false);
			this.LoginPanel.SetActive(true);
		}
	}
}
