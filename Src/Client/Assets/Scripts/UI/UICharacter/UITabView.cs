using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabView : MonoBehaviour {

	public List<UITabButton> buttons;
	public UICharacterView chars;

	// Use this for initialization
	void Start () {
		StartCoroutine(initTabview());
	}

    IEnumerator initTabview()
    {
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].tabView = this;
			buttons[i].Index = i;
		}
		yield return new WaitForEndOfFrame();
		selectTab(0);
		yield return null;
    }

    // Update is called once per frame
    void Update () {
		
	}

	public void selectTab(int index)
	{
		foreach (var button in buttons)
		{
			button.Selected(button.Index == index);
		}
		this.chars.Index = index;
	}
}
