using Common;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    /*
     * 240326
     *  订阅测试消息
     *  打印接收消息日志
     */
    class TestService : Singleton<TestService>
    {
        public void Init()
        { }

        public void Start()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FirstTestRequest>(this.OnFirstTest);
        }

        private void OnFirstTest(NetConnection<NetSession> sender, FirstTestRequest request)
        {
            Log.InfoFormat("FirstTest Content :{0}", request.Content);
        }
    }
}
