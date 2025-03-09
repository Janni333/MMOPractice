using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Services;
using GameServer.Managers;
using Battle;

namespace GameServer
{
    /*
     * 240326
     *  初始化网络服务
     *  启动网络服务
     *  停止网络服务
     *  初始化测试服务
     *  
     *240401
     *  初始化UserService
     */
    class GameServer
    {
        NetService net = new NetService();
        Thread thread;
        bool running = false;
        public bool Init()
        {
            net.Init(8089);

            DataManager.Instance.Load();

            DBService.Instance.Init();
            TestService.Instance.Init();
            UserService.Instance.Init();
            MapService.Instance.Init();
            ItemService.Instance.Init();
            QuestService.Instance.Init();
            FriendService.Instance.Init();
            TeamService.Instance.Init();
            GuildService.Instance.Init();
            BattleService.Instance.Init();
            // ChatService.Instance.Init();

            thread = new Thread(new ThreadStart(this.Update));
            return true;
        }

        public void Start()
        {
            net.Start();

            running = true;
            thread.Start();
        }


        public void Stop()
        {
            net.Stop();
            running = false;
            thread.Join();
        }

        public void Update()
        {
            var mapmanager = MapManager.Instance;
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
                mapmanager.Update();
            }
        }
    }
}
