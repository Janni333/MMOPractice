using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class UIManager : Singleton<UIManager>
{
    class UIElement
    {
        public string Resource;
        public bool Cache;
        public GameObject instance;
    }

    private Dictionary<Type, UIElement> UIelements = new Dictionary<Type, UIElement>();
    public UIManager()
    {
        this.UIelements.Add(typeof(UIBag), new UIElement() { Resource = "UI/UIBag", Cache = false });
        this.UIelements.Add(typeof(UICharEquip), new UIElement() { Resource = "UI/UICharEquip", Cache = false });
        this.UIelements.Add(typeof(UIShop), new UIElement() { Resource = "UI/UIShop", Cache = false });
        this.UIelements.Add(typeof(UIQuest), new UIElement() { Resource = "UI/UIQuest", Cache = false });
        this.UIelements.Add(typeof(UIQuestDialog), new UIElement() { Resource = "UI/UIQuestDialog", Cache = false });
        this.UIelements.Add(typeof(UIFriend), new UIElement() { Resource = "UI/UIFriend", Cache = false });
        this.UIelements.Add(typeof(UIGuild), new UIElement() { Resource = "UI/Guild/UIGuild", Cache = false });
        this.UIelements.Add(typeof(UIGuildList), new UIElement() { Resource = "UI/Guild/UIGuildList", Cache = false });
        this.UIelements.Add(typeof(UIGuildPopNoGuild), new UIElement() { Resource = "UI/Guild/UIGuildPopNoGuild", Cache = false });
        this.UIelements.Add(typeof(UIGuildPopCreate), new UIElement() { Resource = "UI/Guild/UIGuildPopCreate", Cache = false });
        this.UIelements.Add(typeof(UIGuildApplyList), new UIElement() { Resource = "UI/Guild/UIGuildApplyList", Cache = false });
        this.UIelements.Add(typeof(UISetting), new UIElement() { Resource = "UI/UISetting", Cache = false });
        this.UIelements.Add(typeof(UIPopCharMenu), new UIElement() { Resource = "UI/UIPopCharMenu", Cache = false });
        this.UIelements.Add(typeof(UIRide), new UIElement() { Resource = "UI/UIRide", Cache = false });
        this.UIelements.Add(typeof(UISystemConfig), new UIElement() { Resource = "UI/UISystemConfig", Cache = false });
        this.UIelements.Add(typeof(UISkill), new UIElement() { Resource = "UI/UISkill", Cache = false });
    }

    void Start()
    { }
    void Update()
    { }

    public T Show<T>()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
        Type uitype = typeof(T);
        if (this.UIelements.ContainsKey(uitype))
        {
            UIElement element = this.UIelements[uitype];
            if (element.instance != null)
            {
                element.instance.SetActive(true);
            }
            else 
            {
                UnityEngine.Object prefab = Resloader.Load<UnityEngine.Object>(element.Resource);
                if (prefab == null)
                {
                    return default(T);
                }
                else 
                {
                    element.instance = (GameObject)GameObject.Instantiate(prefab);
                }
            }
            return element.instance.GetComponent<T>();
        }

        return default(T);
    }

    public void Close(Type type)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
        if (this.UIelements.ContainsKey(type))
        {
            UIElement element = this.UIelements[type];
            if (element.Cache == true)
            {
                element.instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(element.instance);
                element.instance = null;
            }
        }
    }

    public void Close<T>()
    {
        this.Close(typeof(T));
    }
}
