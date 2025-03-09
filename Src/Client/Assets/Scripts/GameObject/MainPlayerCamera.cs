using Assets.Scripts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera playerCamera;
    public GameObject player;
    void Update()
    {
    }

    private void LateUpdate()
    {
        if (player == null && User.Instance.currentCharacterObj != null)
        {
            player = User.Instance.currentCharacterObj.gameObject;
        }

        if (player == null) return;

        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
