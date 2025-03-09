using Entities;
using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;


class PlayerInputController : MonoBehaviour
{
    /*
     *信息
     *  
     */

    CharacterState curState = CharacterState.Idle;
    public Character curCharacter;
    public EntityController ec;
    public Rigidbody rb;
    public bool onAir = false;

    public int Speed;
    public float rotateSpeed = 2.0f;
    public float turnAngle = 10;

    private NavMeshAgent agent;
    private bool autoNav = false;

    // Use this for initialization
    void Start() {
        curState = SkillBridge.Message.CharacterState.Idle;
        if (this.curCharacter == null)
        {
            DataManager.Instance.Load();
            NCharacterInfo cinfo = new NCharacterInfo();
            cinfo.Id = 1;
            cinfo.Name = "Test";
            cinfo.ConfigId = 1;
            cinfo.Entity = new NEntity();
            cinfo.Entity.Position = new NVector3();
            cinfo.Entity.Direction = new NVector3();
            cinfo.Entity.Direction.X = 0;
            cinfo.Entity.Direction.Y = 100;
            cinfo.Entity.Direction.Z = 0;
            this.curCharacter = new Character(cinfo);

            if (ec != null) ec.curEntity = this.curCharacter;
        }

        if (agent == null)
        {
            agent = this.gameObject.AddComponent<NavMeshAgent>();
            agent.stoppingDistance = 0.3f;
        }
    }


    void FixedUpdate()
    {
        if (curCharacter == null) return;

        if (autoNav)
        {
            NavMove();
            return;
        }

        if (InputManager.Instance != null && InputManager.Instance.IsInputMode) return;
        //确定是否绑定角色
        if (curCharacter != null)
        {
            //移动
            //  获取Vertical输入
            //  判断阈值向前/向后/静止
            //      判断并设置State
            //      调用移动方法
            //      发送事件
            //      计算速度
            float v = Input.GetAxis("Vertical");
            if (v > 0.01f)
            {
                //状态改变
                if (this.curState != CharacterState.Move)
                {
                    this.curState = CharacterState.Move;
                    curCharacter.MoveForward();
                    this.SendEntityEvent(EntityEvent.MoveFwd);
                }
                //rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(curCharacter.direction) * curCharacter.speed / 100f;
                rb.velocity = GameObjectTool.LogicToWorld(curCharacter.direction) * curCharacter.speed / 100f;
            }
            else if (v < -0.01f)
            {
                if (this.curState != CharacterState.Move)
                {
                    this.curState = CharacterState.Move;
                    curCharacter.MoveBack();
                    this.SendEntityEvent(EntityEvent.MoveBack);
                }
                //rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(curCharacter.direction) * curCharacter.speed / 100f;
                rb.velocity = GameObjectTool.LogicToWorld(curCharacter.direction) * curCharacter.speed / 100f;
            }
            else
            {
                if (this.curState != CharacterState.Idle)
                {
                    this.curState = CharacterState.Idle;
                    curCharacter.Stop();
                    this.SendEntityEvent(EntityEvent.Idle);
                }
            }

            //跳跃
            //  发送事件
            if (Input.GetButtonDown("Jump"))
            {
                this.SendEntityEvent(EntityEvent.Jump);
            }


            //旋转
            //  获取Horizontal输入
            //  判断阈值
            float h = Input.GetAxis("Horizontal");
            if (h < 0.1 || h > 0.1)
            {
                this.transform.Rotate(0, h * rotateSpeed, 0);

                Vector3 charDir = GameObjectTool.LogicToWorld(this.curCharacter.direction);
                Quaternion rot = new Quaternion();
                rot.SetFromToRotation(this.transform.forward, charDir);

                if (rot.eulerAngles.y > this.turnAngle || rot.eulerAngles.y < (360 - this.turnAngle))
                {
                    this.curCharacter.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                    rb.transform.forward = this.transform.forward;
                }
            }
        }
    }

    Vector3 lastpos;
    void LateUpdate()
    {
        if (this.curCharacter == null) return;

        Vector3 offset = this.rb.transform.position - this.lastpos;
        this.Speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastpos = this.rb.transform.position;

        //更新transform position
        this.transform.position = this.lastpos;
        //更新logical position
        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.curCharacter.position).magnitude > 50f)
        {
            this.curCharacter.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
    }

    public void SendEntityEvent(EntityEvent eve, int param = 0)
    {
        if (this.ec != null)
        {
            this.ec.OnEntityEvent(eve, param);
        }
        MapService.Instance.SendEntitySync(curCharacter.entityId, curCharacter, eve, param);
    }


    #region Nav
    public void StartNav(Vector3 target)
    {
        StartCoroutine(BeginNav(target));
    }

    IEnumerator BeginNav(Vector3 target)
    {
        // 设置目的地
        agent.SetDestination(target);
        yield return null;

        // 设置状态和速度
        autoNav = true;
        if (curState != CharacterState.Move)
        {
            curState = CharacterState.Move;
            this.curCharacter.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            agent.speed = this.curCharacter.speed / 100f;
        }
    }

    public void StopNav()
    {
        autoNav = false;
        agent.ResetPath();
        if (curState != CharacterState.Idle)
        {
            curState = CharacterState.Idle;
            this.rb.velocity = Vector3.zero;
            this.curCharacter.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
        NavPathRenderer.Instance.SetPath(null, Vector3.zero);
    }

    // 寻路过程中逻辑
    public void NavMove()
    {
        if (agent.pathPending) return;
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            StopNav();
            return;
        }
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return;

        // 玩家主动移动
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1)
        {
            StopNav();
            return;
        }

        NavPathRenderer.Instance.SetPath(agent.path, agent.destination);
        // 有没有到达
        if (agent.isStopped || agent.remainingDistance < 0.3f)
        {
            StopNav();
            return;
        }
    }

    #endregion

}
