using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour
{
    public Vector2 playerFieldSizeMin = new Vector2(0, 0);
    public Vector2 playerFieldSizeMax = new Vector2(0, -3.5f);
    public Vector3 scrollOffset = new Vector3(0, 0, 0);

    public Vector2 scrollMinMax = new Vector2(-1, 20);
    float currentScroll = 0;

    public Camera mainCamera;
    public Transform playerCamera;
    //public Transform playerTiltCamera;
    public Transform playerZoomCamera;
    [Range(0, 1)]
    public float speed;
    [Range(0, 10)]
    public float scrollSpeed;

    public ToolType currentTool = ToolType.Null;

    public GameObject selectionCube;

    Vector3 currentPosition;

    bool tilt = true;
    float cameraDegrees = 45;
    bool cameraChanged;

    int currentBlockX = -1;
    int currentBlockY = -1;

    // Start is called before the first frame update
    void Start()
    {
        playerFieldSizeMin.y = playerFieldSizeMax.y - MapHolder.height;
        playerFieldSizeMax.x = MapHolder.width;

        currentPosition = playerZoomCamera.localPosition;
        cameraDegrees = playerZoomCamera.localRotation.eulerAngles.x;
    }
    Vector3 mousePosition;
    // Update is called once per frame
    void Update()
    {
        cameraChanged = false;
        HandleInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        HandleScrolling(Input.GetAxis("Mouse ScrollWheel"));

        if (Input.GetKeyDown(KeyCode.F))
        {
            HandleTilt();
            return;
        }

        if (cameraChanged || Vector3.SqrMagnitude(Input.mousePosition - mousePosition) > 0.01f)
        {
            mousePosition = Input.mousePosition;
            TrackMousePositionOnGrid();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButtonUp();
        }
    }

    void HandleInput(float x, float y)
    {
        Vector3 currentPosition = playerCamera.localPosition;
        if (Mathf.Abs(x) >= 0.1f)
        {
            currentPosition.x += x * speed;
            currentPosition.x = Mathf.Clamp(currentPosition.x, playerFieldSizeMin.x + MapHolder.offset.x, playerFieldSizeMax.x + MapHolder.offset.x);
            cameraChanged = true;
        }

        if (Mathf.Abs(y) >= 0.1f)
        {
            currentPosition.z += y * speed;
            currentPosition.z = Mathf.Clamp(currentPosition.z, playerFieldSizeMin.y + MapHolder.offset.z, playerFieldSizeMax.y + MapHolder.offset.z);
            cameraChanged = true;
        }

        playerCamera.localPosition = currentPosition;
    }

    void HandleTilt()
    {
        if (tilt)
        {
            float distanceToGround = Mathf.Sqrt((playerCamera.position.y * playerCamera.position.y) * 2) - playerCamera.position.y;
            playerZoomCamera.localPosition = new Vector3(0, distanceToGround, playerCamera.position.y);
            playerZoomCamera.localRotation = Quaternion.Euler(90, 0, 0);
            currentPosition = playerZoomCamera.localPosition;
        }
        else
        {
            playerZoomCamera.localPosition = Vector3.zero;
            playerZoomCamera.localRotation = Quaternion.Euler(cameraDegrees, 0, 0);
            currentPosition = playerZoomCamera.localPosition;
        }

        cameraChanged = true;
        tilt = !tilt;

        var normal = playerZoomCamera.forward;
        scrollOffset = normal * currentScroll;
        playerZoomCamera.localPosition = currentPosition +scrollOffset;

        //scrollOffset = playerZoomCamera.localPosition;
    }

    void HandleScrolling(float scroll)
    {
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentScroll += scroll * scrollSpeed;
            currentScroll = Mathf.Clamp(currentScroll, scrollMinMax.x, scrollMinMax.y);

            var normal = playerZoomCamera.forward;
            scrollOffset = normal * currentScroll;
            playerZoomCamera.localPosition = currentPosition +scrollOffset;
            cameraChanged = true;
            //playerTiltCamera.position = playerTiltCamera.forward * currentScroll;

            /* Vector3 currentPosition = playerCamera.localPosition; 
             currentPosition.x = Mathf.Clamp(currentPosition.x, playerFieldSizeMin.x + MapHolder.offset.x, playerFieldSizeMax.x + MapHolder.offset.x); 
             currentPosition.z = Mathf.Clamp(currentPosition.z, playerFieldSizeMin.y + MapHolder.offset.z + scrollOffset.z / 2f, 
                 playerFieldSizeMax.y + MapHolder.offset.z + scrollOffset.z / 2f);
             playerCamera.localPosition = currentPosition;*/
        }
    }

    void TrackMousePositionOnGrid()
    {
        if (!EventSystem.current.IsPointerOverGameObject(-1))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(mousePosition), out hit, 50))
            {
                selectionCube.transform.localPosition = hit.collider.transform.localPosition;
                currentBlockX = (int)hit.collider.transform.localPosition.x;
                currentBlockY = (int)hit.collider.transform.localPosition.z;
            }
            else
            {
                currentBlockX = -1;
                currentBlockY = -1;
            }
        }
    }

    void OnLeftMouseButtonUp()
    {
        if (currentTool != ToolType.Null && currentBlockX != -1)
        {
            switch (currentTool)
            {
                case ToolType.Waterscaping:
                    TerrainBuilder.ChangeTile(TileType.Water, currentBlockX,Mathf.Abs(currentBlockY) - 1);
                    break;
                case ToolType.CliffConstruction:
                    break;
                case ToolType.PathPermit:
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
                case ToolType.FenceBuilding:
                    break;
                case ToolType.BuildingsMarkUp:
                    break;
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

    public void PathPermitButtonClick()
    {
        ToolChange(ToolType.PathPermit);
    }

    public void ToolChange(ToolType tool)
    {
        currentTool = tool;
    }
}
