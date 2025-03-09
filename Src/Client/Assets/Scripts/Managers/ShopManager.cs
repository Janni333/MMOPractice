using Common.Data;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        #region Data
        public int curShop;

        public void Init()
        {
            NPCManager.Instance.RigisterNpcMap(NPCFunction.InvokeShop, OnClickShop);
        }
        #endregion

        #region Management
        public bool OnClickShop(NPCDefine def)
        {
            this.curShop = def.Param;

            UIShop uishop = UIManager.Instance.Show<UIShop>();
            if (uishop != null)
            {
                if (DataManager.Instance.Shops.ContainsKey(def.Param))
                {
                    uishop.InitShop(def.Param);
                }
            }
            return true;
        }

        internal void ItemBuy(int shopid, int shopitemid)
        {
            ItemService.Instance.SendBuyItem(shopid, shopitemid);
        }
        #endregion
    }
}
