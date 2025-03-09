using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class TestManager : Singleton<TestManager>
    {
        public void Init()
        {
            NPCManager.Instance.RigisterNpcMap(NPCFunction.InvokeShop, OnClickShopTest);
        }

        public bool OnClickShopTest(NPCDefine def)
        {
            Debug.Log("TEST");
            return true;
        }
    }
}
