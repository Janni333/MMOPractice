using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    interface IEntitySyncNotify
    {
        //移除方法
        void OnEntityRm();

        //数据改变方法
        void OnEntityChange(NEntity entitydata);

        //状态改变方法
        void OnEntityEvent(EntityEvent eve, int param);
    }
    class EntityManager : Singleton<EntityManager>
    {
        Dictionary<int, Entity> allEntities = new Dictionary<int, Entity>();
        Dictionary<int, IEntitySyncNotify> allNotifiers = new Dictionary<int, IEntitySyncNotify>();

        //两个管理器字典添加管理对象方法
        public void RgisterEntitySyncNotify(int entityId, IEntitySyncNotify notifier)
        {
            this.allNotifiers[entityId] = notifier;
        }
        public void AddEntity(Entity entity)
        {
            this.allEntities[entity.entityId] = entity;
        }


        //移除方法
        public void RemoveEntity(int entityid)
        {
            if(this.allEntities.ContainsKey(entityid))
            {
                this.allEntities.Remove(entityid);
            }
            if (this.allNotifiers.ContainsKey(entityid))
            {
                allNotifiers[entityid].OnEntityRm();
                this.allNotifiers.Remove(entityid);
            }   
        }

        //清空方法
        public void Clear()
        {
            this.allNotifiers.Clear();
            this.allEntities.Clear();
        }

        //同步方法
        public void OnEntitySync(NEntitySync entitySync)
        {
            Entity syncEntity = null;
            if (this.allEntities.TryGetValue(entitySync.Id, out syncEntity))
            {
                //更新自己
                syncEntity.Nentity = entitySync.Entity;

                //通知ec
                if (this.allNotifiers.ContainsKey(entitySync.Id))
                {
                    allNotifiers[entitySync.Id].OnEntityChange(entitySync.Entity);
                    allNotifiers[entitySync.Id].OnEntityEvent(entitySync.Event, entitySync.Param);
                }
            }
        }

        public Entity GetEntity(int entityid)
        {
            Entity entity = null;
            this.allEntities.TryGetValue(entityid, out entity);
            return entity;
        }
    }
}
