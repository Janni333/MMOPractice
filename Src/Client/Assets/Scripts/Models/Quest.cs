using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Quest
    {
        public NQuestInfo questInfo;
        public QuestDefine questDefine;

        public Quest(NQuestInfo questInfo)
        {
            this.questInfo = questInfo;
            this.questDefine = DataManager.Instance.Quests[questInfo.QuestId];
        }

        public Quest(QuestDefine questDefine)
        {
            this.questInfo = null;
            this.questDefine = questDefine;
        }

        public string GetTypeName(Quest quest)
        {
            return EnumUtil.GetEnumDescription(this.questDefine.Type);
        }
    }
}
