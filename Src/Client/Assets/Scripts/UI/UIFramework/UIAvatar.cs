using Entities;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAvatar : MonoBehaviour {
	public Image avatarImage;
	public Text avatarName;
	public Text avatarLevel;
	public NCharacterInfo curChar;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateInfo()
	{
		if (curChar != null)
		{
			this.avatarName.text = curChar.Name;
			this.avatarLevel.text = string.Format("LV : {0}", curChar.Level);
			this.avatarImage.sprite = Resloader.Load<Sprite>(string.Format("Avatar/{0}", curChar.Class));
		}
	}
}
