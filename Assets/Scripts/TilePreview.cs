﻿using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePreview : MonoBehaviour
{
    public Transform previewCursor;

    public BuildingsPreviewDictionary previewBuilding;
    public List<Transform> previewFlora;
    public List<Transform> previewTree;
    public List<Transform> previewFence;
    public List<Transform> previewIncline;
    public List<BridgeSizeList> previewBridge;
    public List<Transform> previewFlower;

    public Transform nullPreview;

    private DecorationType currentType;
    Transform currentPreview;

    int currentRotation = 0;

    int currentColumn = -1;
    int currentRow = -1;

    public void ChangeTile(DecorationType type = DecorationType.Null, byte variation = 0)
    {
        Transform preview = null;
        switch (type)
        {
            case DecorationType.Null:
                preview = nullPreview;
                break;
            case DecorationType.Fence:
                if (previewFence.Count < variation + 1)
                {
                    preview = previewFence[previewFence.Count - 1];
                }
                else
                {
                    preview = previewFence[variation];
                }

                break;
            case DecorationType.Flower:
                if (previewFlower.Count < variation + 1)
                {
                    preview = previewFlower[previewFlower.Count - 1];
                }else
                {
                    preview = previewFlower[variation];
                }
                break;
            case DecorationType.Flora:
                if (previewFlora.Count < variation + 1)
                {
                    preview = previewFlora[previewFlora.Count - 1];
                }else
                {
                    preview = previewFlora[variation];
                }
                break;
            case DecorationType.Tree:
                if (previewTree.Count < variation + 1)
                {
                    preview = previewTree[previewTree.Count - 1];
                }
                else
                {
                    preview = previewTree[variation];
                }
                break;
            case DecorationType.Incline:
                
                if (previewIncline.Count < variation + 1)
                {
                    preview = previewIncline[0];
                }
                else
                {
                    preview = previewIncline[variation];
                }
                break;
            case DecorationType.Bridge:
                if (previewBridge.Count < variation + 1)
                {
                    preview = nullPreview;
                }
                else
                {
                    preview = previewBridge[variation].bridgePrefabs[0].transform;
                }
                break;
            case DecorationType.Plaza:
            case DecorationType.NookShop:
            case DecorationType.Tailors:
            case DecorationType.Museum:
            case DecorationType.PlayerHouse:
            case DecorationType.House:
            case DecorationType.Camp:
            case DecorationType.Building:
            case DecorationType.Rock:
                if (!previewBuilding.ContainsKey(type))
                {
                    preview = nullPreview;
                }
                else
                {
                    preview = previewBuilding[type];
                }
                break;
        }
        //Quaternion rotation = Quaternion.identity;
        if (currentPreview != null)
        {
            currentPreview.position = Util.cullingPosition;
            if (currentType == DecorationType.Fence && type != DecorationType.Fence || 
                currentType == DecorationType.Incline && type != DecorationType.Incline || 
                currentType == DecorationType.Bridge && type != DecorationType.Bridge)
            {
                currentPreview.GetChild(0).localRotation = Quaternion.identity;
            }
        }
        currentRotation = 0;
        preview.localPosition = Vector3.zero;
        currentPreview = preview;
        currentType = type;
    }
    
    public void ChangeTileRotation(int rotation, DecorationType type)
    {
        if (type == DecorationType.Fence)
        {
            currentPreview.GetChild(0).localRotation = rotation == 0 ? Quaternion.identity : Quaternion.Euler(0, 90, 0);

        }
        
        if (type == DecorationType.Incline)
        {
            currentPreview.localPosition = new Vector3(Util.inclineRotationsOffset[rotation].x,0,Util.inclineRotationsOffset[rotation].y);
            currentPreview.GetChild(0).localRotation = Quaternion.Euler(0, 90 * rotation, 0);
        }
        
        if (type == DecorationType.Bridge)
        {
            Quaternion rotate = Util.bridgeRotations[rotation];
            currentPreview.localPosition = new Vector3(Util.bridgeRotationsOffset[rotation].x,0,Util.bridgeRotationsOffset[rotation].y);
            currentPreview.GetChild(0).localRotation = rotate;
        }
        currentRotation = rotation;
    }

    public void FollowMousePosition(Vector3 position, int column, int row)
    {
        previewCursor.position = position;

        if (currentColumn == column && currentRow == row)
        {
            return;
        }
        currentColumn = column;
        currentRow = row;

        if (currentType == DecorationType.Bridge && currentRotation != 0 && currentRotation != 2 && column != -1)
        {
            //diagonal water offset 0.4,0,-0.4 for rotation 3
            //diagonal water offset -0.4,0,-0.4 for rotation 1
            int size = BuildersController.CheckBridgeSize(column, row, currentRotation);
            Quaternion rotation = Quaternion.identity;
            Vector3 rotatedPosition = Vector3.zero;
            if (currentPreview != null)
            {
                rotation = currentPreview.GetChild(0).localRotation;

                currentPreview.position = Util.cullingPosition;
                currentPreview.GetChild(0).localRotation = Quaternion.identity;
            }
            var preview = previewBridge[0].bridgePrefabs[size - 3].transform;
            rotatedPosition = new Vector3(Util.bridgeRotationsOffset[currentRotation].x, 0, Util.bridgeRotationsOffset[currentRotation].y);

            if (MapHolder.tiles[column,row].type == TileType.WaterDiagonal)
            {
                rotatedPosition.x += Util.bridgeAdditionalRotationsOffset[currentRotation].x;
                rotatedPosition.z += Util.bridgeAdditionalRotationsOffset[currentRotation].y;
            }

            preview.localPosition = rotatedPosition;

            preview.GetChild(0).localRotation = rotation;
            currentPreview = preview;
        }

        if (currentType == DecorationType.Bridge && currentRotation != 1 && currentRotation != 3 && column != -1)
        {
            int size = BuildersController.CheckBridgeSize(column, row, currentRotation);
            Quaternion rotation = Quaternion.identity;
            Vector3 rotatedPosition = Vector3.zero;
            if (currentPreview != null)
            {
                rotatedPosition = currentPreview.localPosition;
                rotation = currentPreview.GetChild(0).localRotation;

                currentPreview.position = Util.cullingPosition;
                currentPreview.GetChild(0).localRotation = Quaternion.identity;
            }
            var preview = previewBridge[0].bridgePrefabs[size - 3].transform;
            preview.localPosition = rotatedPosition;
            preview.GetChild(0).localRotation = rotation;
            currentPreview = preview;
        }
    }
}


[System.Serializable]
public class BuildingsPreviewDictionary : SerializableDictionaryBase<DecorationType, Transform> { }
