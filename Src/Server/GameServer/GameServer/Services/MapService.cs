using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService: Singleton<MapService>
    {
        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }

        #region Map Entity Sync
        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            //Log
            Log.InfoFormat("MapService接收Entity同步请求：Entity：{0} Event：{1}", request.entitySync.Id, request.entitySync.Event);

            MapManager.Instance[sender.Session.Character.Info.mapId].UpdateEntity(request.entitySync);
        }

        public void SendMapEnitySync(NetConnection<NetSession> sender, NEntitySync entitySync)
        {
            //Log
            Log.InfoFormat("MapService发送Entity同步响应：Entity：{0} Event：{1}", entitySync.Id, entitySync.Event);

            //Message
            sender.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            sender.Session.Response.mapEntitySync.entitySyncs.Add(entitySync);

            sender.SendResponse();
        }
        #endregion

        #region Map Teleport
        void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            //data
            Character teleportChar = sender.Session.Character;
            int teleId = request.teleporterId;
            Log.InfoFormat("MapService接收地图传送请求: Character：{0} Teleporter：{1}", teleportChar.Info.Name, teleId);

            //校验
            if (!DataManager.Instance.Teleporters.ContainsKey(teleId))
            {
                Log.WarningFormat("角色进入传送点不存在");
                return;
            }

            TeleporterDefine def = DataManager.Instance.Teleporters[teleId];

            if (def.LinkTo == 0 || !DataManager.Instance.Teleporters.ContainsKey(def.LinkTo))
            {
                Log.WarningFormat("角色传送目标不存在");
            }

            //发起传送 离开地图进入地图
            TeleporterDefine teledef = DataManager.Instance.Teleporters[def.LinkTo];
            MapManager.Instance[def.MapID].CharacterLeaveMap(teleportChar);
            teleportChar.Position = teledef.Position;
            teleportChar.Direction = teledef.Direction;
            MapManager.Instance[teledef.MapID].CharacterEnterMap(sender, teleportChar);
        }
        #endregion
    }
}
