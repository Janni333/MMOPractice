using Common;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;
using GameServer.Managers;

namespace GameServer.Services
{
    class ItemService : Singleton<ItemService>
    {
        public ItemService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<SkillBridge.Message.ItemBuyRequest>(this.OnItemBuy);
        }
        public void Init()
        { }

        private void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest request)
        {
            //Log
            Log.InfoFormat("ItemService接收道具购买请求：Shop：{0} Item：{1}", request.Shopid, request.Itemid);

            //Message
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = ShopManager.Instance.BuyItem(sender, request.Shopid, request.Itemid);
            sender.Session.Response.itemBuy.Errormsg = "None";

            sender.SendResponse();
        }
    }
}
