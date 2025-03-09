using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharinfoBar : MonoBehaviour {
	public Image icon;
	public Text name;

	public Transform owner;
	public Character ownercha;

	

	// Use this for initialization
	void Start () {
		InitInfo();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void InitInfo()
	{
		this.name.text = ownercha.name;
		this.icon.sprite = Resloader.Load<Sprite>(string.Format("Avatar/{0}", ownercha.Define.Class));
	}
}
