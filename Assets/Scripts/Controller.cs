using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public BuildersController buildersController;
    public UIController controller;
    public SaveUI saveUI;
    
    public Vector2 playerFieldSizeMin = new Vector2(0, 0);
    public Vector2 playerFieldSizeMax = new Vector2(0, -3.5f);
    public Vector3 scrollOffset = new Vector3(0, 0, 0);

    public Vector2 scrollMinMax = new Vector2(-1, 20);
    [Range(-30,10)]
    public float defaultScroll = 0;
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

    public TilePreview cursor;
#if UNITY_EDITOR
    public Text coordinateText;
#endif
    //public float timeBetweenClicks = 0.1f;

    //checks how much zoomed in or out
    public CameraInfo cameraInfo;
    
    //float currentScroll = 0;
    //of scroll
    Vector3 currentPosition;

    //bool isCameraParallelToGround = false;
    float cameraDegrees = 45;
    bool cameraChanged;
    Vector2 mousePos;
    //moving
    Vector2 moveVector;
    float currentSpeedMultiplier = 1;
    float scroll;

    //tools
    ToolMode construct = ToolMode.None;
    DecorationType currentDecorationTool;
    byte rotation = 0;
    byte variation = 0;
    FlowerColors color = FlowerColors.Red;
    int currentBlockX = -1;
    int currentBlockY = -1;

    int prevBlockX = -1;
    int prevBlockY = -1;

    public static Action<int, int, ToolType, ToolMode, byte, DecorationType, byte, FlowerColors> ChangeTile;
    public static Action<int, int> StartConstructionAction;
    public static Action EndConstructionAction;

    public Action<DecorationType, byte> ChangeCursor;
    public Action<int, DecorationType> ChangeRotationCursor;
    public Action<Vector3, int ,int> ChangeCursorPosition;
    public Action<Vector2, float> ChangeCameraPosition;

    Controls controls;
    public static Action<Dictionary<Vector2Int, List<Vector2Int>>, List<PreDecorationTile>> RebuildMap;
    public static Action RebuildEmptyMap;
    private int colorEnumCount = Enum.GetValues(typeof(FlowerColors)).Length;

    private void Awake()
    {
        Debug.Log(Application.persistentDataPath);

        ChangeCursor = cursor.ChangeTile;
        ChangeCursorPosition = cursor.FollowMousePosition;
        ChangeRotationCursor = cursor.ChangeTileRotation;

        ChangeCursor.Invoke(DecorationType.Null, 0);

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
        controls.MapControl.CliffConstructionTool.performed += ctx => controller.SetTerraformingButton(0);
        controls.MapControl.WaterscapingTool.performed += ctx => controller.SetTerraformingButton(1);
        controls.MapControl.SandPermitTool.performed += ctx => controller.SetTerraformingButton(2);
        controls.MapControl.PathPermitTool.performed += ctx => controller.SetToolButton(ToolType.PathPermit);
        controls.MapControl.FenceTool.performed += ctx => controller.SetToolButton(ToolType.FenceBuilding);
        controls.MapControl.BushTool.performed += ctx => controller.SetToolButton(ToolType.BushPlanting);
        controls.MapControl.TreeTool.performed += ctx => controller.SetToolButton(ToolType.TreePlanting);
        controls.MapControl.BuildingsTool.performed += ctx => controller.SetToolButton(ToolType.BuildingsMarkUp);
        controls.MapControl.InclineTool.performed += ctx => controller.SetToolButton(ToolType.InclineMarkUp);
        controls.MapControl.BridgeTool.performed += ctx => controller.SetToolButton(ToolType.BridgeMarkUp);
        controls.MapControl.FlowersTool.performed += ctx => controller.SetToolButton(ToolType.FlowerPlanting);

        controls.MapControl.PlaceItem.performed += ctx =>
        {
            if (!CanClick())
            {
                return;
            }
            StartConstruction();
            ChangeItem(ToolMode.Add);
        };
        controls.MapControl.PlaceItem.canceled += ctx => EndConstruction();
        controls.MapControl.RemoveItem.performed += ctx =>
        {
            if (!CanClick())
            {
                return;
            }
            StartConstruction();
            ChangeItem(ToolMode.Remove);
        };
        controls.MapControl.RemoveItem.canceled += ctx => EndConstruction();
        controls.MapControl.SampleItem.performed += ctx => {
            if (!CanClick())
            {
                return;
            }
            SampleItem(); 
        };

        controls.MapControl.Rotate.performed += ctx =>
        {
            if (!CanClick())
            {
                return;
            }
            Rotate();
        };
        controls.MapControl.ColorsScroll.performed += ctx =>
        {
            if (!CanClick())
            {
                return;
            }
            ScrollColors();
        };

        controls.MapControl.HideControls.performed += ctx =>
        {
            if (!CanClick())
            {
                return;
            }
            controller.HideControls();
        };
        controls.MapControl.HideMiniMap.performed += ctx =>
        {
            if (!CanClick())
            {
                return;
            }
            controller.HideMinimap();
        };
        controls.MapControl.Tips.performed += ctx =>
        {
            if (!CanClick())
            {
                return;
            }
            controller.HideTips();
        };
#if !UNITY_WEBGL
        controls.MapControl.Fullscreen.performed += ctx => Screen.fullScreen = !Screen.fullScreen;
#endif        
        construct = ToolMode.None;

        cameraInfo = new CameraInfo();
        cameraInfo.position = new Vector3();

        SaveSystem.cameraInfo = cameraInfo;
        SaveSystem.LoadCameraInfo += LoadMap;
        saveUI.Pause = Pause;
    }

    private void ScrollColors()
    {
        if (currentTool != ToolType.FlowerPlanting)
        {
            return;
        }
        int currentColor = (int) color;
        currentColor++;
        if (currentColor >= colorEnumCount)
        {
            currentColor = 0;
        }

        color = (FlowerColors) currentColor;
        controller.SetColorButton();
    }


    void SetToolButton(ToolType type)
    {
        controller.SetToolButton(type);
    }

    void Start()
    {
        playerFieldSizeMin.y = playerFieldSizeMax.y - MapHolder.height;
        playerFieldSizeMax.x = MapHolder.width;

        cameraInfo.currentScroll = defaultScroll;
        currentPosition = playerZoomCamera.localPosition;
        cameraDegrees = playerZoomCamera.localRotation.eulerAngles.x;
        CalculateScroll();

        ChangeCameraPosition = MiniMap.ChangeCameraPosition;
        UpdateCameraPositionOnTheMap();
    }

    bool CanClick()
    {
        return (Application.isFocused && (mousePos.x >= 0 && mousePos.x <= Screen.width || mousePos.y >= 0 && mousePos.y <= Screen.height));
    }
    
    void Update()
    {
        if (!CanClick())
        {
            return;
        }

        HandleInput(moveVector);
        HandleScrolling(scroll);

        if (construct != ToolMode.None && !(currentBlockX == prevBlockX && currentBlockY == prevBlockY))
        {
            ChangeItem(construct);

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

    void Rotate()
    {
        if (currentTool == ToolType.FenceBuilding)
        {
            rotation = (byte)(rotation == 0 ? 1 : 0);
            ChangeRotationCursor?.Invoke(rotation, DecorationType.Fence);
        }

        if (currentTool == ToolType.InclineMarkUp)
        {
            rotation++;
            if (rotation > 3)
            {
                rotation = 0;
            }
            ChangeRotationCursor?.Invoke(rotation, DecorationType.Incline);
        }
        
        if (currentTool == ToolType.BridgeMarkUp)
        {
            rotation++;
            if (rotation > 3)
            {
                rotation = 0;
            }
            ChangeRotationCursor?.Invoke(rotation, DecorationType.Bridge);
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
        bool cameraChangedHere = false;
        Vector3 currentPosition = playerCamera.localPosition;
        if (Mathf.Abs(move.x) >= 0.1f)
        {
            currentPosition.x += move.x * (speed * currentSpeedMultiplier) *Time.deltaTime;
            currentPosition.x = Mathf.Clamp(currentPosition.x, playerFieldSizeMin.x + MapHolder.offset.x, playerFieldSizeMax.x + MapHolder.offset.x);
            cameraChanged = true;
            cameraChangedHere = true;
        }

        if (Mathf.Abs(move.y) >= 0.1f)
        {
            currentPosition.z += move.y * (speed * currentSpeedMultiplier) *Time.deltaTime;
            currentPosition.z = Mathf.Clamp(currentPosition.z, playerFieldSizeMin.y + MapHolder.offset.z, playerFieldSizeMax.y + MapHolder.offset.z);
            cameraChanged = true;
            cameraChangedHere = true;
        }

        playerCamera.localPosition = currentPosition;
        cameraInfo.position = currentPosition;
        if (cameraChangedHere)
        {
            UpdateCameraPositionOnTheMap();
        }

    }

    void UpdateCameraPositionOnTheMap()
    {
        Vector3 position = playerCamera.position - buildersController.offsetTerrain;
        
        ChangeCameraPosition?.Invoke(new Vector2(position.x,Mathf.Abs(position.z)), Mathf.Abs((cameraInfo.currentScroll - scrollMinMax.y)/scrollMinMax.y));
    }

    void HandleTilt()
    {
        if (cameraInfo.isCameraParallelToGround)
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
        cameraInfo.isCameraParallelToGround = !cameraInfo.isCameraParallelToGround;

        CalculateScroll();
    }

    void HandleScrolling(float scroll)
    {
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cameraInfo.currentScroll += scroll * scrollSpeed * currentSpeedMultiplier *Time.deltaTime;
            cameraInfo.currentScroll = Mathf.Clamp(cameraInfo.currentScroll, scrollMinMax.x, scrollMinMax.y);

            CalculateScroll();
            cameraChanged = true;
        }
    }
    void CalculateScroll()
    {
        var normal = playerZoomCamera.forward;
        scrollOffset = normal * cameraInfo.currentScroll;
        playerZoomCamera.localPosition = currentPosition +scrollOffset;
    }

    void LoadMap()
    {
        cameraChanged = true;
        //cause it will be flipped in HandleTilt
        //so i preflipped it
        cameraInfo.isCameraParallelToGround = !cameraInfo.isCameraParallelToGround;
        //cameraInfo.currentScroll = cameraInfo.currentScroll;
        HandleTilt();
        playerCamera.localPosition = cameraInfo.position;
        UpdateCameraPositionOnTheMap();
    }

    void TrackMousePositionOnGrid()
    {
        if (!EventSystem.current.IsPointerOverGameObject(-1))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(mousePos), out hit, 50,256))
            {
                currentBlockX = (int)(hit.collider.transform.position.x - MapHolder.offset.x);
                currentBlockY = Mathf.Abs((int)(hit.collider.transform.position.z - MapHolder.offset.z));
                ChangeCursorPosition.Invoke(hit.collider.transform.position, currentBlockX,currentBlockY);
#if UNITY_EDITOR
                coordinateText.text = $"{currentBlockX} {currentBlockY}";
#endif
            }
            else
            {
                ChangeCursorPosition.Invoke(new Vector3(20, 0, 20),-1,-1);
                currentBlockX = -1;
                currentBlockY = -1;
            }
        }
        else
        {
            ChangeCursorPosition.Invoke(new Vector3(20, 0, 20), -1, -1);
            currentBlockX = -1;
            currentBlockY = -1;
        }
    }

    void StartConstruction()
    {
        if (construct == ToolMode.None && currentBlockX != -1)
        {
            StartConstructionAction?.Invoke(currentBlockX, currentBlockY);
        }
    }

    void EndConstruction()
    {
        construct = ToolMode.None;
        EndConstructionAction?.Invoke();
    }
    
    void ChangeItem(ToolMode mode)
    {
        if (currentTool != ToolType.Null && currentBlockX != -1)
        {
            if (construct == ToolMode.None)
            {
                construct = mode;
            }
            else
            {
                if (construct != mode)
                {
                    return;
                }
            }

            ChangeTile?.Invoke(currentBlockX, currentBlockY, currentTool, construct, variation, currentDecorationTool, rotation, color);

            prevBlockX = currentBlockX;
            prevBlockY = currentBlockY;
        }
    }

    //TODO: dooo
    private void SampleItem()
    {
        if (currentBlockX != -1)
        {
            ToolType newTool = ToolType.Null;
            byte newVariation = 0;
            int terraTool = -1;

            if (MapHolder.decorationsTiles[currentBlockX, currentBlockY] != null)
            {
                switch (MapHolder.decorationsTiles[currentBlockX,currentBlockY].type)
                {
                    case DecorationType.Fence:
                        newTool = ToolType.FenceBuilding;
                        break;
                    case DecorationType.Building:
                        newTool = ToolType.BuildingsMarkUp;
                        break;
                    case DecorationType.Incline:
                        newTool = ToolType.InclineMarkUp;
                        rotation = 0;
                        break;
                    case DecorationType.Bridge:
                        newTool = ToolType.BridgeMarkUp;
                        rotation = 0;
                        break;
                    case DecorationType.Flora:
                        newTool = ToolType.BushPlanting;
                        break;
                    case DecorationType.Tree:
                        newTool = ToolType.TreePlanting;
                        break;
                    case DecorationType.Flower:
                        newTool = ToolType.FlowerPlanting;
                        color = MapHolder.decorationsTiles[currentBlockX, currentBlockY].color;
                        break;
                }
                if (newTool == ToolType.BuildingsMarkUp)
                {
                    newVariation = (byte) MapHolder.decorationsTiles[currentBlockX, currentBlockY].building.type;
                }
                else
                {
                    newVariation = MapHolder.decorationsTiles[currentBlockX, currentBlockY].variation;
                }
            }
            else
            {
                switch (MapHolder.tiles[currentBlockX, currentBlockY].type)
                {
                    case TileType.Null:
                        break;
                    case TileType.Water:
                    case TileType.WaterDiagonal:
                        terraTool = 1;
                        newTool = ToolType.Waterscaping;
                        break;
                    case TileType.Path:
                    case TileType.PathCurve:
                        newTool = ToolType.PathPermit;
                        newVariation = MapHolder.tiles[currentBlockX, currentBlockY].variation;
                        break;
                    case TileType.Land:
                    case TileType.CliffDiagonal:
                        terraTool = 0;
                        newTool = ToolType.CliffConstruction;
                        break;
                    case TileType.Sea:
                    case TileType.Sand:
                    case TileType.SandDiagonal:
                    case TileType.SeaDiagonal:
                        terraTool = 2;
                        newTool = ToolType.SandPermit;
                        break;
                }

            }
            if (newTool != ToolType.Null)
            {
                ToolChange(newTool);

                if (terraTool != -1)
                {
                    controller.SetTerraformingButton(terraTool);
                }
                else
                {
                    controller.SetNewTool(newTool, newVariation, color);
                }
                /*if (currentDecorationTool != DecorationType.Null)
                {
                    ChooseVariation(newVariation);
                }*/
                //variation = newVariation;
            }
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
    public void SandPermitButtonClick()
    {
        ToolChange(ToolType.SandPermit);
    }

    public void PathPermitButtonClick()
    {
        ToolChange(ToolType.PathPermit);
    }

    public void FencePermitButtonClick()
    {
        rotation = 0;
        ToolChange(ToolType.FenceBuilding);
    }
    public void BushPermitButtonClick()
    {
        ToolChange(ToolType.BushPlanting);
    }
    
    public void TreePermitButtonClick()
    {
        ToolChange(ToolType.TreePlanting);
    }
    public void BuildingsPermitButtonClick()
    {
        ToolChange(ToolType.BuildingsMarkUp);
    }
    
    public void InclinePermitButtonClick()
    {
        rotation = 0;
        ToolChange(ToolType.InclineMarkUp);
    }
    public void BridgePermitButtonClick()
    {
        rotation = 0;
        ToolChange(ToolType.BridgeMarkUp);
    }
    public void FlowerPermitButtonClick()
    {
        ToolChange(ToolType.FlowerPlanting);
    }

    public void ChooseVariation(int variation)
    {
        this.variation = (byte)variation;
        if (currentTool == ToolType.BuildingsMarkUp)
        {
            ChangeCursor.Invoke((DecorationType)variation, 0);
        }
        else
        {
            ChangeCursor.Invoke(currentDecorationTool,this.variation);
        }
    }

    public void ChangeBuilding(int newBuilding)
    {
        currentDecorationTool = (DecorationType) newBuilding;
    }

    public void ChangeColor(int color)
    {
        this.color = (FlowerColors) color;
    }

    public void ToolChange(ToolType tool)
    {
        if (currentTool != tool)
        {
            currentTool = tool;
            rotation = 0;
            currentDecorationTool = Util.toolToDecorationType[tool];
            if (currentDecorationTool == DecorationType.Null)
            {
                variation = 0;
            }
            ChangeCursor.Invoke(currentDecorationTool, variation);
        }
    }
    
    
    
    /*public void SaveButton()
    {
        CameraInfo cameraInfo = new CameraInfo();
        cameraInfo.position = playerCamera.localPosition;
        cameraInfo.currentScroll = currentScroll;
        cameraInfo.isTilted = isCameraParallelToGround;
        SaveSystem.Save(cameraInfo);
    }*/

    /*public void LoadButton()
    {
       CameraInfo info = SaveSystem.Load();
       LoadMap(info);
    }*/

    public void Pause(bool isPaused)
    {
        if (isPaused)
        {
            controls.MapControl.Disable();
        }
        else
        {
            controls.MapControl.Enable();
        }
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
