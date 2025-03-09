using GameServer.Core;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Entity
    {
        #region  Property & Field
        private NEntity nentity;
        public NEntity Nentity
        {
            get
            {
                return nentity;
            }
            set 
            {
                this.nentity = value;
                this.SetEntityData(value);
            }
        }

        public int EntityId
        {
            get 
            {
                return this.nentity.Id;
            }
        }

        private Vector3Int position;
        public Vector3Int Position
        {
            get 
            {
                return this.position;
            }
            set
            {
                this.position = value;
                this.nentity.Position = value;
            }
        }

        private Vector3Int direction;
        public Vector3Int Direction
        {
            get 
            {
                return this.direction;
            }
            set
            {
                this.direction = value;
                this.nentity.Direction = value;
            }
        }

        private int speed;
        public int Speed
        {
            get
            {
                return this.speed;
            }
            set 
            {
                this.speed = value;
                this.nentity.Speed = value;
            }
        }
        #endregion

        #region Function
        public Entity(NEntity NData)
        {
            this.Nentity = NData;
        }

        public Entity(Vector3Int pos, Vector3Int dir)
        {
            this.nentity = new NEntity();
            this.nentity.Position = pos;
            this.nentity.Direction = dir;
            this.SetEntityData(nentity);
        }
        private void SetEntityData(NEntity NData)
        {
            this.Position = NData.Position;
            this.Direction = NData.Direction;
            this.Speed = NData.Speed;
        }
        #endregion
    }
}
