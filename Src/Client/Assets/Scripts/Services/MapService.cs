using Entities;
using Managers;
using Assets.Scripts.Model;
using Common.Data;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/*
 * 240409
 *  OnCharacterEnter逻辑
 *  EnterMap逻辑
 */

/*
 * 修改
 *  1 加载场景时需要校验地图是否存在
 *  2 可以利用响应中的角色列表更新当前本地角色
 */

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        /*
         *信息
         *  当前地图
         */

        public int CurrentMap;

        //构造
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        //析构
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }
        //初始化
        public void Init()
        { 
        }


        #region Character Enter Map
        private void OnCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            //日志
            Debug.LogFormat("MapService接收角色进入地图响应：Character:{0} Map:{1}{2}", response.Characters[0], response.mapId, DataManager.Instance.Maps[response.mapId]);

            //将响应中角色列表拉入客户端角色管理器
            this.AddCharacter(response.Characters);

            //加载场景（或不加载）
            this.LoadMap(response.mapId);

        }

        private void AddCharacter(List<NCharacterInfo> chars)
        {
            foreach (var cha in chars)
            {
                //2 刷新本地角色
                if (User.Instance.currentCharacterInfo == null || (cha.Type == CharacterType.Player && User.Instance.currentCharacterInfo.Id == cha.Id))
                {
                    User.Instance.currentCharacterInfo = cha;
                }
                Character newcha = CharacterManager.Instance.Add(cha);
            }
        }

        private void LoadMap(int mapId)
        {
            //校验并切换场景
            if (CurrentMap != mapId)
            {
                //修改
                if (DataManager.Instance.Maps.ContainsKey(mapId))
                {
                    MapDefine map = DataManager.Instance.Maps[mapId];
                    User.Instance.currentMapdefine = map;
                    SceneManager.Instance.LoadScene(map.Resource);
                    SoundManager.Instance.PlayMusic(map.Music);
                    CurrentMap = mapId;
                    Debug.LogFormat("角色进入新地图{0}", DataManager.Instance.Maps[mapId].Name);
                }
            }
            else
            {
                Debug.LogFormat("角色进入当前地图");
            }
        }
        #endregion

        #region Character Leave Map
        private void OnCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("MapService接收角色离开地图响应：Character:{0} Map:{1}{2}", response.entityId, this.CurrentMap, DataManager.Instance.Maps[this.CurrentMap]);

            if (response.entityId != User.Instance.currentCharacterInfo.EntityId)
            {
                CharacterManager.Instance.Remove(response.entityId);
                EntityManager.Instance.RemoveEntity(response.entityId);
            }
            else 
            {
                CharacterManager.Instance.Clear();
                EntityManager.Instance.Clear();
            }
        }
        #endregion

        #region Map Entity Sync
        public void SendEntitySync(int entityid, Entity entityData, EntityEvent entityEvent, int param)
        {
            //Log
            Debug.LogFormat("MapService发送Entity同步请求：Entity：{0} EntityEvent：{1}", entityid, entityEvent);

            //Message
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entityid,
                Entity = entityData.Nentity,
                Event = entityEvent,
                Param = param
            };

            NetClient.Instance.SendMessage(message);
        }

        private void OnMapEntitySync(System.Object sender, MapEntitySyncResponse response)
        {
            /*
             * 原写法
             Debug.LogFormat("MapService接收Entity同步响应：Entity：{0} Event：{1}", response.entitySyncs[0].Id, response.entitySyncs[0].Event);
             EntityManager.Instance.UpdateEntity(response.entitySyncs[0]);
             */

            System.Text.StringBuilder sb = new StringBuilder();
            sb.AppendFormat("MapEntityUpdateResponse: Entities:{0}", response.entitySyncs.Count);
            sb.AppendLine();

            foreach (var entity in response.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);

                sb.AppendFormat("   [{0}]evt: {1} entity{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
                ;
            }
            Debug.Log(sb.ToString());


        }
        #endregion

        #region TelePorter
        public void SendTeleport(int id)
        {
            Debug.LogFormat("MapService发送地图传送请求：Map：{0} Teleporter：{1}", this.CurrentMap, id);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = id;

            NetClient.Instance.SendMessage(message);
        }
        #endregion
    }
}
