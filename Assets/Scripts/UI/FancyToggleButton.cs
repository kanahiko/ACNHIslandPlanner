using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FancyToggleButton : Button
{
    public Image backgroundImage;

    public Color normalColor;
    public Color pressedColor;

    public Image borderImage;
    public Vector2 hoverBorderSize = new Vector2(1.2f,1.2f);
    Vector2 borderSize;

    public ButtonToggleGroup toggleGroup;
    public ButtonToggleGroup childrenToggleGroup;
    public Canvas buttonsGroup;

    public bool isOn = false;

    public Image graphic;
    private FancyToggleButton previousButton;
    private FancyToggleButton firstChildButton;

    protected override void OnEnable()
    {
        toggleGroup?.RegisterToggle(this);
        onClick.AddListener(ToggleChanged);
        if (borderImage)
        {
            borderSize = borderImage.transform.localScale;
        }

        if (backgroundImage)
        {
            backgroundImage.color = isOn ? pressedColor : normalColor;
        }

        if (graphic)
        {
            graphic.enabled = isOn;
        }

        if (childrenToggleGroup)
        {
            firstChildButton = childrenToggleGroup.transform.GetChild(0).GetComponent<FancyToggleButton>();
        }
        
        base.OnEnable();
    }

    public void ToggleChanged()
    {
        isOn = !isOn;
        if (borderImage)
        {
            if (isOn)
            {
                if (buttonsGroup)
                {
                    if (previousButton != null)
                    {
                        previousButton.onClick?.Invoke();
                        previousButton = null;
                    }
                    else
                    {
                        if (firstChildButton)
                        {
                            firstChildButton.onClick?.Invoke();
                        }
                    }
                    buttonsGroup.enabled = true;
                }
                backgroundImage.color = pressedColor;
                graphic.enabled = true;
            }
            else
            {
                if (buttonsGroup)
                {
                    foreach (var toggleButton in childrenToggleGroup.activeToggles)
                    {
                        previousButton = toggleButton;
                        break;
                    }

                    childrenToggleGroup.TurnOffToggles();
                    buttonsGroup.enabled = false;
                }
                backgroundImage.color = normalColor;
                graphic.enabled = false;
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

