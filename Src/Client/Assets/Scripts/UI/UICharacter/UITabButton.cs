using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabButton : MonoBehaviour {
	public UITabView tabView;
	public int Index;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Selected(bool selected)
	{
		Button thisButton = this.GetComponent<Button>();
		if (selected)
		{
			thisButton.image.color = new Color(0.84f, 0.49f, 0f);
			thisButton.GetComponentInChildren<Text>().color = new Color(1.0f, 1.0f, 1.0f);
		}
		else
		{
			thisButton.image.color = new Color(1.0f, 1.0f, 1.0f);
			thisButton.GetComponentInChildren<Text>().color = new Color(0f, 0f, 0f);
		}
	}

	public void OnClickButton()
	{
		this.tabView.selectTab(this.Index);
	}
}
