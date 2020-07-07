using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Vector2 playerFieldSizeMin = new Vector2(0,0);
    public Vector2 playerFieldSizeMax = new Vector2(16, 16);
    Transform playerCamera;
    [Range(0,1)]
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void HandleInput(float x, float y)
    {
        if (Mathf.Abs(x) < 0.1f)
        {
            x = 0;
        }

        if (Mathf.Abs(y) < 0.1f)
        {
            y = 0;
        }

        Vector3 currentPosition = playerCamera.position;
        currentPosition.x += x * speed;
        currentPosition.z += y * speed;
        if (currentPosition.x > playerFieldSizeMax.x)
        {
            currentPosition.x = playerFieldSizeMax.x;
        }
        if (currentPosition.x < playerFieldSizeMin.x)
        {
            currentPosition.x = playerFieldSizeMin.x;
        }
        if (currentPosition.y > playerFieldSizeMax.y)
        {
            currentPosition.y = playerFieldSizeMax.y;
        }
        if (currentPosition.y < playerFieldSizeMin.y)
        {
            currentPosition.y = playerFieldSizeMin.x;
        }
        playerCamera.position = currentPosition;
    }
}
