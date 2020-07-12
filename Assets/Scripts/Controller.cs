﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public UIController controller;
    
    public Vector2 playerFieldSizeMin = new Vector2(0, 0);
    public Vector2 playerFieldSizeMax = new Vector2(0, -3.5f);
    public Vector3 scrollOffset = new Vector3(0, 0, 0);

    public Vector2 scrollMinMax = new Vector2(-1, 20);
    [Range(1,20)]
    public float speedMultiplier = 1;
    [Range(1,20)]
    public float mouseScrollMultiplier = 1;

    public Camera mainCamera;
    public Transform playerCamera;
    public Transform playerZoomCamera;
    
    [Range(0, 10)]
    public float speed;
    [Range(0, 10)]
    public float scrollSpeed;

    public ToolType currentTool = ToolType.Null;

    public GameObject selectionCube;
    
    public Text coordinateText;

    public float timeBetweenClicks = 0.1f;

    float currentScroll = 0;
    Vector3 currentPosition;

    bool isCameraParallelToGround = false;
    float cameraDegrees = 45;
    bool cameraChanged;

    int currentBlockX = -1;
    int currentBlockY = -1;

    private int prevBlockX = -1;
    private int prevBlockY = -1;

    private ToolMode construct = ToolMode.None;
    
    Vector2 mousePos;
    Vector2 moveVector;
    float currentSpeedMultiplier = 1;

    private float scroll;

    Controls controls;

    private float timeElapsedSinceClick;

    private void Awake()
    {
        //Application.targetFrameRate = 60;
        controls = new Controls();
        controls.MapControl.ShowGrid.performed += ctx => ShowGrid();
        controls.MapControl.ShowElevation.performed += ctx => ShowElevation();
        controls.MapControl.CameraTilt.performed += ctx => HandleTilt();

        controls.MapControl.CameraMove.performed += ctx => moveVector = ctx.ReadValue<Vector2>();
        controls.MapControl.CameraMove.canceled += ctx => moveVector = Vector2.zero;

        controls.MapControl.ScrollButton.performed += ctx => scroll = ctx.ReadValue<float>();
        controls.MapControl.ScrollButton.canceled += ctx => scroll = 0;
        controls.MapControl.ScrollMouse.performed += ctx => scroll = (ctx.ReadValue<Vector2>().y > 0 ? 1 : -1)*mouseScrollMultiplier;
        controls.MapControl.ScrollMouse.canceled += ctx => scroll = 0;

        controls.MapControl.SpeedChange.performed += ctx => currentSpeedMultiplier = speedMultiplier;
        controls.MapControl.SpeedChange.canceled += ctx => currentSpeedMultiplier = 1;

        //placing items controls
        controls.MapControl.WaterscapingTool.performed += ctx => controller.SetToolButton(ToolType.Waterscaping);
        controls.MapControl.CliffConstructionTool.performed += ctx => controller.SetToolButton(ToolType.CliffConstruction);
        controls.MapControl.PathPermitTool.performed += ctx => controller.SetToolButton(ToolType.PathPermit);
        controls.MapControl.FenceTool.performed += ctx => controller.SetToolButton(ToolType.FenceBuilding);

        controls.MapControl.PlaceItem.performed += ctx =>
        {
            StartConstruction();
            PlaceItem();
        };
        controls.MapControl.PlaceItem.canceled += ctx => EndConstruction();
        controls.MapControl.RemoveItem.performed += ctx =>
        {
            StartConstruction();
            RemoveItem();
        };
        controls.MapControl.RemoveItem.canceled += ctx => EndConstruction();
        controls.MapControl.SampleItem.performed += ctx => SampleItem();
        
        construct = ToolMode.None;

    }

    void Start()
    {
        playerFieldSizeMin.y = playerFieldSizeMax.y - MapHolder.height;
        playerFieldSizeMax.x = MapHolder.width;

        currentPosition = playerZoomCamera.localPosition;
        cameraDegrees = playerZoomCamera.localRotation.eulerAngles.x;

    }
    
    void Update()
    {
        HandleInput(moveVector);
        HandleScrolling(scroll);

        if (construct != ToolMode.None && !(currentBlockX == prevBlockX && currentBlockY == prevBlockY))
        {
            if (construct == ToolMode.Add)
            {
                PlaceItem();
            }
            else
            {
                RemoveItem();
            }

            currentBlockX = -1;
            currentBlockY = -1;
        }
    }

    private void FixedUpdate()
    {
        if (cameraChanged || Vector3.SqrMagnitude(Pointer.current.position.ReadValue() - mousePos) > 10f)
        {
            mousePos = Pointer.current.position.ReadValue();
            TrackMousePositionOnGrid();
            cameraChanged = false;
        }
    }

    void ShowGrid()
    {
        MapHolder.mapPrefab.ShowGrid();
    }
    void ShowElevation()
    {
        MapHolder.mapPrefab.ShowElevation();
    }

    void HandleInput(Vector2 move)
    {
        Vector3 currentPosition = playerCamera.localPosition;
        if (Mathf.Abs(move.x) >= 0.1f)
        {
            currentPosition.x += move.x * (speed * currentSpeedMultiplier) *Time.deltaTime;
            currentPosition.x = Mathf.Clamp(currentPosition.x, playerFieldSizeMin.x + MapHolder.offset.x, playerFieldSizeMax.x + MapHolder.offset.x);
            cameraChanged = true;
        }

        if (Mathf.Abs(move.y) >= 0.1f)
        {
            currentPosition.z += move.y * (speed * currentSpeedMultiplier) *Time.deltaTime;
            currentPosition.z = Mathf.Clamp(currentPosition.z, playerFieldSizeMin.y + MapHolder.offset.z, playerFieldSizeMax.y + MapHolder.offset.z);
            cameraChanged = true;
        }

        playerCamera.localPosition = currentPosition;
    }

    void HandleTilt()
    {
        if (isCameraParallelToGround)
        {
            playerZoomCamera.localPosition = Vector3.zero;
            playerZoomCamera.localRotation = Quaternion.Euler(cameraDegrees, 0, 0);
        }
        else
        {
            float distanceToGround = Mathf.Sqrt((playerCamera.position.y * playerCamera.position.y) * 2) -
                                     playerCamera.position.y;
            playerZoomCamera.localPosition = new Vector3(0, distanceToGround, playerCamera.position.y);
            playerZoomCamera.localRotation = Quaternion.Euler(90, 0, 0);
        }

        currentPosition = playerZoomCamera.localPosition;

        cameraChanged = true;
        isCameraParallelToGround = !isCameraParallelToGround;

        var normal = playerZoomCamera.forward;
        scrollOffset = normal * currentScroll;
        playerZoomCamera.localPosition = currentPosition +scrollOffset;
    }

    void HandleScrolling(float scroll)
    {
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentScroll += scroll * scrollSpeed * currentSpeedMultiplier *Time.deltaTime;
            currentScroll = Mathf.Clamp(currentScroll, scrollMinMax.x, scrollMinMax.y);

            var normal = playerZoomCamera.forward;
            scrollOffset = normal * currentScroll;
            playerZoomCamera.localPosition = currentPosition +scrollOffset;
            cameraChanged = true;
        }
    }

    void TrackMousePositionOnGrid()
    {
        if (!EventSystem.current.IsPointerOverGameObject(-1))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(mousePos), out hit, 50,256))
            {
                //Vector3 newPos = mousePosition;
                selectionCube.transform.localPosition = hit.collider.transform.position;

                currentBlockX = (int) (hit.collider.transform.position.x - MapHolder.offset.x);
                currentBlockY = (int) (hit.collider.transform.position.z - MapHolder.offset.z);

                //mousePos.position = mousePoint;
                coordinateText.text = $"{currentBlockX} {-currentBlockY}";
            }
            else
            {
                selectionCube.transform.localPosition = new Vector3(20,0,20);
                currentBlockX = -1;
                currentBlockY = -1;
            }
        }
        else
        {
            selectionCube.transform.localPosition = new Vector3(20,0,20);
            currentBlockX = -1;
            currentBlockY = -1;
        }
    }

    void StartConstruction()
    {
        if (construct == ToolMode.None && currentBlockX != -1)
        {
            TerrainBuilder.StartConstruction(MapHolder.tiles[currentBlockX,Mathf.Abs(currentBlockY)].elevation);
        }
    }

    void EndConstruction()
    {
        //Debug.Log($"construction ended {currentBlockX} {currentBlockY}");
        construct = ToolMode.None;
        TerrainBuilder.EndConstruction();
    }

    void PlaceItem()
    {
        if (currentTool != ToolType.Null && currentBlockX != -1)
        {
            if (construct == ToolMode.None)
            {
                construct = ToolMode.Add;
                //Debug.Log($"construction started {construct} {MapHolder.tiles[currentBlockX,Mathf.Abs(currentBlockY)].elevation}");
            }
            else
            {
                if (construct != ToolMode.Add)
                {
                    return;
                }
            }
            switch (currentTool)
            {
                case ToolType.Waterscaping:
                    TerrainBuilder.ChangeTile(TileType.Water, currentBlockX,Mathf.Abs(currentBlockY));
                    break;
                case ToolType.CliffConstruction:
                    TerrainBuilder.ChangeTile(TileType.Cliff, currentBlockX, Mathf.Abs(currentBlockY));
                    break;
                case ToolType.PathPermit:
                    TerrainBuilder.ChangeTile(TileType.Path, currentBlockX, Mathf.Abs(currentBlockY));
                    break;
                case ToolType.FenceBuilding:
                    break;
                case ToolType.BridgeMarkUp:
                    break;
                case ToolType.InclineMarkUp:
                    break;
                case ToolType.BushPlanting:
                    break;
                case ToolType.TreePlanting:
                    break;
                case ToolType.FlowerPlanting:
                    break;
                case ToolType.BuildingsMarkUp:
                    break;
            }

            prevBlockX = currentBlockX;
            prevBlockY = currentBlockY;
        }
    }

    private void RemoveItem()
    {
        if (currentTool != ToolType.Null && currentBlockX != -1)
        {
            if (construct == ToolMode.None)
            {
                construct = ToolMode.Remove;
                Debug.Log($"construction started {construct}");
            }
            else
            {
                if (construct != ToolMode.Remove)
                {
                    return;
                }
            }
            switch (currentTool)
            {
                case ToolType.Waterscaping:
                    TerrainBuilder.RemoveTile(TileType.Water, currentBlockX,Mathf.Abs(currentBlockY));
                    break;
                case ToolType.CliffConstruction:
                    TerrainBuilder.RemoveTile(TileType.Cliff, currentBlockX, Mathf.Abs(currentBlockY));
                    break;
                case ToolType.PathPermit:
                    TerrainBuilder.RemoveTile(TileType.Path, currentBlockX, Mathf.Abs(currentBlockY));
                    break;
                case ToolType.FenceBuilding:
                    break;
                case ToolType.BridgeMarkUp:
                    break;
                case ToolType.InclineMarkUp:
                    break;
                case ToolType.BushPlanting:
                    break;
                case ToolType.TreePlanting:
                    break;
                case ToolType.FlowerPlanting:
                    break;
                case ToolType.BuildingsMarkUp:
                    break;
            }
        }
    }

    private void SampleItem()
    {
        if (currentBlockX != -1)
        {
            switch (MapHolder.grid[currentBlockX + Mathf.Abs(currentBlockY)*MapHolder.width])
            {
                case TileType.Null:
                    break;
                case TileType.Water:
                case TileType.WaterDiagonal:
                    currentTool = ToolType.Waterscaping;
                    break;
                case TileType.Path:
                case TileType.PathCurve:
                    currentTool = ToolType.PathPermit;
                    break;
                case TileType.Land:
                case TileType.CliffDiagonal:
                    currentTool = ToolType.CliffConstruction;
                    break;
            }
        }
        else
        {
            currentTool = ToolType.Null;
        }
    }


    public void WaterscapingButtonClick()
    {
        ToolChange(ToolType.Waterscaping);
    }
    
    public void CliffConstructionButtonClick()
    {
        ToolChange(ToolType.CliffConstruction);
    }

    public void PathPermitButtonClick()
    {
        ToolChange(ToolType.PathPermit);
    }

    public void FencePermitButtonClick()
    {
        ToolChange(ToolType.FenceBuilding);
    }
    
    public void ToolChange(ToolType tool)
    {
        currentTool = tool;
    }
    
    

    private void OnEnable()
    {
        controls.MapControl.Enable();
    }

    private void OnDisable()
    {
        controls.MapControl.Disable();
    }

    private void OnApplicationQuit()
    {
        MapHolder.mapPrefab.ResetShaders();
    }
}
