﻿using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public class Attributes
    {
        AttributeData Initial = new AttributeData();
        AttributeData Growth = new AttributeData();
        AttributeData Equip = new AttributeData();
        AttributeData Basic = new AttributeData();
        AttributeData Buff = new AttributeData();
        public AttributeData Final = new AttributeData();

        int Level;

        private NAttributeDynamic dynamic;

        public float HP
        {
            get
            {
                return dynamic.Hp;
            }
            set
            {
                dynamic.Hp = (int)Math.Min(MaxHP, value);
            }
        }

        public float MP
        {
            get
            {
                return dynamic.Mp;
            }
            set
            {
                dynamic.Mp = (int)Math.Min(MaxMP, value);
            }
        }

        public float MaxHP { get { return this.Final.MaxHP; } }
        public float MaxMP { get { return this.Final.MaxMP; } }
        public float STR { get { return this.Final.STR; } }
        public float INT { get { return this.Final.INT; } }
        public float DEX { get { return this.Final.DEX; } }
        public float AD { get { return this.Final.AD; } }
        public float AP { get { return this.Final.AP; } }
        public float DEF { get { return this.Final.DEF; } }
        public float MDEF { get { return this.Final.MDEF; } }
        public float SPD { get { return this.Final.SPD; } }
        public float CRI { get { return this.Final.CRI; } }

        public void Init(CharacterDefine define, int level, List<EquipDefine> equips, NAttributeDynamic dynamicAttr)
        {
            this.dynamic = dynamicAttr;
            this.LoadInitAttribute(this.Initial, define);
            this.LoadGrowthAttribute(this.Growth, define);
            this.LoadEquipAttribute(this.Equip, equips);

            this.Level = level;
            this.InitBasicAttributes();
            this.InitSecondaryAttributes();

            // todo 设置buff数值
            this.InitFinalAttributes();

            if(this.dynamic == null)
            {
                this.dynamic = new NAttributeDynamic();
            }
            this.HP = dynamicAttr.Hp;
            this.MP = dynamicAttr.Mp;
        }

        /// <summary>
        /// 加载初始属性
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="define"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadInitAttribute(AttributeData attr, CharacterDefine define)
        {
            attr.MaxHP = define.MaxHP;
            attr.MaxMP = define.MaxMP;

            attr.STR = define.STR;
            attr.INT = define.INT;
            attr.DEX = define.DEX;
            attr.AD = define.AD;
            attr.AP = define.AP;
            attr.DEF = define.DEF;
            attr.MDEF = define.MDEF;
            attr.SPD = define.SPD;
            attr.CRI = define.CRI;
            
        }

        private void LoadGrowthAttribute(AttributeData attr, CharacterDefine define)
        {
            //  只对一级属性成长
            attr.STR = define.GrowthSTR;
            attr.INT = define.GrowthINT;
            attr.DEX = define.GrowthDEX;
        }

        private void LoadEquipAttribute(AttributeData attr, List<EquipDefine> equips)
        {
            attr.Reset();
            foreach (var define in equips)
            {
                attr.MaxHP += define.MaxHP;
                attr.MaxMP += define.MaxMP;
                attr.STR += define.STR;
                attr.INT += define.INT;
                attr.DEX += define.DEX;
                attr.AD += define.AD;
                attr.AP += define.AP;
                attr.DEF += define.DEF;
                attr.MDEF += define.MDEF;
                attr.SPD += define.SPD;
                attr.CRI += define.CRI;
            }
        }

        /// <summary>
        /// 计算基础属性
        /// </summary>
        private void InitBasicAttributes()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++)
            {
                this.Basic.Data[i] = this.Initial.Data[i];
            }

            // 一级属性
            for (int i = (int)AttributeType.STR; i < (int)AttributeType.DEX; i++)
            {
                this.Basic.Data[i] = this.Initial.Data[i] + this.Growth.Data[i] * (this.Level - 1);
                this.Basic.Data[i] += this.Equip.Data[i];
            }
        }

        private void InitSecondaryAttributes()
        {
            // 二级属性成长
            this.Basic.MaxHP = this.Basic.STR * 10 + this.Initial.MaxHP + this.Equip.MaxHP;
            this.Basic.MaxMP = this.Basic.INT * 10 + this.Initial.MaxMP + this.Equip.MaxMP;

            this.Basic.AD = this.Basic.STR * 5 + this.Initial.AD + this.Equip.AD;
            this.Basic.AP = this.Basic.INT * 5 + this.Initial.AP + this.Equip.AP;
            this.Basic.DEF = this.Basic.STR * 2 + this.Basic.DEX * 1 + this.Initial.DEF + this.Equip.DEF;
            this.Basic.MDEF = this.Basic.INT * 2 + this.Basic.DEX * 1 + this.Initial.MDEF + this.Equip.MDEF;

            this.Basic.SPD = this.Basic.DEX* 0.2f + this.Initial.SPD + this.Equip.SPD;
            this.Basic.CRI = this.Basic.DEX * 0.0002f + this.Initial.CRI + this.Equip.CRI;
        }


        // 叠加动态属性(buff)
        private void InitFinalAttributes()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++)
            {
                this.Final.Data[i] = this.Basic.Data[i] + this.Buff.Data[i];
            }
        }
    }
}
