using Assets.Scripts.Model;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Managers
{
    public enum NPCQuestStatus
    {
        None = 0,
        Available = 1,
        Complete = 2,
        Incomplete = 3
    }

    public class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> questInfos;
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        public Dictionary<int, Dictionary<NPCQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NPCQuestStatus, List<Quest>>>();
        public UnityAction<Quest> OnQuestStatusChanged;

        #region Init Logic
        //初始化Manager
        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            this.allQuests.Clear();
            this.npcQuests.Clear();
            this.InitQuests();
        }

        public void InitQuests()
        {
            //已接取任务加入管理器
            CheckCharQuest();

            //可接取任务加入管理器
            CheckAvailableQuest();

            //所有任务分配给NPC
            foreach (var quest in allQuests.Values)
            {
                AddQuesttoNPC(quest.questDefine.AcceptNPC, quest);
                AddQuesttoNPC(quest.questDefine.SubmitNPC, quest);
            }
        }

        private void CheckCharQuest()
        {
            foreach (var info in questInfos)
            {
                Quest quest = new Quest(info);
                allQuests.Add(info.QuestId, quest);
            }
        }

        private void CheckAvailableQuest()
        {
            foreach (var define in DataManager.Instance.Quests.Values)
            {
                if (this.allQuests.ContainsKey(define.ID))      // 已接取
                    continue;
                if (define.LimitLevel > User.Instance.currentCharacterInfo.Level)   // 等级限制
                    continue;
                if (define.LimitClass != CharacterClass.None && define.LimitClass != User.Instance.currentCharacterInfo.Class)      // 角色限制
                    continue;
                
                if (define.PreQuest > 0)    // 前置任务限制
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(define.PreQuest, out preQuest))
                    {
                        if (preQuest.questInfo == null)
                            continue;
                        if (preQuest.questInfo.Status != QuestStatus.Finished)
                            continue;
                    }
                    else
                        continue;
                }

                Quest quest = new Quest(define);
                allQuests.Add(quest.questDefine.ID, quest);
            }
        }

        //分配任务至npc
        public void AddQuesttoNPC(int npcid, Quest quest)
        {
            //检验并创建该npc字典是否存在
            Dictionary<NPCQuestStatus, List<Quest>> statusQuests;
            if (!this.npcQuests.TryGetValue(npcid, out statusQuests))
            {
                statusQuests = new Dictionary<NPCQuestStatus, List<Quest>>();
                this.npcQuests.Add(npcid, statusQuests);
            }

            //检验并创建该状态列表是否存在
            List<Quest> available;
            List<Quest> complete;
            List<Quest> incomplete;

            if (!statusQuests.TryGetValue(NPCQuestStatus.Available, out available))
            {
                available = new List<Quest>();
                statusQuests.Add(NPCQuestStatus.Available, available);
            }
            if (!statusQuests.TryGetValue(NPCQuestStatus.Complete, out complete))
            {
                complete = new List<Quest>();
                statusQuests.Add(NPCQuestStatus.Complete, complete);
            }
            if (!statusQuests.TryGetValue(NPCQuestStatus.Incomplete, out incomplete))
            {
                incomplete = new List<Quest>();
                statusQuests.Add(NPCQuestStatus.Incomplete, incomplete);
            }

            //分配任务
            if (quest.questInfo == null)    // 可接取任务（无NInfo）
            {
                if (quest.questDefine.AcceptNPC == npcid && !available.Contains(quest))
                    available.Add(quest);
            }
            else
            {
                if (quest.questDefine.SubmitNPC == npcid && quest.questInfo.Status == QuestStatus.Complete)
                {
                    if(!complete.Contains(quest))
                        complete.Add(quest);
                }

                if (quest.questDefine.SubmitNPC == npcid && quest.questInfo.Status == QuestStatus.InProgress)
                {
                    if(!incomplete.Contains(quest))
                        incomplete.Add(quest);
                }
            }
        }
        #endregion

        #region UI Logic
        ///<summary>
        ///获取NPC状态
        ///</summary>
        ///<param name="npcId"></param>
        ///<returns></returns>
        public NPCQuestStatus GetQuestStatusbyNPC(int npcid)
        {
            Dictionary<NPCQuestStatus, List<Quest>> status = new Dictionary<NPCQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcid, out status))
            {
                if (status[NPCQuestStatus.Complete].Count > 0)
                    return NPCQuestStatus.Complete;
                if (status[NPCQuestStatus.Available].Count > 0)
                    return NPCQuestStatus.Available;
                if (status[NPCQuestStatus.Incomplete].Count > 0)
                    return NPCQuestStatus.Incomplete;
            }
            return NPCQuestStatus.None;
        }

        public bool OpenNPCQuest(int npcid)
        {
            Dictionary<NPCQuestStatus, List<Quest>> statusQuests;
            if (this.npcQuests.TryGetValue(npcid, out statusQuests))
            {
                if (statusQuests[NPCQuestStatus.Complete].Count > 0)
                    ShowQuestDia(statusQuests[NPCQuestStatus.Complete].First());
                if (statusQuests[NPCQuestStatus.Available].Count > 0)
                    ShowQuestDia(statusQuests[NPCQuestStatus.Available].First());
                if (statusQuests[NPCQuestStatus.Incomplete].Count > 0)
                    ShowQuestDia(statusQuests[NPCQuestStatus.Incomplete].First());
            }
            return false;
        }

        bool ShowQuestDia(Quest quest)
        {
            if (quest.questInfo == null || quest.questInfo.Status == QuestStatus.Complete)
            {
                UIQuestDialog dia = UIManager.Instance.Show<UIQuestDialog>();
                dia.SetDia(quest);
                dia.OnClose += OnDialogClose;
                return true;
            }
            if (quest.questInfo != null && quest.questInfo.Status == QuestStatus.InProgress)
            {
                if (!string.IsNullOrEmpty(quest.questDefine.DialogIncomplete))
                    MessageBox.Show(quest.questDefine.DialogIncomplete);
            }
            return true;
        }

        void OnDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog ui = (UIQuestDialog)sender;
            if (result == UIWindow.WindowResult.Yes)
            {
                if (ui.quest.questInfo == null)
                    QuestService.Instance.SendQuestAccept(ui.quest.questDefine.ID);
                if(ui.quest.questInfo != null && ui.quest.questInfo.Status == QuestStatus.Complete)
                    QuestService.Instance.SendQuestSubmit(ui.quest.questDefine.ID);
            }
            if (result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(ui.quest.questDefine.DialogDeny);
            }
        }
        #endregion

        #region Quest Logic
        private Quest RefreshQuestStatus(NQuestInfo questinfo)
        {
            Quest questUpdate;
            if (allQuests.ContainsKey(questinfo.QuestId))
            {
                allQuests[questinfo.QuestId].questInfo = questinfo;
                questUpdate = allQuests[questinfo.QuestId];
            }
            else
            {
                questUpdate = new Quest(questinfo);
                allQuests[questinfo.QuestId] = questUpdate;
            }

            npcQuests.Clear();
            CheckAvailableQuest();
            foreach (var quest in allQuests.Values)
            {
                AddQuesttoNPC(quest.questDefine.AcceptNPC, quest);
                AddQuesttoNPC(quest.questDefine.SubmitNPC, quest);
            }

            if (this.OnQuestStatusChanged != null)
                this.OnQuestStatusChanged(questUpdate);

            return questUpdate;
        }

        public void OnQuestAccept(NQuestInfo questinfo)
        {
            Quest quest = RefreshQuestStatus(questinfo);
            MessageBox.Show(quest.questDefine.DialogAccept);
        }

        public void OnQuestSubmit(NQuestInfo questinfo)
        {
            Quest quest = RefreshQuestStatus(questinfo);
            MessageBox.Show(quest.questDefine.DialogFinish);
            
        }
        #endregion
    }
}
