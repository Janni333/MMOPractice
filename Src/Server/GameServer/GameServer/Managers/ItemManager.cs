using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ItemManager
    {
        Character Owner;

        public Dictionary<int, Item> CharItems = new Dictionary<int, Item>();

        public ItemManager(Character cha)
        {
            this.Owner = cha;

            foreach (var item in cha.Tcharacter.Items)
            {
                Item newitem = new Item(item);
                this.CharItems.Add(newitem.ItemID, newitem);
            }
        }

        public bool HasItem(int id)
        {
            Item itemhas = null;
            if (this.CharItems.TryGetValue(id, out itemhas))
            {
                return itemhas.Count > 0;
            }
            return false;
        }

        public Item GetItem(int id)
        {
            Item itemget = null;
            this.CharItems.TryGetValue(id, out itemget);

            Log.InfoFormat("获取道具：Item:{0} Count:{1}", itemget.ItemID, itemget.Count);
            return itemget;
        }

        public bool AddItem(int id, int count)
        {
            Item itemadd = null;
            if (this.CharItems.TryGetValue(id, out itemadd))
            {
                itemadd.AddItem(count);
            }
            else
            {
                TCharacterItem Titem = new TCharacterItem()
                {
                    ItemID = id,
                    ItemCount = count,
                    TCharacterID = Owner.Tcharacter.ID,
                    Owner = Owner.Tcharacter
                };
                Owner.Tcharacter.Items.Add(Titem);

                this.CharItems.Add(id, new Item(Titem));
            }
            Owner.statusManager.AddItemStatus(id, count, StatusAction.Add);
            DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int id, int count)
        {
            Item itemrm = null;
            if (!this.CharItems.TryGetValue(id, out itemrm))
            {
                return false;
            }
            if (itemrm.Count < count)
            {
                return false;
            }

            itemrm.Remove(count);
            return true;
        }

        public bool UseItem(int id, int count = 1)
        {
            Item itemuse = null;
            if (this.CharItems.TryGetValue(id, out itemuse))
            {
                if (itemuse.Count >= count)
                {
                    Log.InfoFormat("使用道具:Owner:{0} Item:{1} {2} Count:{3}",this.Owner.Tcharacter.Name, itemuse.ItemID, DataManager.Instance.Items[itemuse.ItemID].Name, count);
                    
                    //todo:使用逻辑
                    itemuse.Remove(count);
                    return true;
                }
            }
            return false;
        }


        //内存数据——>网络数据
        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in this.CharItems)
            {
                list.Add(new NItemInfo() 
                {
                    Id = item.Value.ItemID, 
                    Count = item.Value.Count 
                }
                );
            }
        }
    }
}
