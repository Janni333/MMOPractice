using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class UIGuildPopCreate: UIWindow
{
    public InputField inputName;
    public InputField inputDes;

    private void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreate;
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    public override void OnClickYes()
    {
        if (string.IsNullOrEmpty(inputName.text))
        {
            MessageBox.Show("Please enter guild name", "Error", MessageBoxType.Error);
            return;
        }
        if (inputName.text.Length < 4 || inputName.text.Length > 10)
        {
            MessageBox.Show("公会名称为4-10个字符", "Error", MessageBoxType.Error);
            return;
        }

        if (string.IsNullOrEmpty(inputDes.text))
        {
            MessageBox.Show("Please enter guild des", "Error", MessageBoxType.Error);
            return;
        }

        GuildService.Instance.SendGuildCreate(inputName.text, inputDes.text);
    }

    void OnGuildCreate(bool result)
    {
        if(result)
            this.Close(WindowResult.Yes);
    }
}
