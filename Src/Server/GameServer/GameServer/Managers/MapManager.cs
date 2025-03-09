using Common;
using Common.Data;
using GameServer.Models;
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
 *  1 Init：管理器字典key应为mapid
 */
namespace GameServer.Managers
{
    class MapManager: Singleton<MapManager>
    {
        //管理器字典
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();

        //索引器
        public Map this[int index]
        {
            get
            {
                return this.Maps[index];
            }
            set
            {
                this.Maps[index] = value;
            }
        }

        //初始化
        public void Init()
        {
            //根据MapDefine字典创建Map并设置管理器字典
            //修改1
            /*
            foreach(var kv in DataManager.Instance.Maps) 
            {
                Maps[kv.Key] = new Map(kv.Value);
            }
            */
            foreach (MapDefine mapdefine in DataManager.Instance.Maps.Values)
            {
                //日志
                Log.InfoFormat("初始化地图：ID{0} Name{1}", mapdefine.ID, mapdefine.Name);
                //字典添加新地图
                Maps[mapdefine.ID] = new Map(mapdefine);
            }
        }

        //更新地图
        public void Update()
        {
            //调用管理器字典中每个map
            foreach (var map in Maps.Values)
            {
                map.Update();
            }
        }
    }
}
