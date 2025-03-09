using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Item
    {
        public int ItemID;
        public int Count;
        public ItemDefine itemdef;

        public Item(NItemInfo item) : this(item.Id, item.Count)
        { 
        }

        public Item(int id, int count)
        {
            this.ItemID = id;
            this.Count = count;
            DataManager.Instance.Items.TryGetValue(id, out this.itemdef);
        }

        public override string ToString()
        {
            return string.Format("Id:{0},Count:{1}", this.ItemID, this.Count);
        }
    }
}
