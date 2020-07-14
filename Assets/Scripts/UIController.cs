using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Vector3 buttonClickedSize;
    public Vector3 buttonHoverSize;
    
    public Toggle CliffToolButton;
    public Toggle WaterToolButton;
    public Toggle PathToolButton;
    public Toggle FenceToolButton;
    public Toggle BushToolButton;

    public AudioSource click;

    public List<UIOnHover> hoveredUI;

    private Transform previousToggle;
    private Transform previousHover;

    public RawImage miniMap;

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
    }

    void SetNewMiniMapTexture(Texture2D texture)
    {
        miniMap.texture = texture;
    }

    public void SetToolButton(ToolType type)
    {
        switch (type)
        {
            case ToolType.Waterscaping:
                WaterToolButton.isOn = true;
                break;
            case ToolType.CliffConstruction:
                CliffToolButton.isOn = true;
                break;
            case ToolType.PathPermit:
                PathToolButton.isOn = true;
                break;
            case ToolType.FenceBuilding:
                FenceToolButton.isOn = true;
                break;
            case ToolType.BridgeMarkUp:
                break;
            case ToolType.InclineMarkUp:
                break;
            case ToolType.BushPlanting:
                break;
            case ToolType.TreePlanting:
                break;
            case ToolType.FlowerPlanting:
                break;
            case ToolType.BuildingsMarkUp:
                break;
            case ToolType.Null:
                break;
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
