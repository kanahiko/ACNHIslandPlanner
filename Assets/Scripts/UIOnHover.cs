using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIOnHover : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public Transform border;
    
    public Action PlayHoverSound;
    public Action<Transform> EnlargeHoverButton;
    public Action<Transform> ScaleHoverButton;
    public Action<Transform> EnlargeButton;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverSound?.Invoke();
        EnlargeHoverButton?.Invoke(transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ScaleHoverButton?.Invoke(transform);
    }

    public void DoThis()
    {
        OnPointerClick();
    }

    public void OnPointerClick()
    {
        EnlargeButton?.Invoke(border);
    }
}
