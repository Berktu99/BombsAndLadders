using UnityEngine;

public class PlayerFollowCamera : MonoBehaviour
{
    private Cinemachine.CinemachineVirtualCamera virtualCamera;
    void Start()
    {
        virtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        virtualCamera.LookAt = FindObjectOfType<PlayerController>(true).transform;
        virtualCamera.Follow = FindObjectOfType<PlayerController>(true).transform;

        Destroy(this, 1f);
    }
}
