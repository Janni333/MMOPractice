using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void Init()
        { }

        public void SendBuyItem(int shopid, int itemid)
        {
            //Log
            Debug.LogFormat("ItemService发送道具购买请求：Shop:{0} ShopItem:{1}", shopid, itemid);

            //
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.Shopid = shopid;
            message.Request.itemBuy.Itemid = itemid;

            NetClient.Instance.SendMessage(message);
        }

        internal void SendEquipItem(Item equip, bool v)
        {
            throw new NotImplementedException();
        }

        private void OnItemBuy(object sender, ItemBuyResponse response)
        {
            //Log
            Debug.LogFormat("ItemService接收道具购买请求：Result:{0} Info:{1}", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                MessageBox.Show("购买成功");
            }
        }
    }
}
