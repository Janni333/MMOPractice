using Entities;
using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Battle
{
    public class BattleService : Singleton<BattleService>, IDisposable
    {
        public BattleService() 
        {
            MessageDistributer.Instance.Subscribe<SkillCastResponse>(this.OnSkillCast);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<SkillCastResponse>(this.OnSkillCast);
        }

        public void SendSkillCast(int skillid, int casterid, int targetid, NVector3 position)
        {
            if (position == null) position = new NVector3();
            Debug.LogFormat("【BattleService】发送SkillCastRequest: Skill:{0} caster:{1} target:{2} pos:{3}", skillid, casterid, targetid, position);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.skillCast = new SkillCastRequest();
            message.Request.skillCast.castInfo = new NSkillCastInfo();
            message.Request.skillCast.castInfo.skillId = skillid;
            message.Request.skillCast.castInfo.casterId = casterid;
            message.Request.skillCast.castInfo.targetId = targetid;
            message.Request.skillCast.castInfo.Position = position;
            NetClient.Instance.SendMessage(message);
        }

        
        public void OnSkillCast(object sender, SkillCastResponse message)
        {
            Debug.LogFormat("【BattleService】收到SkillCastResponse: Skill:{0} caster:{1} target:{2} pos:{3} result:{4}", message.castInfo.skillId, message.castInfo.casterId, message.castInfo.targetId, message.castInfo.Position, message.Result);
            if (message.Result == Result.Success)
            {
                // 此协议中的释放技能主体可能是该地图上的所有实体
                Creature caster = EntityManager.Instance.GetEntity(message.castInfo.casterId) as Creature;
                if (caster != null)
                {
                    Creature target = EntityManager.Instance.GetEntity(message.castInfo.targetId) as Creature;
                    caster.CastSkill(message.castInfo.skillId, target, message.castInfo.Position);
                }
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }
        }
    }
}
