using Common.Data;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Creature : Entity
    {
        #region Property & Fields
        public int ID { get; set; }
        public string Name { get { return this.Info.Name; } }
        public NCharacterInfo Info;
        public CharacterDefine Define;

        public SkillManager SkillMgr;
        #endregion


        #region Function
        // 角色使用
        /*
         public Creature(Vector3Int pos, Vector3Int dir) : base(pos, dir)
        {
        }
         */


        // 怪物使用 ——> 角色怪物共用
        public Creature(CharacterType type, int Configid, int level, Vector3Int pos, Vector3Int dir) : base(pos, dir)
        {
            this.Define = DataManager.Instance.Characters[Configid];
            this.Info = new NCharacterInfo();
            
            this.Info.Type = type;
            this.Info.Level = level;
            this.Info.Entity = this.Nentity;
            this.Info.ConfigId = Configid;
            this.Info.EntityId = this.EntityId;
            this.Info.Name = this.Define.Name;
            this.InitSkills();
        }

        private void InitSkills()
        {
            SkillMgr = new SkillManager(this);
            this.Info.Skills.AddRange(this.SkillMgr.Infos);
        }
        #endregion
    }
}
