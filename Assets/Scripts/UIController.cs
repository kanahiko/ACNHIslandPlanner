using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Vector3 buttonClickedSize;
    public Vector3 buttonHoverSize;

    public ToggleDictionary toggleDictionary;

    public Toggle CliffToolButton;
    public Toggle WaterToolButton;
    public Toggle PathToolButton;
    public Toggle FenceToolButton;
    public Toggle BushToolButton;
    public Toggle TreeToolButton;

    public AudioSource click;

    public List<UIOnHover> hoveredUI;

    private Transform previousToggle;
    private Transform previousHover;

    public RawImage miniMap;

    public MinimapDecorationsDictionary minimapPinsDictionary;

    private void Awake()
    {
        if (hoveredUI != null)
        {
            foreach (var hover in hoveredUI)
            {
                hover.PlayHoverSound = PlaySelectSound;
                hover.EnlargeButton = EnlargeButton;
                hover.EnlargeHoverButton = EnlargeHoverButton;
                hover.ScaleHoverButton = ScaleHoverButton;
            }
        }        
    }

    private void Start()
    {
        MiniMap.UpdateMiniMap = SetNewMiniMapTexture;
        MiniMap.CreateMiniMap();
        MiniMap.CreatePins(minimapPinsDictionary);
    }

    void SetNewMiniMapTexture(Texture2D texture)
    {
        miniMap.texture = texture;
    }

    public void SetToolButton(ToolType type)
    {
        if (toggleDictionary.ContainsKey(type))
        {
            toggleDictionary[type].isOn = true;
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
    
    
    public void EnlargeHoverButton(Transform rect)
    {
        if (previousHover != null)
        {
            previousHover.localScale = Vector3.one;
        }
        previousHover = rect;

        rect.localScale =buttonHoverSize;
        
    }

    public void EnlargeButton(Transform rect)
    {
        if (previousToggle != null)
        {
            previousToggle.localScale = Vector3.one;
        }
        
        previousToggle = rect;

        rect.localScale = buttonClickedSize;
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
public class ToggleDictionary : SerializableDictionaryBase<ToolType, Toggle> { }
