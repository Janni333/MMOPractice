using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class UIBagButton : MonoBehaviour
{
    public UIBagView View;
    public int Index;

    Button thisButton;
    Image thisImage;

    Color normalColor = new Color(1f, 1f, 1f);
    Color selectedColor = new Color(1f, 0.6f, 0f);

    public bool isSelected
    {
        set 
        {
            this.OnSelected(value);
        }
    }

    void Start()
    {
        thisButton = this.GetComponent<Button>();
        thisImage = thisButton.GetComponent<Image>();
    }
    public void OnClick()
    {
        this.View.Selected(this.Index);
    }

    public void OnSelected(bool selected)
    {
        thisImage.color = selected ? selectedColor : normalColor;
    }
}
