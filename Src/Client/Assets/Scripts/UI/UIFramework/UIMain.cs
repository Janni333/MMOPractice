using Assets.Scripts.Model;
using Managers;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoSingleton<UIMain> {
	public UIAvatar uiavartar;

	public UITeam uiteam;

	// Use this for initialization
	protected override void OnStart () {
		UpdateAvatar();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void UpdateAvatar()
	{
		this.uiavartar.curChar = User.Instance.currentCharacterInfo;
		this.uiavartar.UpdateInfo();
	}

    public void OnClickBag()
	{
		UIManager.Instance.Show<UIBag>();
	}

	public void OnClickEquip()
	{
		UIManager.Instance.Show<UICharEquip>();
	}

	public void OnClickQuest()
	{
		UIManager.Instance.Show<UIQuest>();
	}

	public void OnClickFriend()
	{
		UIManager.Instance.Show<UIFriend>();
	}

    public void OnClickGuild()
    {
		GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {
		UIManager.Instance.Show<UIRide>();
    }

    public void OnClickSetting()
    {
		UIManager.Instance.Show<UISetting>();	
    }

    public void OnClickSkill()
    {
		UIManager.Instance.Show<UISkill>();
    }

    public void ShowTeamUI(bool show)
	{
		uiteam.ShowTeam(show);
	}
}
