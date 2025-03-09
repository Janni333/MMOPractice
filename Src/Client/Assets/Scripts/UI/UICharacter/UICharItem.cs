using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharItem : MonoBehaviour {
	public Text Name;
	public Text Class;
	public Button button;
	public NCharacterInfo info;

	// Use this for initialization
	void Start () {
		if (info != null)
		{
			this.Name.text = info.Name;
			this.Class.text = info.Class.ToString();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Selected(bool selected)
	{
		if (selected)
		{
			this.button.image.color = new Color(0.95f, 0.59f, 0f);
		}
		else
		{
			this.button.image.color = new Color(0.11f, 0.33f, 0.59f);
		}
	}
}
