using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour {
	//小地图组件
	public Image mapImage;
	public Text mapName;
	public Image arrow;
	public Collider boundingbox;

	public Transform playerTransform;

	// Use this for initialization
	void Start () {
		MinimapManager.Instance.minimap = this;
		this.UpdateMapInfo();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMap();
	}

	public void UpdateMapInfo()
	{
		this.mapName.text = MinimapManager.Instance.miniDefine.Name;
		this.mapImage.sprite = MinimapManager.Instance.LoadSprite();
		this.boundingbox = MinimapManager.Instance.mapboundingBox;

		//增
		this.mapImage.SetNativeSize();
		this.mapImage.transform.localPosition = Vector3.zero;

		this.playerTransform = null;
	}

	public void UpdateMap()
	{
		if (this.playerTransform == null)
		{
			this.playerTransform = MinimapManager.Instance.PlayerTransform;
		}

		if (this.boundingbox == null || this.playerTransform == null) return;

		float realx = this.boundingbox.bounds.size.x;
		float realz = this.boundingbox.bounds.size.z;

		float movex = this.playerTransform.position.x - this.boundingbox.bounds.min.x;
		float movez = this.playerTransform.position.z - this.boundingbox.bounds.min.z;

		float posx = movex / realx;
		float posy = movez / realz;

		this.mapImage.rectTransform.pivot = new Vector2(posx, posy);
		this.mapImage.rectTransform.localPosition = Vector2.zero;
		this.arrow.transform.eulerAngles = new Vector3(0, 0, - this.playerTransform.eulerAngles.y); 
	}
}
