using RotaryHeart.Lib.SerializableDictionary;
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

    public Transform nullPreview;

    private DecorationType currentType;
    Transform currentPreview;

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
                    preview = nullPreview;
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
        if (currentPreview != null)
        {
            currentPreview.position = Util.cullingPosition;
            if (currentType == DecorationType.Fence || currentType == DecorationType.Incline || currentType == DecorationType.Bridge)
            {
                currentPreview.GetChild(0).localRotation = Quaternion.identity;
            }
        }

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
            Quaternion rotate = Quaternion.identity;
            if (rotation == 2)
            {
                rotate = Quaternion.Euler(0,90,0);
            }

            if (rotation == 1)
            {
                rotate = Quaternion.Euler(0,45,0);
            }

            if (rotation == 3)
            {
                rotate = Quaternion.Euler(0,-45,0); 
            }
            currentPreview.localPosition = new Vector3(Util.bridgeRotationsOffset[rotation].x,0,Util.bridgeRotationsOffset[rotation].y);
            currentPreview.GetChild(0).localRotation = rotate;
        }
    }

    public void FollowMousePosition(Vector3 position)
    {
        previewCursor.position = position;
    }
}


[System.Serializable]
public class BuildingsPreviewDictionary : SerializableDictionaryBase<DecorationType, Transform> { }
