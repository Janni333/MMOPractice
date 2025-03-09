using Common.Data;
using Entities;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Character : Creature
    {
        public Character(NCharacterInfo NInfo) : base(NInfo)
        {
        }

        public override List<EquipDefine> GetEquips()
        {
            return EquipManager.Instance.GetEquipedDefines();
        }
    }
}
