﻿using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePreview : MonoBehaviour
{
    public Transform previewCursor;

    public BuildingsPreviewDictionary previewBuilding;
    public List<Transform> previewFlora;
    public List<Transform> previewTree;
    public List<Transform> previewFence;

    public Transform nullPreview;

    private DecorationType currentType;
    Transform currentPreview;

    public void ChangeTile(DecorationType type = DecorationType.Null, int variation = -1)
    {
        Transform preview = null;
        switch (type)
        {
            case DecorationType.Null:
                preview = nullPreview;
                break;
            case DecorationType.Fence:
                preview = previewFence[variation];
                break;
            case DecorationType.Flora:
                preview = previewFlora[variation];
                break;
            case DecorationType.Tree:
                preview = previewTree[variation];
                break;
            case DecorationType.Plaza:
            case DecorationType.NookShop:
            case DecorationType.Tailors:
            case DecorationType.Museum:
            case DecorationType.PlayerHouse:
            case DecorationType.House:
            case DecorationType.Incline:
            case DecorationType.Bridge:
            case DecorationType.Camp:
            case DecorationType.Building:
                preview = previewBuilding[type];
                break;
        }
        if (currentPreview != null)
        {
            currentPreview.position = Util.cullingPosition;
            if (currentType == DecorationType.Fence)
            {
                currentPreview.GetChild(0).localRotation = Quaternion.identity;
            }
        }

        preview.localPosition = Vector3.zero;
        currentPreview = preview;
        currentType = type;
    }
    
    public void ChangeTileRotation(bool isRotated)
    {
        currentPreview.GetChild(0).localRotation = isRotated ? Quaternion.identity : Quaternion.Euler(0, 90, 0);
    }

    public void FollowMousePosition(Vector3 position)
    {
        previewCursor.position = position;
    }
}


[System.Serializable]
public class BuildingsPreviewDictionary : SerializableDictionaryBase<DecorationType, Transform> { }