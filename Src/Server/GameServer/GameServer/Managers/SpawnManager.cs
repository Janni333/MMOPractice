using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class SpawnManager
    {
        private List<Spawner> spawners = new List<Spawner>();

        private Map ownerMap;

        public void Init(Map map)
        {
            this.ownerMap = map;

            if (DataManager.Instance.SpawnRules.ContainsKey(map.ID))
            {
                foreach (var sp in DataManager.Instance.SpawnRules[map.ID].Values)
                {
                    spawners.Add(new Spawner(sp, ownerMap));
                }
            }
        }

        public void Update()
        {
            if (this.spawners.Count == 0)
            {
                return;
            }
            foreach (var sp in spawners)
            {
                sp.Update();
            }
        }
    }
}
