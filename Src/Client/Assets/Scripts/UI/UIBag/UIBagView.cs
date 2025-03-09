using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class UIBagView : MonoBehaviour
{
    public List<UIBagButton> Buttons = new List<UIBagButton>();
    public List<GameObject> Pages = new List<GameObject>();

    public int curIndex
    {
        set
        {
            this.Selected(value);
        }
    }

    void Start()
    {
        for (int i = 0; i < this.Buttons.Count; i++)
        {
            Buttons[i].Index = i;
            Buttons[i].View = this;
        }
        this.curIndex = 0;
    }

    public void Selected(int idx)
    {   
        //page
        for (int i = 0; i < this.Pages.Count; i++)
        {
            this.Pages[i].gameObject.SetActive(i == idx);
            this.Buttons[i].isSelected = (i == idx);
        }
    }
}
