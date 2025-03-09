using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBox {
    static Object cacheobject = null;

    public static UIInputBox Show(string message, string title = "", string btnok = "", string btnCancle = "", string tip = "")
    {
        if (cacheobject == null)
        {
            cacheobject = Resloader.Load<Object>("UI/UIInputBox");
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheobject);
        UIInputBox ipb = go.GetComponent<UIInputBox>();
        ipb.Init(message, title, btnok, btnCancle, tip);

        return ipb;
    }
}
