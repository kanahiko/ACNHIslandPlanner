using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FancyToggleButton : Toggle, IPointerEnterHandler, IPointerExitHandler
{
    public Image backgroundImage;

    public Color normalColor;
    public Color pressedColor;

    public Image borderImage;
    public Vector2 hoverBorderSize = new Vector2(1.2f,1.2f);
    Vector2 borderSize;

    public ToggleGroup toggleGroup;

    public ToggleGroup childrenToggleGroup;
    public Canvas buttonsGroup;

    protected override void OnEnable()
    {
        if (toggleGroup)
        {
            toggleGroup.RegisterToggle(this);
        }
        onValueChanged.AddListener(ToggleChanged);
        if (borderImage)
        {
            borderSize = borderImage.transform.localScale;
        }
        base.OnEnable();
    }

    void ToggleChanged(bool isOn)
    {

        if (borderImage)
        {
            if (isOn)
            {
                if (buttonsGroup)
                { 
                    buttonsGroup.enabled = true;
                }


                    if (toggleGroup)
                {
                    var toggles = toggleGroup.ActiveToggles();

                    foreach (var toggle in toggles)
                    {
                        if (toggle != this)
                            toggle.isOn=false;
                    }
                }
                backgroundImage.color = pressedColor;
            }
            else
            {
                if (buttonsGroup)
                {
                    var toggles = childrenToggleGroup.ActiveToggles();

                    foreach (var toggle in toggles)
                    {
                        if (toggle != this)
                            toggle.isOn = false;
                    }
                    buttonsGroup.enabled = false;
                }
                backgroundImage.color = normalColor;
            }
        }
    }

    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (borderImage)
        { 
            borderImage.transform.localScale = hoverBorderSize;
            backgroundImage.transform.localScale = hoverBorderSize;
            graphic.transform.localScale = hoverBorderSize;
        }

        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (borderImage)
        {
            borderImage.transform.localScale = borderSize;
            backgroundImage.transform.localScale = borderSize;
            graphic.transform.localScale = borderSize;
        }
        base.OnPointerExit(eventData);
    }

}

