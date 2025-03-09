using Assets.Scripts.Model;
using Common;
using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/*
 * 240331-0401
 *  注册逻辑
 *  登录逻辑
 *  
 * 240405
 *  创建逻辑
 *  
 * 240407
 *  游戏进入逻辑
 *  地图进入逻辑
 */

/*
 * 增删改
 *  1 网络连接: ConnectToGameServer / OnGameServerConnect / OnGameServerDisconnect / DisconnectNotify
 *  2 登录和注册网络校验
 *  3 UnityAction
 *  4 User.Instance.UserInfo.Player.Characters只读
 */
namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        //修改1
        NetMessage pendingMessage = null;
        bool connected = false;
        bool isQuitGame = false;

        //修改3
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnCreateCharacter;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;

            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnCharacterCreate);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapEnter);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnCharacterCreate);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnGameLeave);
            //MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapEnter);

            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }
        public void Init()
        { }

        //修改1：网络连接
        #region Connected
        public void ConnectToGameServer()
        {
            //日志
            Debug.Log("连接到服务器");

            //初始化NetClient，调用连接
            NetClient.Instance.Init("127.0.0.1", 8089);
            NetClient.Instance.Connect();
        }

        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("服务器连接:{0} reason:{1}", result, reason);

            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if (this.pendingMessage != null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                //当pendingmessage为空时
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！ \n Result:{0} Error:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }
        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result, string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userLogin != null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开! \n Result:{0} Error:{1}", result, reason));
                    }
                }
                else if (this.pendingMessage.Request.userRegister != null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开! \n Result:{0} Error:{1}", result, reason));
                    }
                }
                else
                {
                    if (this.OnCreateCharacter != null)
                    {
                        this.OnCreateCharacter(Result.Failed, string.Format("服务器断开! \n Result:{0} Error:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Register
        public void SendRegister(string username, string password)
        {
            //日志
            Debug.LogFormat("发送注册请求：User:{0}  Password:{1}", username, password);

            //消息
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = username;
            message.Request.userRegister.Passward = password;

            //校验和发送
            //修改2：
            if (this.connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                pendingMessage = message;
                this.ConnectToGameServer();
            }
        }

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            //日志
            Debug.LogFormat("接收注册响应: Result:{0} Msg:{1}", response.Result, response.Errormsg);

            //引发事件
            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }
        }
        #endregion

        #region Log In
        public void SendLogin(string username, string password)
        {
            //日志
            Debug.LogFormat("发送登录请求：username:{0} password:{1}", username, password);

            //准备消息
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = username;
            message.Request.userLogin.Passward = password;

            //检查链接
            //  发送消息
            //  连接
            if (this.connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                pendingMessage = message;
                this.ConnectToGameServer();
            }
        }

        void OnUserLogin(object sender, UserLoginResponse response)
        {
            //日志
            Debug.LogFormat("接收登录响应: Result:{0} Msg:{1}", response.Result, response.Errormsg);

            //登录成功
            //  填充本地映射
            if (response.Result == Result.Success)
            {
                User.Instance.SetInfo(response.Userinfo);
            }
            //回调UI
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
            }
        }
        #endregion

        #region Create Character
        public void SendCreateCharacter(string chaname, CharacterClass chaclass)
        {
            //日志
            Debug.LogFormat("发送角色创建请求：charname:{0} charclass:{1}", chaname, chaclass);

            //消息
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Class = chaclass;
            message.Request.createChar.Name = chaname;

            //校验和发送
            if (this.connected && NetClient.Instance.Connected)
            {
                pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else 
            {
                pendingMessage = message;
                this.ConnectToGameServer();
            }

        }

        public void OnCharacterCreate(object sender, UserCreateCharacterResponse response)
        {
            //日志
            Debug.LogFormat("接收角色创建响应：Result:{0} Msg:{1}", response.Result, response.Errormsg);

            //本地角色更新
            //修改4 填充User.Instance.UserInfo.Player.Characters的操作 & 条件判断
            if (response.Result == Result.Success)
            {
                User.Instance.UserInfo.Player.Characters.Clear();
                User.Instance.UserInfo.Player.Characters.AddRange(response.Characters);
            }
      
            //回调UI
            if (this.OnCreateCharacter != null)
            {
                this.OnCreateCharacter(response.Result, response.Errormsg);
            }
        }
        #endregion

        #region Enter Game
        public void SendGameEnter(int idx)
        {
            Debug.LogFormat("发送进入游戏请求：Character:{0}", User.Instance.UserInfo.Player.Characters[idx].Name);

            // 进入游戏时初始化
            ChatManager.Instance.Init();

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = idx;

            NetClient.Instance.SendMessage(message);
        }

        private void OnGameEnter(object sender, UserGameEnterResponse response)
        {
            //日志
            Debug.LogFormat("接收进入游戏响应：Result:{0} Msg:{1}", response.Result, response.Errormsg);

            //进入成功
            if (response.Result == Result.Success)
            {
                if (response.Character != null)
                {
                    User.Instance.currentCharacterInfo = response.Character;
                    ItemManager.Instance.Init(response.Character.Items);
                    BagManager.Instance.Init(response.Character.Bag);
                    QuestManager.Instance.Init(response.Character.Quests);
                    FriendManager.Instance.Init(response.Character.Friends);
                    GuildManager.Instance.Init(response.Character.Guild);
                }
            }

        }
        #endregion

        #region Enter Map(弃用)
        /*
        private void OnMapEnter(object sender, MapCharacterEnterResponse request)
        {
            //日志
            Debug.LogFormat("接收角色进入地图响应: character:{0} mapid:{1} map:{2}", request.Characters[0], request.mapId, DataManager.Instance.Maps[request.mapId].Name);

            //填充本地
            NCharacterInfo info = request.Characters[0];
            User.Instance.currentCharacter = info;

            //加载新场景
            SceneManager.Instance.LoadScene(DataManager.Instance.Maps[request.mapId].Resource);
        }
        */
        #endregion

        #region Leave Game
        public void SendGameLeave(bool isQuitGame = false)
        {
            //Log
            Debug.LogFormat("发送角色离开游戏请求：用户：{0} 角色：{1}", User.Instance.UserInfo.Id, User.Instance.currentCharacterInfo.Name);

            this.isQuitGame = isQuitGame;
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        private void OnGameLeave(object sender, UserGameLeaveResponse response)
        {
            Debug.LogFormat("接收角色离开游戏响应：Result：{0} Msg:{1}", response.Result, response.Errormsg);

            MapService.Instance.CurrentMap = 0;
            User.Instance.currentCharacterInfo = null;
            if (this.isQuitGame)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
#endregion

    }
}
