using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class MonsterManager
    {
        private Map ownerMap;
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();

        public void Init(Map map)
        {
            this.ownerMap = map;
        }

        internal Monster Create(int SpawnID, int SpawnLevel, NVector3 Position, NVector3 Direction)
        {
            Monster newMonster = new Monster(SpawnID, SpawnLevel, Position, Direction);
            EntityManager.Instance.Add(ownerMap.ID, newMonster);
            newMonster.Info.EntityId = newMonster.EntityId;
            newMonster.Info.mapId = ownerMap.ID;
            Monsters[newMonster.ID] = newMonster;

            this.ownerMap.MonsterEnter(newMonster);

            return newMonster;
        }
    }
}
