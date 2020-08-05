using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGrid : MonoBehaviour
{
    public Transform firstOffsetCamera;
    public Transform offsetCamera;

    public RawImage mapImage;
    public RectTransform mapRect;

    public RectTransform horizontalLine;
    public RectTransform verticalLine;

    public int horizontalLinesCount = 5;
    public int verticalLinesCount = 6;

    public int horizontalFirstOffset = 3;
    public float horizontalDeltaFirstOffset = 0f;
    public int horizontalOffset = 3;
    public float horizontalDeltaOffset = 0f;

    public int verticalFirstOffset = 3;
    public float verticalDeltaFirstOffset = 0f;
    public int verticalOffset = 3;
    public float verticalDeltaOffset = 0f;

    public Slider horFirstSlider;
    public Slider horDeltaFirstSlider;
    public Slider horSlider;
    public Slider horDeltaSlider;
    public Slider vertFirstSlider;
    public Slider vertDeltaFirstSlider;
    public Slider vertSlider;
    public Slider vertDeltaSlider;

    RectTransform[] horizontaLines;
    RectTransform[] verticalLines;

    public Action<MapInfoHelper> CreateMap;
    private void Awake()
    {
        horizontaLines = new RectTransform[horizontalLinesCount];
        horizontaLines[0] = horizontalLine;

        Vector2 sizeDelta = horizontalLine.sizeDelta;
        sizeDelta.x = mapRect.sizeDelta.x;
        horizontalLine.sizeDelta = sizeDelta;

        for (int i = 1; i < horizontalLinesCount; i++)
        {
            horizontaLines[i] = Instantiate(horizontalLine, horizontalLine.transform.parent); 
        }
        verticalLines = new RectTransform[verticalLinesCount];
        verticalLines[0] = verticalLine; 

        sizeDelta = verticalLine.sizeDelta;
        sizeDelta.y = mapRect.sizeDelta.y;
        verticalLine.sizeDelta = sizeDelta;

        for (int i = 1; i < verticalLinesCount; i++)
        {
            verticalLines[i] = Instantiate(verticalLine, verticalLine.transform.parent);
        }

        horFirstSlider.value = horizontalFirstOffset;
        horDeltaFirstSlider.value = horizontalDeltaFirstOffset;
        horSlider.value = horizontalOffset;
        horDeltaSlider.value = horizontalDeltaOffset;

        vertFirstSlider.value = verticalFirstOffset;
        vertDeltaFirstSlider.value = verticalDeltaFirstOffset;
        vertSlider.value = verticalOffset;
        vertDeltaSlider.value = verticalDeltaOffset;
    }

    void RepositionLines(bool isHorizontal)
    {
        Vector3 position;
        if (isHorizontal)
        {
            for (int i = 0; i < horizontalLinesCount; i++)
            {
                position = new Vector3(0, -(horizontalOffset + horizontalDeltaOffset) * i - (horizontalFirstOffset + horizontalDeltaFirstOffset));
                horizontaLines[i].anchoredPosition = position;
            }
        }
        else
        {
            for (int i = 0; i < verticalLinesCount; i++)
            {
                position = new Vector3((verticalOffset + verticalDeltaOffset) * i + verticalFirstOffset + verticalDeltaFirstOffset, 0);
                verticalLines[i].anchoredPosition = position;
            }
        }
        position = firstOffsetCamera.position;
        position.x = verticalLines[0].position.x;
        position.y = horizontaLines[0].position.y;
        firstOffsetCamera.position = position;

        position = offsetCamera.position;
        position.x = verticalLines[1].position.x;
        position.y = horizontaLines[1].position.y;
        offsetCamera.position = position;
    }

    public void HorizontalFirstSliderChange(float newValue)
    {
        horizontalFirstOffset = (int)newValue;
        RepositionLines(true);
    }
    public void HorizontalDeltaFirstSliderChange(float newValue)
    {
        horizontalDeltaFirstOffset = newValue;
        RepositionLines(true);
    }
    public void HorizontalSliderChange(float newValue)
    {
        horizontalOffset = (int)newValue;
        RepositionLines(true);
    }
    public void HorizontalDeltaSliderChange(float newValue)
    {
        horizontalDeltaOffset = newValue;
        RepositionLines(true);
    }

    public void VerticalFirstSliderChange(float newValue)
    {
        verticalFirstOffset = (int)newValue;
        RepositionLines(false);
    }
    public void VerticalDeltaFirstSliderChange(float newValue)
    {
        verticalDeltaFirstOffset = newValue;
        RepositionLines(false);
    }
    public void VerticalSliderChange(float newValue)
    {
        verticalOffset = (int)newValue;
        RepositionLines(false);
    }
    public void VerticalDeltaSliderChange(float newValue)
    {
        verticalDeltaOffset = newValue;
        RepositionLines(false);
    }

    public void OnCreateMapButtonClick()
    {
        MapInfoHelper info = new MapInfoHelper();
        info.mapImage = (Texture2D) mapImage.mainTexture;

        info.xMapSize = (int)mapRect.sizeDelta.x;
        info.yMapSize = (int)mapRect.sizeDelta.y;

        info.xOffsetMap = (int)mapRect.anchoredPosition.x;
        info.yOffsetMap = (int)mapRect.anchoredPosition.y;

        info.xMapEnd = (int)verticalLines[verticalLinesCount - 1].anchoredPosition.x;
        info.yMapEnd = (int)horizontaLines[0].anchoredPosition.y;


        info.xMapStart = (int)verticalLines[0].position.x;
        info.yMapStart = (int)horizontaLines[horizontalLinesCount - 1].position.y;

        info.xfirstChunkOffset = verticalFirstOffset;
        info.yfirstChunkOffset = horizontalFirstOffset;

        info.xChunkOffset = verticalOffset;
        info.yChunkOffset = horizontalOffset;

        CreateMap.Invoke(info);
    }
}
