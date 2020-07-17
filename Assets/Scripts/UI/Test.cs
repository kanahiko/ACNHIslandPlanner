using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 blash = transform.InverseTransformPoint(eventData.position) / 3;
        Debug.Log($"{(int)blash.x} {(int)blash.y}");
    }
}
