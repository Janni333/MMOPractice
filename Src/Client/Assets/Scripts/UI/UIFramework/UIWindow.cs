using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow window, WindowResult result);
    public event CloseHandler OnClose;
    public virtual Type windowType
    {
        get { return this.GetType(); }
    }

    public GameObject root;

    public enum WindowResult
    {
        None = 0,
        Yes,
        No
    }

    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Close(this.windowType);
        if (this.OnClose != null)
        {
            this.OnClose(this, result);
        }
        this.OnClose = null;
    }

    public virtual void OnClickClose()
    {
        this.Close();
    }

    public virtual void OnClickYes()
    {
        this.Close(WindowResult.Yes);
    }

    public void OnClickNo()
    {
        this.Close(WindowResult.No);
    }
}
