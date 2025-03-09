using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListView : MonoBehaviour
{
    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        public ListView itemOwner;
        private bool isselected;
        public bool isSelected
        {
            get { return isselected; }
            set
            {
                isselected = value;
                this.OnSelected(value);
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            //我的写法
            /*
            this.isSelected = true;
            this.itemOwner.listSelect = this;
            */

            //严谨写法
            if (!this.isSelected)
            {
                this.isSelected = true;
            }
            if (itemOwner != null && itemOwner.listSelect != this)
            {
                itemOwner.listSelect = this;
            }
        }

        public virtual void OnSelected(bool selected)
        { 
        }
    }

    public UnityAction<ListViewItem> OnSelectItem;
    public List<ListViewItem> listItems = new List<ListViewItem>();
    private ListViewItem listselect;
    public ListViewItem listSelect
    {
        get { return listselect; }
        private set
        {
            if (listselect != null && listselect != value)
            {
                listselect.isSelected = false;
            }
            listselect = value;
            if (this.OnSelectItem != null)
            {
                this.OnSelectItem.Invoke((ListViewItem)value);
            }
        }
    }

    public void AddItem(ListViewItem item)
    {
        item.itemOwner = this;
        this.listItems.Add(item);
    }

    public void RemoveAll()
    {
        foreach (var item in listItems)
        {
            Destroy(item.gameObject);
        }
        this.listItems.Clear();
    }
}
