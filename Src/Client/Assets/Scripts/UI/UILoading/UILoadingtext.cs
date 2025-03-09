using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingtext : MonoBehaviour {
	public Slider loadingSlider;
	Text loadingText;
	// Use this for initialization
	void Start () {
		loadingText = this.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		loadingText.text = loadingSlider.value.ToString();
	}
}
