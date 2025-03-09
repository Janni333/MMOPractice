using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour {
	public Text title;
	public Text tips;
	public Text message;
	public InputField input;
	public Button btnOK;
	public Button btnCancel;
	public Text btnOKtext;
	public Text btnCanceltext;

	public delegate bool SubmitHandler(string input, out string tips);
	public event SubmitHandler OnSubmit;
	public UnityAction OnCancel;

	public string emptyTips;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void Init(string message, string title, string btnok, string btnCancle, string emptytips)
    {
		if (!string.IsNullOrEmpty(title))
			this.title.text = title;
		if (!string.IsNullOrEmpty(btnok))
			this.btnOKtext.text = title;
		if (!string.IsNullOrEmpty(btnCancle))
			this.btnCanceltext.text = title;
		this.message.text = message;
		this.tips.text = null;

		this.OnSubmit = null;
		this.emptyTips = emptytips;
    }

    

    public void OnClickOK()
    {
		this.tips.text = "";
		//检查输入
		if (string.IsNullOrEmpty(this.input.text))
		{
			this.tips.text = emptyTips;
			return;
		}
		//触发事件，根据输入返回执行结果和回执
		if (this.OnSubmit != null)
		{
			string tips;
			if (!OnSubmit(input.text, out tips))
			{
				this.tips.text = tips;
				return;
			}
		}
		//关闭当前页面
		Destroy(this.gameObject);
    }

	public void onClickNo()
	{
		Destroy(this.gameObject);
		if(OnCancel != null)
			this.OnCancel();
	}
}
