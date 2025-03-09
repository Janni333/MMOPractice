using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class QuestManager
    {
        Character Owner;

        public QuestManager(Character cha)
        {
            this.Owner = cha;
        }

        public void GetQuestInfos(List<NQuestInfo> questinfos)
        {
            foreach (var quest in Owner.Tcharacter.Quests)
            {
                questinfos.Add(GetQuestInfo(quest));
            }
        }

        NQuestInfo GetQuestInfo(TCharacterQuest Tquest)
        {
            NQuestInfo questinfo = new NQuestInfo()
            {
                QuestId = Tquest.QuestID,
                QuestGuid = Tquest.Id,
                Status = (QuestStatus)Tquest.Status,
                Targets = new int[3]
                {
                    Tquest.Target1,
                    Tquest.Target2,
                    Tquest.Target3
                }
            };
            return questinfo;
        }


        public Result AcceptQuest(NetConnection<NetSession> sender, int questid)
        {
            Character character = sender.Session.Character;
            QuestDefine questdefine;
            if (DataManager.Instance.Quests.TryGetValue(questid, out questdefine))
            {
                TCharacterQuest dbquest = DBService.Instance.Entities.CharacterQuests.Create();
                dbquest.QuestID = questdefine.ID;
                if (questdefine.Target1 == QuestTarget.None)
                {
                    dbquest.Status = (int)QuestStatus.Complete;
                }
                else
                {
                    dbquest.Status = (int)QuestStatus.InProgress;
                }
                sender.Session.Response.questAccept.Questinfo = this.GetQuestInfo(dbquest);
                character.Tcharacter.Quests.Add(dbquest);
                DBService.Instance.Save();
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        internal Result SubmitQuest(NetConnection<NetSession> sender, int questId)
        {
            Character character = sender.Session.Character;
            QuestDefine questdefine;
            if (DataManager.Instance.Quests.TryGetValue(questId, out questdefine))
            {
                var dbquest = character.Tcharacter.Quests.Where(q => q.QuestID == questId).FirstOrDefault();
                if (dbquest != null)
                {
                    if (dbquest.Status != (int)QuestStatus.Complete)
                    {
                        sender.Session.Response.questSubmit.Errormsg = "该任务未完成";
                        return Result.Failed;
                    }
                    else
                    {
                        dbquest.Status = (int)QuestStatus.Finished;
                        sender.Session.Response.questSubmit.Questinfo = this.GetQuestInfo(dbquest);
                        DBService.Instance.Save();

                        if (questdefine.RewardExp > 0)
                        {
                            //character.Exp += questdefine.RewardExp;
                        }
                        if (questdefine.RewardGold > 0)
                        {
                            character.gold += questdefine.RewardGold;
                        }

                        if (questdefine.RewardItem1 > 0)
                        {
                            character.itemManager.AddItem(questdefine.RewardItem1, questdefine.RewardItem1Count);
                        }
                        if (questdefine.RewardItem2 > 0)
                        {
                            character.itemManager.AddItem(questdefine.RewardItem2, questdefine.RewardItem2Count);
                        }
                        if (questdefine.RewardItem3 > 0)
                        {
                            character.itemManager.AddItem(questdefine.RewardItem3, questdefine.RewardItem3Count);
                        }
                        DBService.Instance.Save();
                        return Result.Success;
                    }
                }

                sender.Session.Response.questSubmit.Errormsg = "角色不包含该任务";
                return Result.Failed;
            }
            sender.Session.Response.questSubmit.Errormsg = "该任务不存在";
            return Result.Failed;
        }
    }
}
