using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    public Canvas canvas;
    public float showTime = 3;

    float startTime;

    public void Enable()
    {
        startTime = Time.time;
        if (!canvas.enabled)
        {
            canvas.enabled = true;

            gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (startTime + showTime <= Time.time)
        {
            Disable();
        }
    }

    void Disable()
    {
        canvas.enabled = false;

        gameObject.SetActive(false);
    }
}
