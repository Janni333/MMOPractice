using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 240331
 *	加载页面
 *	进度条数值变化
 *	
 *240404
 *	加载Data
 *	初始化UserService
 */

public class LoadingManager : MonoBehaviour {
	public Text Tips;
	public Text My;
	public Text MMO;
	public GameObject LoadingPanel;
	public GameObject LoginPanel;
	public GameObject RegisterPanel;
	public Slider loadingBar;

	// Use this for initialization
	void Start () {
		Tips.gameObject.SetActive(false);
		My.gameObject.SetActive(false);
		MMO.gameObject.SetActive(false);
		LoadingPanel.SetActive(false);
		LoginPanel.SetActive(false);
		RegisterPanel.SetActive(false);

		StartCoroutine(LoadingProcess());
	}

	IEnumerator LoadingProcess()
	{
		//修改1：增加UnityLogger初始化
		log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
		UnityLogger.Init();
		Common.Log.Init("Unity");
		Common.Log.Info("LoadingManager start");

		My.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);

		MMO.gameObject.SetActive(true);
		yield return new WaitForSeconds(3f);

		My.gameObject.SetActive(false);
		MMO.gameObject.SetActive(false);
		Tips.gameObject.SetActive(true);
		yield return new WaitForSeconds(3f);

		Tips.gameObject.SetActive(false);
		LoadingPanel.SetActive(true);

		//初始化服务及管理
		yield return DataManager.Instance.LoadData();
		UserService.Instance.Init();
		MapService.Instance.Init();
		ItemService.Instance.Init();
		StatusService.Instance.Init();
		QuestService.Instance.Init();
		FriendService.Instance.Init();
		TeamService.Instance.Init();
		GuildService.Instance.Init();
		//ChatService.Instance.Init();
		//TestManager.Instance.Init();
		ShopManager.Instance.Init();

		SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);

		//进度条效果
		while (loadingBar.value < 100)
		{ 
			float i = Random.Range(0.1f, 2f);
			loadingBar.value += i;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(1.5f);

		LoadingPanel.SetActive(false);
		LoginPanel.SetActive(true);
		//修改2：增加yield return null表明协程结束
		yield return null;

	}
	
	// Update is called once per frame
	void Update () {
	}
}
