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
    //private FancyToggleButton firstChildButton;

    private List<FancyToggleButton> children;

    private int currentButton = -1;
    
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
            childrenToggleGroup.buttonWasClicked = button => currentButton = children.IndexOf(button);
            children = new List<FancyToggleButton>();
            for (int i = 0; i < childrenToggleGroup.transform.childCount; i++)
            {
                children.Add(childrenToggleGroup.transform.GetChild(i).GetComponent<FancyToggleButton>());
            }
        }

        currentButton = -1;
        base.OnEnable();
    }

    public void WasClicked()
    {
        if (isOn && childrenToggleGroup)
        {
            currentButton++;
            if (currentButton >= children.Count)
            {
                currentButton = 0;
            }
                
            children[currentButton].onClick?.Invoke();
        }
        else
        {
            onClick?.Invoke();
        }
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
                        currentButton = children.IndexOf(previousButton);
                        previousButton.onClick?.Invoke();
                        previousButton = null;
                    }
                    else
                    {
                        if (children.Count != 0)
                        {
                            children[0].onClick?.Invoke();
                            currentButton = 0;
                        }
                    }
                    buttonsGroup.enabled = true;
                }
                backgroundImage.color = pressedColor;
                graphic.enabled = true;
            }
            else
            {
                if (toggleGroup && !toggleGroup.canTurnOff)
                {
                    isOn = true;
                    return;
                }
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
                //backgroundImage.color = normalColor;
                //graphic.enabled = false;
            }
        }
    }

    public void TurnOffButton()
    {
        isOn =  false;
        if (borderImage)
        {
            if (buttonsGroup)
            {
                foreach (var toggleButton in childrenToggleGroup.activeToggles)
                {
                    previousButton = toggleButton;
                    //currentButton = -1;
                    break;
                }

                childrenToggleGroup.TurnOffToggles();
                buttonsGroup.enabled = false;
            }
            backgroundImage.color = normalColor;
            graphic.enabled = false;
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

