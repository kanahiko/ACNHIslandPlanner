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
    bool isHorizontal = true;
    int variation = 0;
    int currentBlockX = -1;
    int currentBlockY = -1;

    int prevBlockX = -1;
    int prevBlockY = -1;

    public static Action<int, int, ToolType, ToolMode, int, DecorationType, bool> ChangeTile;
    public static Action<int, int> StartConstructionAction;
    public static Action EndConstructionAction;

    public Action<DecorationType, int> ChangeCursor;
    public Action<bool> ChangeRotationCursor;
    public Action<Vector3> ChangeCursorPosition;

    public TilePreview cursor;

    Controls controls;

    private float timeElapsedSinceClick;


    private void Awake()
    {
        ChangeCursor = cursor.ChangeTile;
        ChangeCursorPosition = cursor.FollowMousePosition;
        ChangeRotationCursor = cursor.ChangeTileRotation;

        ChangeCursor.Invoke(DecorationType.Null, -1);

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

        currentPosition = playerZoomCamera.localPosition;
        cameraDegrees = playerZoomCamera.localRotation.eulerAngles.x;

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
            isHorizontal = !isHorizontal;
            ChangeRotationCursor?.Invoke(isHorizontal);
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
                //selectionCube.transform.localPosition = hit.collider.transform.position;
                ChangeCursorPosition.Invoke(hit.collider.transform.position);
                currentBlockX = (int) (hit.collider.transform.position.x - MapHolder.offset.x);
                currentBlockY = (int) (hit.collider.transform.position.z - MapHolder.offset.z);

                //mousePos.position = mousePoint;
                coordinateText.text = $"{currentBlockX} {-currentBlockY}";
            }
            else
            {
                ChangeCursorPosition.Invoke(new Vector3(20, 0, 20));
                //selectionCube.transform.localPosition = new Vector3(20,0,20);
                currentBlockX = -1;
                currentBlockY = -1;
            }
        }
        else
        {
            ChangeCursorPosition.Invoke(new Vector3(20, 0, 20));
            //selectionCube.transform.localPosition = new Vector3(20,0,20);
            currentBlockX = -1;
            currentBlockY = -1;
        }
    }

    void StartConstruction()
    {
        if (construct == ToolMode.None && currentBlockX != -1)
        {
            StartConstructionAction?.Invoke(currentBlockX, Mathf.Abs(currentBlockY));
            //TerrainBuilder.StartConstruction(MapHolder.tiles[].elevation);
        }
    }

    void EndConstruction()
    {
        construct = ToolMode.None;
        EndConstructionAction?.Invoke();
       // TerrainBuilder.EndConstruction();
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

            ChangeTile?.Invoke(currentBlockX, Mathf.Abs(currentBlockY), currentTool, construct, variation, currentDecorationTool, isHorizontal);

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
            ChangeCursor.Invoke(DecorationType.Null, -1);
            //ChangeCursor.?Invoke();
            currentDecorationTool = DecorationType.Null;
            ToolChange(ToolType.Waterscaping);
        }
    }
    
    public void CliffConstructionButtonClick()
    {
        if (currentTool != ToolType.CliffConstruction)
        {
            ChangeCursor.Invoke(DecorationType.Null, -1);
            currentDecorationTool = DecorationType.Null;
            ToolChange(ToolType.CliffConstruction);
        }
    }

    public void PathPermitButtonClick()
    {
        if (currentTool != ToolType.PathPermit)
        {
            ChangeCursor.Invoke(DecorationType.Null, -1);
            currentDecorationTool = DecorationType.Null;
            ToolChange(ToolType.PathPermit);
        }
    }

    public void FencePermitButtonClick()
    {
        //ChangeCursor.Invoke(DecorationType.Fence, 0);
        //variation = 0;
        if (currentTool != ToolType.FenceBuilding)
        {
            isHorizontal = true;
            currentDecorationTool = DecorationType.Fence;
            ToolChange(ToolType.FenceBuilding);
        }
    }
    public void BushPermitButtonClick()
    {
        //ChangeCursor.Invoke(DecorationType.Flora, 0);
        //variation = 0;
        if (currentTool != ToolType.BushPlanting)
        {
            isHorizontal = true;
            currentDecorationTool = DecorationType.Flora;
            ToolChange(ToolType.BushPlanting);
        }
    }
    
    public void TreePermitButtonClick()
    {
        //ChangeCursor.Invoke(DecorationType.Tree, 0);
        //variation = 0;
        if (currentTool != ToolType.TreePlanting)
        {
            isHorizontal = true;
            currentDecorationTool = DecorationType.Tree;
            ToolChange(ToolType.TreePlanting);
        }
    }
    public void BuildingsPermitButtonClick()
    {
        //ChangeCursor.Invoke(DecorationType.House, 0);
        //variation = 0;
        if (currentTool != ToolType.BuildingsMarkUp)
        {
            isHorizontal = true;
            currentDecorationTool = DecorationType.House;
            ToolChange(ToolType.BuildingsMarkUp);
        }
    }

    public void ChooseVariation(int variation)
    {
        //Debug.Log($"test {variation}");
        this.variation = variation;
        switch (currentTool)
        {
            case ToolType.BridgeMarkUp:
                break;
            case ToolType.InclineMarkUp:
                break;
            case ToolType.TreePlanting:
                ChangeCursor.Invoke(DecorationType.Tree, variation);
                break;
            case ToolType.BushPlanting:
                ChangeCursor.Invoke(DecorationType.Flora, variation);
                break;
            case ToolType.FenceBuilding:
                ChangeCursor.Invoke(DecorationType.Fence, variation);
                break;
            case ToolType.BuildingsMarkUp:
                ChangeCursor.Invoke((DecorationType)variation, -1);
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

}
