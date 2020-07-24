﻿using RotaryHeart.Lib.SerializableDictionary;
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

    public MinimapDecorationsDictionary minimapPinsDictionary;
    public Image cameraPosition;

    private void Start()
    {
        MiniMap.cameraPosition = cameraPosition;
        MiniMap.UpdateMiniMap = SetNewMiniMapTexture;
        MiniMap.CreateMiniMap();
        MiniMap.CreatePins(minimapPinsDictionary);
        
        MiniMap.ratio = new Vector2();
        MiniMap.ratio.x = miniMap.rectTransform.rect.width / (MapHolder.width*MiniMap.pixelSize + MapHolder.mapPrefab.miniMapOffset.x * 2);
        MiniMap.ratio.y = miniMap.rectTransform.rect.height / (MapHolder.height*MiniMap.pixelSize + MapHolder.mapPrefab.miniMapOffset.y * 2);

        //MiniMap.miniMapPosition = miniMap.rectTransform.localPosition;
    }

    void SetNewMiniMapTexture(Texture2D texture)
    {
        miniMap.texture = texture;
    }

    public void SetToolButton(ToolType type)
    {
        if (toggleDictionary.ContainsKey(type))
        {
            toggleDictionary[type].WasClicked();
            //toggleDictionary[type].onClick?.Invoke();
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

    public void OnMiniMapClick(Transform miniMapTransform)
    {
        if (miniMapTransform.transform.localScale.x < 1.1f)
        {
            miniMapTransform.transform.localScale = Vector3.one * 1.5f;
        }else
        {
            miniMapTransform.transform.localScale = Vector3.one;
            
        }
    }
}


[System.Serializable]
public class ToggleDictionary : SerializableDictionaryBase<ToolType, FancyToggleButton> { }
