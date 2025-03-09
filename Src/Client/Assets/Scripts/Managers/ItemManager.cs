
using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager()
        {
            StatusService.Instance.RegisterHandler(StatusType.Item, this.ChangeItemfromStatus);
        }


        internal void Init(List<NItemInfo> items)
        {
            this.Items.Clear();

            foreach (var Nitem in items)
            {
                Item newitem = new Item(Nitem);
                this.Items.Add(newitem.ItemID, newitem);
                Debug.LogFormat("初始化道具管理器:Item:{0} Count:{1}", newitem.ItemID, newitem.Count);
            }
        }

        public void ChangeItemfromStatus(NStatusInfo statusInfo)
        {
            if (statusInfo.Action == StatusAction.Add)
            {
                this.AddItem(statusInfo.Id, statusInfo.Value);
            }
            if (statusInfo.Action == StatusAction.Delete)
            {
                this.RemoveItem(statusInfo.Id, statusInfo.Value);
            }
        }

        public bool AddItem(int id, int count)
        {
            Item itemadd = null;
            if (this.Items.TryGetValue(id, out itemadd))
            {
                itemadd.Count += count;
            }
            this.Items.Add(id, new Item(id, count));
            return true;
        }

        public bool RemoveItem(int id, int count)
        {
            Item itemrm = null;
            if (this.Items.TryGetValue(id, out itemrm))
            {
                if (itemrm.Count >= count)
                {
                    itemrm.Count -= count;
                    return true;
                }
            }
            return false;
        }

        public bool UseItem(int id)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}
