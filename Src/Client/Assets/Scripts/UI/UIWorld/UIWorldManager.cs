using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldManager : MonoSingleton<UIWorldManager> 
{
	public GameObject CharinfoList;
	public GameObject charinfoPrefab;

	public GameObject NpcInfoList;
	public GameObject NpcInfoPrefab;

	Dictionary<Transform, GameObject> charinfoelements = new Dictionary<Transform, GameObject>();
	Dictionary<Transform, GameObject> npcinfoelements = new Dictionary<Transform, GameObject>();

	// Update is called once per frame
	void Update () {
	}

	#region CharinfoBar
	public void Addcharinfobar(Transform owner, Character cha)
	{
		GameObject charbar = Instantiate(charinfoPrefab, CharinfoList.transform);
		charbar.name ="CharinfoBar" + cha.name + cha.entityId;

		UICharinfoBar charbarinfo = charbar.GetComponent<UICharinfoBar>();
		charbarinfo.owner = owner;
		charbarinfo.ownercha = cha;

		UIWorldElement charele = charbar.GetComponent<UIWorldElement>();
		charele.owner = owner;

		charbar.gameObject.SetActive(true);

		charinfoelements[owner] = charbar;
	}

	public void Rmcharinfobar(Transform owner)
	{
		if (this.charinfoelements.ContainsKey(owner))
		{
			Destroy(this.charinfoelements[owner]) ;
			this.charinfoelements.Remove(owner);
		}
	}

	public UINPCInfoBar AddNpcinfobar(Transform owner)
	{
		if (this.npcinfoelements.ContainsKey(owner))
		{
			return this.npcinfoelements[owner].GetComponent<UINPCInfoBar>();
		}
		else
		{
			GameObject npcbar = Instantiate(NpcInfoPrefab, NpcInfoList.transform);
			UINPCInfoBar uibar = npcbar.GetComponent<UINPCInfoBar>();
			UIWorldElement uiele = npcbar.GetComponent<UIWorldElement>();
			uiele.owner = owner;
			this.npcinfoelements[owner] = npcbar;

			npcbar.SetActive(true);
			return uibar;
		}
	}

	public void RmNpcinfobar(Transform owner)
	{
	}
	#endregion
}
