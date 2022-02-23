using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButtonMenu> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public TabButtonMenu selectedTab;

    public void Subscribe(TabButtonMenu button)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButtonMenu>();
        }

        tabButtons.Add(button);
    }
    public void OnTabEnter(TabButtonMenu button)
    {
        ResetTabs();
        if(selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        } 
        
    }
    public void OnTabExit(TabButtonMenu button)
    {
        ResetTabs();
    }
    public void OnTabSelected(TabButtonMenu button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;
    }
    public void ResetTabs()
    {
        foreach(TabButtonMenu button in tabButtons)
        {
            if(selectedTab!=null && button == selectedTab) { continue; }
            button.background.sprite = tabIdle;
        }
    }
}
