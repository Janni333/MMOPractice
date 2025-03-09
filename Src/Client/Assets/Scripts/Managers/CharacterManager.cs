using Assets.Scripts.Model;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public UnityAction<Character> OnAddCharacter;
        public UnityAction<Character> OnRemoveCharacter;

        //管理器字典
        public Dictionary<int, Character> ClientCharacters = new Dictionary<int, Character>();

        //构造 析构 初始化
        public CharacterManager()
        { 
        }
        public void Dispose()
        {
        }
        public void Init()
        {
        }

        public void Clear()
        {
            this.ClientCharacters.Clear();
        }

        public Character Add(NCharacterInfo NInfo)
        {
            Character newCha = new Character(NInfo);
            this.ClientCharacters[newCha.entityId] = newCha;
            EntityManager.Instance.AddEntity(newCha);
            if(this.OnAddCharacter != null)
            {
                this.OnAddCharacter(newCha);
            }
            if (newCha.entityId == User.Instance.currentCharacterInfo.EntityId)
            {
                User.Instance.currentCharacter = newCha;
            }
            return newCha;
        }

        public void Remove(int entityId)
        {
            if (ClientCharacters.ContainsKey(entityId))
            {
                if (this.OnRemoveCharacter != null)
                {
                    this.OnRemoveCharacter(ClientCharacters[entityId]);
                }
                this.ClientCharacters.Remove(entityId);
            }
        }

        public Character GetCharacter(int id)
        {
            Character character;
            this.ClientCharacters.TryGetValue(id, out character);
            return character;   
        }
    }
}
