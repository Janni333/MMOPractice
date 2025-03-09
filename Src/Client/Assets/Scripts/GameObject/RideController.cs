using ProtoBuf.Meta;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

internal class RideController : MonoBehaviour
{
    public Transform mountPoint;
    public EntityController rider;
    public Vector3 offset;
    private Animator anim;

    void Start()
    {
        this.anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (this.mountPoint == null || this.rider == null) return;

        this.rider.SetRidePosition(this.mountPoint.position + this.mountPoint.TransformDirection(this.offset));
    }

    internal void SetRider(EntityController rider)
    {
        this.rider = rider;
    }

    public void OnEntityEvent(EntityEvent eve, int param)
    {
        switch (eve)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
                case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }
}
