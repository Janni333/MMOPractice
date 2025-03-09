using Assets.Scripts.Model;
using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    public class MinimapManager: Singleton<MinimapManager>
    {
        #region Minimap & Info
        public UIMinimap minimap;

        //包围盒
        private Collider mapboundingbox;
        public Collider mapboundingBox
        {
            get { return mapboundingbox; }
            set { this.mapboundingbox = value; }
        }
        //角色
        private Transform playerTransform;
        public Transform PlayerTransform
        {
            get
            {
                if (this.playerTransform == null)
                {
                    this.playerTransform = User.Instance.currentCharacterObj.transform;
                }
                return this.playerTransform;
            }
        }
        //地图信息
        private MapDefine minidefine;
        public MapDefine miniDefine
        {
            get
            {
                if (this.minidefine == null)
                {
                    this.minidefine = User.Instance.currentMapdefine;
                }
                return this.minidefine;
            }
        }
        #endregion
        
        public void UpdataMinimap(Collider boudingbox)
        {
            this.mapboundingBox = boudingbox;
            if (this.minimap != null)
            {
                this.minimap.UpdateMapInfo();
            }
        }

        public Sprite LoadSprite()
        {
            Sprite mapSprite = Resloader.Load<Sprite>("UI/Minimap/" + miniDefine.MiniMap);
            return mapSprite;
        }

    }
}
