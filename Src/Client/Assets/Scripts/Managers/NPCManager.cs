using Common.Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class NPCManager : Singleton<NPCManager>
    {
        public delegate bool NpcActionHandeler(NPCDefine npcdef);

        public Dictionary<NPCFunction, NpcActionHandeler> NpcMap = new Dictionary<NPCFunction, NpcActionHandeler>();
        public Dictionary<int, Vector3> NpcPos = new Dictionary<int, Vector3>();

        //注册方法
        public void RigisterNpcMap(NPCFunction function, NpcActionHandeler npcAction)
        {
            if (!this.NpcMap.ContainsKey(function))
            {
                NpcMap[function] = npcAction;
            }
            else
            {
                NpcMap[function] += npcAction;
            }
        }


        //交互方法
        public bool NpcInteract(NPCDefine def, NPCQuestStatus npcStatus)
        {
            if(npcStatus != NPCQuestStatus.None && DoTaskInteraction(def))
            {
                return true;
            }
            if (def.Type == NPCType.Functional)
            {
                return DoFunctionInteraction(def);
            }
            return false;
        }

        private bool DoFunctionInteraction(NPCDefine def)
        {
            if (this.NpcMap.ContainsKey(def.Function))
            {
                return NpcMap[def.Function](def);
            }
            return false;
        }

        private bool DoTaskInteraction(NPCDefine def)
        {
            if(QuestManager.Instance.GetQuestStatusbyNPC(def.ID) == NPCQuestStatus.None)
                return false;
            Dictionary<NPCQuestStatus, List<Quest>> npcquets = QuestManager.Instance.npcQuests[def.ID];
            
            return QuestManager.Instance.OpenNPCQuest(def.ID);
        }

        internal void UpdateNpcPosition(int npc, Vector3 pos)
        {
            this.NpcPos[npc] = pos;
        }
        internal Vector3 GetNpcPosition(int npc)
        {
            return this.NpcPos[npc];
        }
    }
}
