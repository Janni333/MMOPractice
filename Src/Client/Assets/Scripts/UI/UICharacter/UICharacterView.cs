using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour {
	public List<GameObject> Chars;
	private int index;
	public int Index
	{
		get 
		{
			return index;
		}
		set
		{
			index = value;
			for (int i = 0; i < Chars.Count; i++)
			{
				Chars[i].SetActive(i == value);
			}
		}
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {	
	}
}
