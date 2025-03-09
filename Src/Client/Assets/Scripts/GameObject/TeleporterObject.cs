using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : MonoBehaviour {

	public int teleporterId;
	Mesh mesh = null;

	// Use this for initialization
	void Start () {
		this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
		//
		Debug.Log("只是说明真的触发了。。。");

		PlayerInputController pc = other.gameObject.GetComponent<PlayerInputController>();
		if (pc != null && pc.isActiveAndEnabled)
		{
			//校验
			//	进入的传送点是否存在
			if (!DataManager.Instance.Teleporters.ContainsKey(this.teleporterId))
			{
				Debug.LogFormat("角色进入传送点不存在: Character：{0} Teleporter：{1}", pc.curCharacter.name, this.teleporterId);
				return;
			}

			Debug.LogFormat("角色进入传送点: Character：{0} Teleporter：{1}", pc.curCharacter.name, this.teleporterId);

			//	传送的target是否存在
			TeleporterDefine def = DataManager.Instance.Teleporters[this.teleporterId];
			if (def.LinkTo > 0 && DataManager.Instance.Teleporters.ContainsKey(def.LinkTo))
			{
				MapService.Instance.SendTeleport(this.teleporterId);
			}
			else
			{
				Debug.LogFormat("角色进入传送点目标错误或不存在: Character：{0} Teleporter：{1}", pc.curCharacter.name, this.teleporterId);
			}
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if (this.mesh != null)
		{
			Gizmos.DrawWireMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y * .5f, this.transform.rotation, this.transform.localScale);
		}
		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);
	}
#endif
}
