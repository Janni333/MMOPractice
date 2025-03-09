using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 240407
 *  创建与基础逻辑
 */

/*
 * 增删改
 *  1 MapCharacer类
 *  2 CharacterEnterMap character加入mapcharacters顺序调整，防止重复发送给自己
 */
namespace GameServer.Models
{
    class Map
    {
        //Map
        internal MapDefine mapDefine;
        public int ID
        {
            get
            {
                return this.mapDefine.ID;
            }
        }

        /// <summary>
        /// Characters in Map
        /// </summary>
        internal class MapCharacter
        {
            internal Character Character;
            internal NetConnection<NetSession> chaConnection;
            public MapCharacter(NetConnection<NetSession> sender, Character cha)
            {
                this.Character = cha;
                this.chaConnection = sender;
            }
        }
        Dictionary<int, MapCharacter> mapCharacters = new Dictionary<int, MapCharacter>();

        /// <summary>
        /// Spawners & Monsters in Map
        /// </summary>
        SpawnManager spawnManager = new SpawnManager();
        public MonsterManager monsterManager = new MonsterManager();


        internal Map(MapDefine define)
        {
            this.mapDefine = define;
            //this.spawnManager.Init(this);
            this.monsterManager.Init(this);
        }
        public void Update()
        {
            //this.spawnManager.Update();
        }


        #region Character Enter Map
        public void CharacterEnterMap(NetConnection<NetSession> conn, Character character)
        {
            //日志
            Log.InfoFormat("Map接收角色进入地图请求：character:{0} map:{1} {2}", character.Tcharacter.Name, this.ID, this.mapDefine.Name);
            //修改2 
            character.Info.mapId = this.ID;
            this.mapCharacters[character.ID] = new MapCharacter(conn, character);

            //角色加入本地图角色管理器
            //this.mapCharacters.Add(character.Info.Id, new MapCharacter(conn, character));
            //this.mapCharacters[character.Info.Id] = new MapCharacter(conn, character);

            //响应消息
            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.ID;

            //填充消息 & 向其他角色发送角色进入地图消息
            foreach (var mapcharacter in mapCharacters.Values)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(mapcharacter.Character.Info);
                if (mapcharacter.Character != character)
                {
                    this.AddCharacterEnterMap(mapcharacter.chaConnection, character.Info);
                }
            }
            foreach (var monster in monsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(monster.Value.Info);
            }

            //向自己客户端发送角色进入消息
            Log.InfoFormat("Map发送角色进入地图响应：character:{0} map:{1} {2}", character.Tcharacter.Name, this.ID, this.mapDefine.Name);
            conn.SendResponse();
        }

        void AddCharacterEnterMap(NetConnection<NetSession> sender, NCharacterInfo character)
        {
            //日志
            Log.InfoFormat("Map向现有角色发送角色进入地图响应：Character :{0} enterCharacter:{1} map:{2} {3}", sender.Session.Character.Tcharacter.Name, character.Name,this.ID, this.mapDefine.Name);

            //消息
            if (sender.Session.Response.mapCharacterEnter == null)
            {
                sender.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                sender.Session.Response.mapCharacterEnter.mapId = this.ID;
            }
            sender.Session.Response.mapCharacterEnter.Characters.Add(character);
            sender.SendResponse();
        }
        #endregion

        #region Character Leave Map
        public void CharacterLeaveMap(Character leaveChar)
        {
            //向其他客户端广播(包括自己)
            foreach(var kv in mapCharacters) 
            {
                this.SendCharacterLeaveMap(kv.Value.chaConnection, leaveChar);
            }

            //地图角色管理器删除
            this.mapCharacters.Remove(leaveChar.ID);
        }

        public void SendCharacterLeaveMap(NetConnection<NetSession> chaConnection, Character leaveChar)
        {
            //Log
            Log.InfoFormat("Map向现有角色发送角色离开地图响应：Character :{0} leaveCharacter:{1} map:{2} {3}", chaConnection.Session.Character.Info.Name, leaveChar.Info.Name, this.ID, this.mapDefine.Name);

            chaConnection.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            chaConnection.Session.Response.mapCharacterLeave.entityId = leaveChar.EntityId;

            chaConnection.SendResponse();
        }
        #endregion

        #region Map Entity Sync
        public void UpdateEntity(NEntitySync entitySync)
        {
            /*
             * MapCharacter syncCharacter = null;
            if (this.mapCharacters.TryGetValue(entitySync.Id, out syncCharacter))
            {
                //更新
                syncCharacter.Character.Nentity = entitySync.Entity;
                //广播
                foreach (var mapCharacter in this.mapCharacters.Values)
                {
                    if (mapCharacter.Character.ID != entitySync.Id)
                    {
                        MapService.Instance.SendMapEnitySync(mapCharacter.chaConnection, entitySync);
                    }
                }
            }
             */
            foreach (var kv in this.mapCharacters)
            {
                if (kv.Value.Character.EntityId == entitySync.Id)
                {
                    kv.Value.Character.Position = entitySync.Entity.Position;
                    kv.Value.Character.Direction = entitySync.Entity.Direction;
                    kv.Value.Character.Speed = entitySync.Entity.Speed;

                    if (entitySync.Event == EntityEvent.Ride)
                    {
                        kv.Value.Character.Ride = entitySync.Param;
                    }
                }
                else
                {
                    MapService.Instance.SendMapEnitySync(kv.Value.chaConnection, entitySync);
                }
            }
        }
        #endregion

        #region Monster
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map:{0} monsterId:{1}", this.mapDefine.ID, monster.ID);

            foreach (var character in mapCharacters)
            {
                this.AddCharacterEnterMap(character.Value.chaConnection, monster.Info);
            }
        }

        internal void BroadCastBattleResponse(NetMessageResponse response)
        {
            foreach(var kv in this.mapCharacters)
            {
                kv.Value.chaConnection.Session.Response.skillCast = response.skillCast;
                kv.Value.chaConnection.SendResponse();
            }
        }

        #endregion
    }
}
