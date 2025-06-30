using UnityEngine;

public class GetTransform : MonoBehaviour
{
    [SerializeField] private Transform packTransform = null;

    public Transform getPickUpPackTransform()
    {
        return packTransform;
    }
}
