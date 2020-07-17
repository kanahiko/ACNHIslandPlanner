using System.Collections.Generic;
using UnityEngine;

public class ButtonToggleGroup:MonoBehaviour
{
    private HashSet<FancyToggleButton> registeredToggleButtons;
    public HashSet<FancyToggleButton> activeToggles;

    
    
    void Awake()
    {
        registeredToggleButtons = new HashSet<FancyToggleButton>();
        activeToggles = new HashSet<FancyToggleButton>();
    }
    
    
    public void RegisterToggle(FancyToggleButton toggle)
    {
        toggle.onClick.AddListener(() => CheckButtonValue(toggle));
    }

    public void CheckButtonValue(FancyToggleButton toggle)
    {
        if (activeToggles.Count > 0)
        {
            foreach (var activeToggle in activeToggles)
            {
                if (activeToggle != toggle)
                    activeToggle.ToggleChanged();
            }
            activeToggles.Clear();
        }

        activeToggles.Add(toggle);
    }

    public void TurnOffToggles()
    {
        if (activeToggles.Count > 0)
        {
            foreach (var activeToggle in activeToggles)
            {
                activeToggle.ToggleChanged();
            }
            activeToggles.Clear();
        }
    }
    
}