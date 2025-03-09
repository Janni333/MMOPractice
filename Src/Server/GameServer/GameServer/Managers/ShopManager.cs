using Common;
using Common.Data;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        public Result BuyItem(NetConnection<NetSession> sender, int shopid, int shopitemid)
        {
            //校验
            //是否存在商店；是否存在商品；是否可以购买
            if (!DataManager.Instance.Shops.ContainsKey(shopid))
            {
                Log.InfoFormat("道具购买商店不存在: Shop:{0}", shopid);
                return Result.Failed;
            }

            ShopItemDefine buyItem = null;
            if (DataManager.Instance.ShopItems[shopid].TryGetValue(shopitemid, out buyItem))
            {
                Character buyCha = sender.Session.Character;
                if (buyItem.Price <= buyCha.gold)
                {
                    buyCha.gold -= buyItem.Price;
                    buyCha.itemManager.AddItem(buyItem.ItemID, buyItem.count);
                    return Result.Success;
                }
            }            
            return Result.Failed;
        }
    }
}
