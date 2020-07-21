using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public BuildersController buildersController;
    public UIController controller;
    
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
    
    public Text coordinateText;

    //public float timeBetweenClicks = 0.1f;

    //checks how much zoomed in or out
    float currentScroll = 0;
    //of scroll
    Vector3 currentPosition;

    bool isCameraParallelToGround = false;
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
    int currentBlockX = -1;
    int currentBlockY = -1;

    int prevBlockX = -1;
    int prevBlockY = -1;

    public static Action<int, int, ToolType, ToolMode, byte, DecorationType, byte> ChangeTile;
    public static Action<int, int> StartConstructionAction;
    public static Action EndConstructionAction;

    public Action<DecorationType, byte> ChangeCursor;
    public Action<int, DecorationType> ChangeRotationCursor;
    public Action<Vector3, int ,int> ChangeCursorPosition;
    public Action<Vector2, float> ChangeCameraPosition;

    Controls controls;
    public static Action<Dictionary<Vector2Int, List<Vector2Int>>, List<PreDecorationTile>> RebuildMap;

    //private float timeElapsedSinceClick;


    private void Awake()
    {
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
        controls.MapControl.WaterscapingTool.performed += ctx => controller.SetToolButton(ToolType.Waterscaping);
        controls.MapControl.CliffConstructionTool.performed += ctx => controller.SetToolButton(ToolType.CliffConstruction);
        controls.MapControl.PathPermitTool.performed += ctx => controller.SetToolButton(ToolType.PathPermit);
        controls.MapControl.FenceTool.performed += ctx => controller.SetToolButton(ToolType.FenceBuilding);
        controls.MapControl.BushTool.performed += ctx => controller.SetToolButton(ToolType.BushPlanting);
        controls.MapControl.TreeTool.performed += ctx => controller.SetToolButton(ToolType.TreePlanting);
        controls.MapControl.BuildingsTool.performed += ctx => controller.SetToolButton(ToolType.BuildingsMarkUp);

        controls.MapControl.PlaceItem.performed += ctx =>
        {
            StartConstruction();
            ChangeItem(ToolMode.Add);
        };
        controls.MapControl.PlaceItem.canceled += ctx => EndConstruction();
        controls.MapControl.RemoveItem.performed += ctx =>
        {
            StartConstruction();
            ChangeItem(ToolMode.Remove);
        };
        controls.MapControl.RemoveItem.canceled += ctx => EndConstruction();
        controls.MapControl.SampleItem.performed += ctx => SampleItem();

        controls.MapControl.Rotate.performed += ctx => Rotate();
        
        construct = ToolMode.None;

    }
    

    void SetToolButton(ToolType type)
    {
        controller.SetToolButton(type);
    }

    void Start()
    {
        playerFieldSizeMin.y = playerFieldSizeMax.y - MapHolder.height;
        playerFieldSizeMax.x = MapHolder.width;

        currentScroll = defaultScroll;
        currentPosition = playerZoomCamera.localPosition;
        cameraDegrees = playerZoomCamera.localRotation.eulerAngles.x;
        CalculateScroll();

        ChangeCameraPosition = MiniMap.ChangeCameraPosition;
        UpdateCameraPositionOnTheMap();
    }
    
    void Update()
    {
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
        
        if (cameraChangedHere)
        {
            UpdateCameraPositionOnTheMap();
        }

    }

    void UpdateCameraPositionOnTheMap()
    {
        Vector3 position = playerCamera.position - buildersController.offsetTerrain;
        
        ChangeCameraPosition?.Invoke(new Vector2(position.x,Mathf.Abs(position.z)), Mathf.Abs((currentScroll - scrollMinMax.y)/scrollMinMax.y));
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

        CalculateScroll();
    }

    void HandleScrolling(float scroll)
    {
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentScroll += scroll * scrollSpeed * currentSpeedMultiplier *Time.deltaTime;
            currentScroll = Mathf.Clamp(currentScroll, scrollMinMax.x, scrollMinMax.y);

            CalculateScroll();
            cameraChanged = true;
        }
    }
    void CalculateScroll()
    {
        var normal = playerZoomCamera.forward;
        scrollOffset = normal * currentScroll;
        playerZoomCamera.localPosition = currentPosition +scrollOffset;
    }

    void LoadMap(CameraInfo loadedCamera)
    {
        cameraChanged = true;
        isCameraParallelToGround = !loadedCamera.isTilted;
        currentScroll = loadedCamera.currentScroll;
        HandleTilt();
        playerCamera.localPosition = loadedCamera.position;
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
                currentBlockY = (int)(hit.collider.transform.position.z - MapHolder.offset.z);
                ChangeCursorPosition.Invoke(hit.collider.transform.position, currentBlockX, Mathf.Abs(currentBlockY));

                coordinateText.text = $"{currentBlockX} {-currentBlockY}";
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
            StartConstructionAction?.Invoke(currentBlockX, Mathf.Abs(currentBlockY));
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

            ChangeTile?.Invoke(currentBlockX, Mathf.Abs(currentBlockY), currentTool, construct, variation, currentDecorationTool, rotation);

            prevBlockX = currentBlockX;
            prevBlockY = currentBlockY;
        }
    }

    private void SampleItem()
    {
        if (currentBlockX != -1)
        {
            switch (MapHolder.tiles[currentBlockX , Mathf.Abs(currentBlockY)].type)
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
        if (currentTool != ToolType.Waterscaping)
        {
            variation = 0;
            rotation = 0;
            ChangeCursor.Invoke(DecorationType.Null, 0);
            currentDecorationTool = DecorationType.Null;
            ToolChange(ToolType.Waterscaping);
        }
    }
    
    public void CliffConstructionButtonClick()
    {
        if (currentTool != ToolType.CliffConstruction)
        {
            variation = 0;
            rotation = 0;
            ChangeCursor.Invoke(DecorationType.Null, 0);
            currentDecorationTool = DecorationType.Null;
            ToolChange(ToolType.CliffConstruction);
        }
    }
    public void SandPermitButtonClick()
    {
        if (currentTool != ToolType.SandPermit)
        {
            variation = 0;
            rotation = 0;
            ChangeCursor.Invoke(DecorationType.Null, 0);
            currentDecorationTool = DecorationType.Null;
            ToolChange(ToolType.SandPermit);
        }
    }

    public void PathPermitButtonClick()
    {
        if (currentTool != ToolType.PathPermit)
        {
            rotation = 0;
            ChangeCursor.Invoke(DecorationType.Null, 0);
            currentDecorationTool = DecorationType.Null;
            ToolChange(ToolType.PathPermit);
        }
    }

    public void FencePermitButtonClick()
    {
        if (currentTool != ToolType.FenceBuilding)
        {
            rotation = 0;
            currentDecorationTool = DecorationType.Fence;
            ToolChange(ToolType.FenceBuilding);
        }
    }
    public void BushPermitButtonClick()
    {
        if (currentTool != ToolType.BushPlanting)
        {
            rotation = 0;
            currentDecorationTool = DecorationType.Flora;
            ToolChange(ToolType.BushPlanting);
        }
    }
    
    public void TreePermitButtonClick()
    {
        if (currentTool != ToolType.TreePlanting)
        {
            rotation = 0;
            currentDecorationTool = DecorationType.Tree;
            ToolChange(ToolType.TreePlanting);
        }
    }
    public void BuildingsPermitButtonClick()
    {
        if (currentTool != ToolType.BuildingsMarkUp)
        {
            rotation = 0;
            currentDecorationTool = DecorationType.House;
            ToolChange(ToolType.BuildingsMarkUp);
        }
    }
    
    public void InclinePermitButtonClick()
    {
        if (currentTool != ToolType.InclineMarkUp)
        {
            rotation = 0;
            currentDecorationTool = DecorationType.Incline;
            ToolChange(ToolType.InclineMarkUp);
        }
    }
    public void BridgePermitButtonClick()
    {
        if (currentTool != ToolType.BridgeMarkUp)
        {
            rotation = 0;
            currentDecorationTool = DecorationType.Bridge;
            ToolChange(ToolType.BridgeMarkUp);
        }
    }

    public void ChooseVariation(int variation)
    {
        this.variation = (byte)variation;
        switch (currentTool)
        {
            case ToolType.BridgeMarkUp:
                ChangeCursor.Invoke(DecorationType.Bridge,this.variation);
                break;
            case ToolType.InclineMarkUp:
                ChangeCursor.Invoke(DecorationType.Incline,this.variation);
                break;
            case ToolType.TreePlanting:
                ChangeCursor.Invoke(DecorationType.Tree, this.variation);
                break;
            case ToolType.BushPlanting:
                ChangeCursor.Invoke(DecorationType.Flora, this.variation);
                break;
            case ToolType.FenceBuilding:
                ChangeCursor.Invoke(DecorationType.Fence, this.variation);
                break;
            case ToolType.BuildingsMarkUp:
                ChangeCursor.Invoke((DecorationType)variation, 0);
                break;
        }
    }

    public void ChangeBuilding(int newBuilding)
    {
        currentDecorationTool = (DecorationType) newBuilding;
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


    public void TestSaveButton()
    {
        CameraInfo cameraInfo = new CameraInfo();
        cameraInfo.position = playerCamera.localPosition;
        cameraInfo.currentScroll = currentScroll;
        cameraInfo.isTilted = isCameraParallelToGround;
        MapHolder.Save(cameraInfo);
    }

    public void TestLoadButton()
    {
       CameraInfo info = MapHolder.Load();
       LoadMap(info);
    }
}
