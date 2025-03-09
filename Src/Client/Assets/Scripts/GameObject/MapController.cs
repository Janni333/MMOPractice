using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

	public Collider MapBoundingBox;
	// Use this for initialization
	void Start () {
		MinimapManager.Instance.UpdataMinimap(MapBoundingBox);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
