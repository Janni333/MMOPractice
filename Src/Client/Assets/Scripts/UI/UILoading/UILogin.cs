using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * 240331
 *	登录逻辑
 *	OnClickLogin
 *	
 *240401
 *	OnClickLogin
 *	OnLogin
 */
public class UILogin : MonoBehaviour {
	public InputField username;
	public InputField password;

	public GameObject RegisterPanel;
	// Use this for initialization
	void Start () {
		UserService.Instance.OnLogin += this.OnLogin;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickRegister()
	{
		//UI
		this.gameObject.SetActive(false);
		this.RegisterPanel.SetActive(true);
	}

	public void OnClickLogin()
	{
		//UI
		//Data
		//	校验输入
		//	调用UserService
		if (username.text == "")
		{
			MessageBox.Show("Please input your username", "Login Info");
			return;
		}
		else if (password.text == "")
		{
			MessageBox.Show("Please input your password", "Login Info");
			return;
		}
		else 
		{
			UserService.Instance.SendLogin(username.text, password.text);
		}
	}

	void OnLogin(Result result, string msg)
	{
		//登录失败反馈
		if (result == Result.Failed)
		{
			MessageBox.Show(msg, string.Format("{0}", result));
		}
		//登录成功切换场景
		else
        {
			SceneManager.Instance.LoadScene("CharChoose");
        }
	}
}
