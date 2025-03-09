using Common;
using Common.Data;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class Spawner
    {
        public SpawnRuleDefine def { get; set; }
        private SpawnPointDefine pointDef = null;
        private Map ownerMap;

        private float spawnTime = 0;
        private float unspawnTime = 0;
        private bool spawned = false;

        public Spawner(SpawnRuleDefine define, Map map)
        {
            this.def = define;
            this.ownerMap = map;
            if (DataManager.Instance.SpawnPoints.ContainsKey(ownerMap.ID))
            {
                if (DataManager.Instance.SpawnPoints[ownerMap.ID].ContainsKey(def.SpawnPoint))
                {
                    this.pointDef = DataManager.Instance.SpawnPoints[ownerMap.ID][def.SpawnPoint];
                }
                else
                {
                    Log.InfoFormat("SpawnRule{0}SpawnPoint{1}不存在", def.ID, def.SpawnPoint);
                }
            }
        }

        public void Update()
        {
            if (canSpawn())
            {
                DoSpawn();
            }
        }

        public bool canSpawn()
        {
            if (this.spawned)
                return false;
            if (unspawnTime + def.SpawnPeriod > Time.time)
                return false;
            return true;
        }

        private void DoSpawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map{0} Spawn{1}: Mon{2} Level{3} Point{4}", ownerMap.ID, def.ID, def.SpawnMonID, def.SpawnLevel, pointDef.ID);
            this.ownerMap.monsterManager.Create(def.SpawnMonID, def.SpawnLevel, pointDef.Position, pointDef.Direction);
        }
    }
}
