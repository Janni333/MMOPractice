using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class UIGuildPopNoGuild : UIWindow
{
    public void OnClickCreate()
    {
        UIManager.Instance.Show<UIGuildPopCreate>();
        this.Close();
    }

    public void OnClickJoin() 
    {
        UIManager.Instance.Show<UIGuildList>();
        this.Close();
    }
}
