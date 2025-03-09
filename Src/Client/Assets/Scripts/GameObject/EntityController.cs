using Battle;
using Entities;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EntityController : MonoBehaviour, IEntitySyncNotify, IEntityController
{
    /*信息
     *   
     */
    public Animator ani;
    public Rigidbody rb;
    public Collider co;
    public Entity curEntity;

    public float Speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;
    public bool isPlayer = false;

    public Vector3 Position;
    public Vector3 Direction;
    public Quaternion Rotation;
    public Vector3 lasPosition;
    public Quaternion lasRotation;

    public RideController rideController;
    private int currentRide = 0;
    public Transform rideBone;


    void Start()
    {
        if (this.curEntity != null)
        {
            this.UpdateTransform();
        }

        if (!this.isPlayer)
        {
            this.rb.useGravity = false;
        }
    }
    void OnDestroy()
    {
        if (this.curEntity != null)
        {
            Debug.LogFormat("EntityDestroy: Entity: {0}", this.curEntity.entityId);
        }
    }

    //每帧移动
    void FixedUpdate()
    {
        if (this.curEntity == null)
        {
            return;
        }

        this.curEntity.OnUpdate(Time.deltaTime);

        if (! isPlayer)
        {
            this.UpdateTransform();
        }
    }

    //从逻辑更新对象
    void UpdateTransform()
    {
        this.Position = GameObjectTool.LogicToWorld(this.curEntity.position);
        this.Direction = GameObjectTool.LogicToWorld(this.curEntity.direction);

        this.rb.MovePosition(this.Position);
        this.transform.forward = this.Direction;
        this.lasPosition = this.Position;
        this.lasRotation = this.Rotation;
    }



    #region IEntitySyncNotify
    //同步方法
    public void OnEntityEvent(EntityEvent eve, int param)
    {
        Debug.LogFormat("EC同步Entity事件:Entity:{0}", curEntity.entityId);
        switch (eve)
        {
            case EntityEvent.Idle:
                ani.SetBool("Move", false);
                ani.SetTrigger("Idle");
                break;
            case EntityEvent.MoveBack:
                ani.SetBool("Move", true);
                break;
            case EntityEvent.MoveFwd:
                ani.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                ani.SetBool("Move", false);
                ani.SetTrigger("Jump");
                break;
            case EntityEvent.Ride:
                this.Ride(param);
                break;
        }
        if (this.rideController != null) this.rideController.OnEntityEvent(eve, param);
    }
    public void OnEntityRm()
    {
        Debug.LogFormat("EC同步Entity移除");

        //校验！！！！！！啊啊啊啊啊啊啊啊啊啊！！！！！！！！！！！！！！！！！！！
        if (UIWorldManager.Instance != null)
        {
            UIWorldManager.Instance.Rmcharinfobar(this.transform);
        }
        Destroy(this.gameObject);
    }

    public void OnEntityChange(NEntity entitydata)
    {
        Debug.LogFormat("EC同步Entity数据改变:Entity:{0}", curEntity.entityId);
    }

    internal void Ride(int rideId)
    {
        if (currentRide == rideId) return;
        this.currentRide = rideId;
        if (rideId > 0)
        {
            this.rideController = GameObjectManager.Instance.LoadRide(rideId, this.transform);
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }

        if (this.rideController == null)
        {
            this.ani.transform.localPosition = Vector3.zero;
            this.ani.SetLayerWeight(1, 0);
        }
        else
        {
            this.rideController.SetRider(this);
            this.ani.SetLayerWeight(1, 1);
        }

    }

    public void SetRidePosition(Vector3 position)
    {
        this.ani.transform.position = position + (this.ani.transform.position - this.rideBone.position);
    }

    void OnMouseDown()
    {
        BattleManager.Instance.CurrentTarget = this.curEntity as Creature;
    }

    public void PlayAnim(string name)
    {
        this.ani.SetTrigger(name);
    }

    public void SetStandby(bool standby)
    {
        this.ani.SetBool("Standby", standby);
    }

    #endregion
}
