using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button tabButton;   // The button for the tab
        public GameObject content; // The content panel associated with the tab
    }

    public Tab[] tabs;  // Array to hold all the tabs

    private void Start()
    {
        // Initialize the tabs
        foreach (var tab in tabs)
        {
            tab.tabButton.onClick.AddListener(() => OnTabSelected(tab));
        }

        // Activate the first tab by default
        OnTabSelected(tabs[0]);
    }

    private void OnTabSelected(Tab selectedTab)
    {
        // Deactivate all tabs
        foreach (var tab in tabs)
        {
            tab.content.SetActive(false);
        }

        // Activate the selected tab
        selectedTab.content.SetActive(true);
    }
}
