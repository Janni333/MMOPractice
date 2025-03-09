using Network;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*240326
*	初始化NetClient
*	测试NetClient链接
*	
*	发送测试消息
*/

public class NetTest : MonoBehaviour {
	void Start () {
		NetClient.Instance.Init("127.0.0.1", 8089);
		NetClient.Instance.Connect();

		NetMessage message = new NetMessage();
		message.Request = new NetMessageRequest();
		message.Request.fistTest = new FirstTestRequest();
		message.Request.fistTest.Content = "Fighting CFJ!";

		NetClient.Instance.SendMessage(message);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
