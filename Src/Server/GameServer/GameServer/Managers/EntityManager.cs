using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EntityManager : Singleton<EntityManager>
    {
        private int index = 0;

        public List<Entity> allEntities = new List<Entity>();
        public Dictionary<int, List<Entity>> mapEntities = new Dictionary<int, List<Entity>>();

        public void Add(int mapId, Entity newEn)
        {
            allEntities.Add(newEn);
            newEn.Nentity.Id = ++this.index;

            List<Entity> entities = null;
            if (!mapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>();
                mapEntities[mapId] = entities;
            }
            entities.Add(newEn);
        }

        public void Remove(int mapId, Entity rmEn)
        {
            this.allEntities.Remove(rmEn);
            this.mapEntities[mapId].Remove(rmEn);
        }

    }
}
