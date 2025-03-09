using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 240407
 *  创建和基本逻辑
 */
namespace GameServer.Managers
{
    class CharacterManager: Singleton<CharacterManager>
    {
        //管理器字典
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        //构造
        //初始化
        //析构
        public CharacterManager()
        { }
        public void Init()
        { }
        public void Despose()
        { }

        //清除管理器字典
        public void Clear()
        {
            this.Characters.Clear();
        }

        //创建新实体并加入管理器字典
        public Character Add(CharacterType type, TCharacter Tcha)
        {
            Character newcha = new Character(type, Tcha);
            EntityManager.Instance.Add(Tcha.MapID, newcha);
            newcha.Info.EntityId = newcha.EntityId;
            this.Characters[newcha.ID] = newcha;            //使用的是CharacterBase.Id即entityId
            return newcha;
        }

        //删除管理器字典元素
        public void Remove(int charid)
        {
            if (this.Characters.ContainsKey(charid) && this.Characters[charid] != null)
            {
                this.Characters.Remove(charid);
            }
        }

        //获取管理器字典角色
        public Character GetCharacter(int characterId)
        {
            Character character = null;
            this.Characters.TryGetValue(characterId, out character);
            return character;
        }
    }
}
