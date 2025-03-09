using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    public class Item
    {
        public TCharacterItem TItem;

        public int ItemID;
        public int Count;

        public Item(TCharacterItem titem)
        {
            this.TItem = titem;
            this.ItemID = titem.ItemID;
            this.Count = titem.ItemCount;
        }

        public void AddItem(int count)
        {
            this.Count += count;
            this.TItem.ItemCount = this.Count;
        }

        public void Remove(int count)
        {
            this.Count -= count;
            this.TItem.ItemCount = this.Count;
        }

        public bool UseItem(int count = 1)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.ItemID, this.Count);
        }
    }
}
