using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public ToggleDictionary toggleDictionary;

    public AudioSource click;

    private Transform previousToggle;
    private Transform previousHover;

    public RawImage miniMap;

    private void Awake()
    {      
    }

    private void Start()
    {
        MiniMap.UpdateMiniMap = SetNewMiniMapTexture;
        MiniMap.CreateMiniMap();
    }

    void SetNewMiniMapTexture(Texture2D texture)
    {
        miniMap.texture = texture;
    }

    public void SetToolButton(ToolType type)
    {
        if (toggleDictionary.ContainsKey(type))
        {
            toggleDictionary[type].onClick?.Invoke();
        }
    }
    public void ScaleHoverButton(Transform rect)
    {
        if (previousHover != null)
        {
            previousHover.localScale = Vector3.one;
        }
        previousHover = null;
    }

    public void PlayClickSound()
    {
        click.Stop();
        click.pitch = 0.5f;
        click.Play();
    }

    public void PlaySelectSound()
    {
        click.Stop();
        click.pitch = 2f;
        click.Play();
    }
}


[System.Serializable]
public class ToggleDictionary : SerializableDictionaryBase<ToolType, FancyToggleButton> { }
