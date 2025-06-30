using DG.Tweening;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    [HideInInspector] public bool isEmpty = true;
    [HideInInspector] public Bomb bomb = null;

    [SerializeField] private float posAnimTime = 1.5f;
    [SerializeField] private float beginOffset = 1.2f;
    [SerializeField] private float endOffset = 2.4f;

    private GameObject animate;
    private void Awake()
    {
        animate = new GameObject("pedAnim");

        Vector3 beginPos = transform.position + Vector3.up * beginOffset;
        
        animate.transform.SetPositionAndRotation(beginPos, Quaternion.identity);

        animate.transform.DOMove(beginPos + endOffset * Vector3.up, posAnimTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        if (!isEmpty && bomb != null)
        {
            bomb.gameObject.transform.position = animate.transform.position;

            if (bomb.pickedUpFromPadestal)
            {
                isEmpty = true;
                bomb = null;
            }
        }
    }
}
