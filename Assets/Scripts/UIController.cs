using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public FancyToggleButton terraFormingToggles;
    public ToggleDictionary toggleDictionary;
    public FancyToggleButton colorToggles;
    public AudioSource click;

    private Transform previousToggle;
    private Transform previousHover;

    public RawImage miniMap;

    public MinimapDecorationsDictionary minimapPinsDictionary;
    public Image cameraPosition;

    public PopUp alertPopUp;

    public Canvas minimapParent;
    public Canvas controlsParent;
    public Canvas tipsParent;
    public TextMeshProUGUI tipsText;

    public static Action ShowPopUp;

    [Multiline]
    public string webGLTipstext = "";
    [Multiline]
    public string winTipsText = "";

    private void Start()
    {
        ShowPopUp = ShowAlert;
        MiniMap.cameraPosition = cameraPosition;
        MiniMap.UpdateMiniMap = SetNewMiniMapTexture;
        MiniMap.CreateMiniMap();
        MiniMap.CreatePins(minimapPinsDictionary);
        
        MiniMap.ratio = new Vector2();
        MiniMap.ratio.x = miniMap.rectTransform.rect.width / (MapHolder.width*MiniMap.pixelSize + MapHolder.mapPrefab.miniMapOffset.x * 2);
        MiniMap.ratio.y = miniMap.rectTransform.rect.height / (MapHolder.height*MiniMap.pixelSize + MapHolder.mapPrefab.miniMapOffset.y * 2);

#if UNITY_WEBGL
        tipsText.text = webGLTipstext;
#else
        tipsText.text = winTipsText;
#endif
        //MiniMap.miniMapPosition = miniMap.rectTransform.localPosition;
    }

    void SetNewMiniMapTexture(Texture2D texture)
    {
        miniMap.texture = texture;
    }

    public void SetTerraformingButton(int type)
    {
        if (terraFormingToggles)
        {
            terraFormingToggles.WasClicked(type);
            //toggleDictionary[type].onClick?.Invoke();
        }
    }
    
    public void SetToolButton(ToolType type)
    {
        if (toggleDictionary.ContainsKey(type))
        {
            toggleDictionary[type].WasClicked();
            //toggleDictionary[type].onClick?.Invoke();
        }
    }

    public void SetNewTool(ToolType type, int variation, FlowerColors color)
    {
        if (toggleDictionary.ContainsKey(type))
        {
            toggleDictionary[type].WasClicked(variation);

            if (type == ToolType.FlowerPlanting)
            {

                colorToggles.WasClicked((int)color);
            }
        }
    }
    
    public void SetColorButton()
    {
        if (colorToggles)
        {
            colorToggles.WasClicked();
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
    
    public void OnMiniMapRightClick(Transform miniMapTransform)
    {
        if (miniMapTransform.transform.localScale.x > 0.9f)
        {
            miniMapTransform.transform.localScale = Vector3.one * 0.5f;
        }else
        {
            miniMapTransform.transform.localScale = Vector3.one;
            
        }
    }

    public void HideTips()
    {
        tipsParent.enabled = !tipsParent.enabled;
    }
    
    public void HideControls()
    {
        controlsParent.enabled = !controlsParent.enabled;
    }
    
    public void HideMinimap()
    {
        minimapParent.enabled = !minimapParent.enabled;
    }

    public void ShowAlert()
    {
        alertPopUp.Enable();
    }
}


[System.Serializable]
public class ToggleDictionary : SerializableDictionaryBase<ToolType, FancyToggleButton> { }
