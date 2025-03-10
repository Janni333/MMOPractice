﻿using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    internal class SkillManager
    {
        private Creature Owner;
        public List<Skill> Skills { get; private set; }
        public List<NSkillInfo> Infos { get; private set; }

        public SkillManager(Creature owner)
        {
            this.Owner = owner;
            this.Skills = new List<Skill>();
            this.Infos = new List<NSkillInfo>();
            this.InitSkills();
        }

        void InitSkills()
        {
            this.Skills.Clear();
            this.Infos.Clear();

            // 扩展：从数据库读取角色技能信息
            if (!DataManager.Instance.Skills.ContainsKey(this.Owner.Define.TID))
                return;

            // 利于前期技能测试
            foreach (var define in DataManager.Instance.Skills[this.Owner.Define.TID])
            {
                NSkillInfo info = new NSkillInfo();
                info.Id = define.Key;
                if (this.Owner.Info.Level >= define.Value.UnlockLevel)
                {
                    info.Level = 5;
                }
                else
                {
                    info.Level = 1;
                }
                this.Infos.Add(info);
                Skill skill = new Skill(info, this.Owner);
                this.AddSkill(skill);
            }
        }

        private void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }
    }
}
