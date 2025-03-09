using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct BagItem
    {
        public ushort ItemId;
        public ushort ItemCount;

        public BagItem(int id, int count)
        {
            this.ItemId = (ushort)id;
            this.ItemCount = (ushort)count;
        }
    }
}
