using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class BagManager : Singleton<BagManager>
    {
        //
        public int unlockedSlots;
        public NBagInfo NbagInfo;
        public BagItem[] bagItems;

        //

        #region Data Transfer
        unsafe void Analyze(byte[] data)
        {
            fixed (byte* pt = data)
            {
                for (int i = 0; i < unlockedSlots; i++)
                {
                    BagItem* bg = (BagItem*)(pt + i * sizeof(BagItem));
                    this.bagItems[i] = *bg;
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()
        {
            fixed (byte* pt = this.NbagInfo.Items)
            {
                for (int i = 0; i < this.unlockedSlots; i++)
                {
                    BagItem* bg = (BagItem*)(pt + i * sizeof(BagItem));
                    *bg = this.bagItems[i];
                }
            }
            return this.NbagInfo;
        }
        #endregion

        #region Management
        unsafe public void Init(NBagInfo netInfo)
        {
            this.NbagInfo = netInfo;
            this.unlockedSlots = netInfo.Unlocked;
            this.bagItems = new BagItem[this.unlockedSlots];

            if (netInfo.Items != null && netInfo.Items.Length > this.unlockedSlots)
            {
                this.Analyze(netInfo.Items);
            }
            else
            {
                netInfo.Items = new byte[sizeof(BagItem) * this.unlockedSlots];
                this.Reset();
            }

            Debug.LogFormat("初始化背包管理器:Bag:{0}", unlockedSlots);
        }

        public void Reset()
        {
            int i = 0;
            foreach (var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Count <= kv.Value.itemdef.StackLimit)
                {
                    this.bagItems[i].ItemId = (ushort)kv.Value.ItemID;
                    this.bagItems[i].ItemCount = (ushort)kv.Value.Count;
                }
                else
                {
                    while (kv.Value.Count > kv.Value.itemdef.StackLimit)
                    {
                        this.bagItems[i].ItemId = (ushort)kv.Value.ItemID;
                        this.bagItems[i].ItemCount = (ushort)kv.Value.itemdef.StackLimit;
                        i++;
                        kv.Value.Count -= kv.Value.itemdef.StackLimit;
                    }
                    this.bagItems[i].ItemId = (ushort)kv.Value.ItemID;
                    this.bagItems[i].ItemCount = (ushort)kv.Value.Count;
                }
                i++;
            }
        }

        public void AddItem()
        { }

        public void RemoveItem()
        { }
        #endregion

    }
}