using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Entities
{
    public class Entity
    {
        /*
         *信息
         *  实体id
         *  位置/方向/速度
         *  NEntity：字段/属性
         */
        public int entityId;
        public Vector3Int position;
        public Vector3Int direction;
        public int speed;
        public IEntityController controller;
        private NEntity nentity;
        public NEntity Nentity
        {
            get 
            {
                this.SetNEntityData();
                return this.nentity;
            }
            set
            {
                this.nentity = value;
                this.SetEntityData(value);
            }
        }

        //构造
        public Entity(NEntity NData)
        {
            this.Nentity = NData;
            this.SetEntityData(NData);
        }

        //网络数据 ---> 本地实体数据
        public void SetEntityData(NEntity NData)
        {
            this.entityId = NData.Id;
            this.position = this.position.FromNVector3(NData.Position);
            this.direction = this.direction.FromNVector3(NData.Direction);
            this.speed = NData.Speed;
        }

        public void SetNEntityData()
        {
            nentity.Position.FromVector3Int(this.position);
            nentity.Direction.FromVector3Int(this.direction);
            nentity.Speed = this.speed;
        }

        //Update:基础移动
        public virtual void OnUpdate(float delta)
        {
            if (this.speed != 0)
            {
                Vector3 dir = this.direction;
                this.position += Vector3Int.RoundToInt(dir * speed * delta / 100f);
            }
        }
    }
}
