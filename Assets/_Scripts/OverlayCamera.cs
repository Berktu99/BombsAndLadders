using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OverlayCamera : MonoBehaviour
{
    private Camera thisCamera;

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();

        thisCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;

        if (!Camera.main.GetUniversalAdditionalCameraData().cameraStack.Contains(thisCamera))
        {
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(thisCamera);
        }

        Destroy(this, 1f);
    }
}
