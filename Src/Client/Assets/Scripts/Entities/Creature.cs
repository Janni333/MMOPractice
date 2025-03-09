using Assets.Scripts.Model;
using Battle;
using Common.Battle;
using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using UnityEngine;

namespace Entities
{
    public class Creature : Entity
    {
        /*
         *信息
         *  网络信息 / 配置信息
         *  姓名
         *  是否玩家
         */
        public NCharacterInfo Info;
        public CharacterDefine Define;
        public Attributes Attributes;
        public SkillManager SkillMgr;

        bool battleState = false;
        public bool BattleState
        {
            get { return this.battleState; }
            set
            {
                if (this.battleState != value)
                {
                    battleState = value;
                    this.SetStandby(value);
                }
            }
        }

        public Skill CastingSkill = null;

        public int Id
        {
            get { return this.Info.Id; }
        }


        public string name
        {
            get 
            {
                return this.Info.Name;
            }
        }
        public bool isPlayer
        {
            get
            {
                /*白痴写法
                if (Ncharacterinfo.Id == Model.User.Instance.currentCharacter.Id)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                */
                return (this.Info.Type == CharacterType.Player);
            }
        }

        public bool isCurPlayer
        {
            get 
            {
                if (!isPlayer) return false;
                return this.Info.Id == User.Instance.currentCharacterInfo.Id;
            }
        }



        //构造
        public Creature(NCharacterInfo NInfo) : base(NInfo.Entity)
        {
            this.Info = NInfo;
            this.Define = DataManager.Instance.Characters[NInfo.ConfigId];

            // 初始化属性
            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attrDynamic);
            // 初始化技能
            this.SkillMgr = new SkillManager(this);
        }

        // 怪物没有装备
        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        //移动方法
        //  向前/后 停止 设置方向 设置位置
        public void MoveForward()
        {
            //Debug.LogFormat("{0}向前移动", this.name);
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            //Debug.LogFormat("{0}向前移动", this.name);
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            //Debug.LogFormat("{0}停止移动", this.name);
            this.speed = 0;
        }

        public void SetPosition(Vector3Int pos)
        {
            //Debug.LogFormat("更新位置：{0}", pos);
            this.position = pos;
        }

        public void SetDirection(Vector3Int dir)
        {
            //Debug.LogFormat("更新方向：{0}", dir);
            this.direction = dir;
        }

        // Skill
        public void CastSkill(int skillId, Creature target, NVector3 position)
        {
            this.SetStandby(true);
            Skill skill = this.SkillMgr.GetSkill(skillId);
            skill.BeginCast();
        }

        public void PlayAnim(string name)
        {
            if(this.controller != null)
                this.controller.PlayAnim(name);
        }

        public void SetStandby(bool standby)
        {
            if(this.controller != null)
                this.controller.SetStandby(standby);
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);
            this.SkillMgr.OnUpdate(delta);
        }
    }
}
