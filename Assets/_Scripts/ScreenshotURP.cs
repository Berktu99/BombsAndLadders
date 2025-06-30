using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotURP : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {

        }
        ScreenCapture.CaptureScreenshot("Level_2_SS.png");
    }
}
