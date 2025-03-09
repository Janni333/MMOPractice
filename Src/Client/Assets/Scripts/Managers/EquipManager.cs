using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Managers
{
    internal class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangedHandler();
        public event OnEquipChangedHandler OnEquipChanged;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];
        byte[] Data;

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }

        internal bool Contains(int equipid)
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].ItemID == equipid)
                    return true;
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }

        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int itemid = *(int*)(pt + i * sizeof(int));
                    if(itemid > 0)
                        Equips[i] = ItemManager.Instance.Items[itemid];
                    else
                        Equips[i] = null;
                }
            }
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = this.Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemid = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemid = 0;
                    else
                        *itemid = Equips[i].ItemID;
                }
            }
            return this.Data;
        }


        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }

        public void OnEquipItem(Item equip)
        {
            //if (this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].ItemID == equip.ItemID)
                //return;
            //this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.ItemID];

            if (OnEquipChanged != null)
                OnEquipChanged();
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equips[(int)slot] != null)
            {
                this.Equips[(int)slot] = null;
                if(OnEquipChanged != null)
                    OnEquipChanged();
            }
        }

        public List<EquipDefine> GetEquipedDefines()
        {
            List<EquipDefine> result = new List<EquipDefine>();
            for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
            {
                //if (Equips[i] != null)
                    //result.Add(Equips[i].EquipInfo);
            }
            return result;
        }

        
    }
}
