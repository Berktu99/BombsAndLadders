using UnityEngine;
using DG.Tweening;

public class CoinAnimated : MonoBehaviour
{
    [SerializeField] private IntEvent tweenEnd;

    [SerializeField] private Vector2 randomMoveSpeedMinMax = new Vector2(60, 80);
    [SerializeField] private float rotateSpeed;

    [SerializeField] private float coinGrowSecond = 0.8f;
    [SerializeField] private float growScale = 1.5f;
    [SerializeField] private float coinShrinkSecond = 0.3f;

    [SerializeField] private Ease growEase;
    [SerializeField] private Ease shrinkEase;

    [SerializeField] private float moveTime;

    [SerializeField] private ParticleSystem singlePS;

    public void doAnim(RectTransform begin, RectTransform goldGui, int value, float spawnRadius)
    {
        Vector3 originalScale;
        originalScale = transform.localScale;

        Sequence seq = DOTween.Sequence();

        transform.localScale = Vector3.zero;
        transform.localEulerAngles = new Vector3(1, 1, 1).randomV3() * 360;
        transform.localPosition = begin.position + new Vector3(1, 1, 1).randomV3() * spawnRadius;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(1, 1, 1).randomV3() * UnityEngine.Random.Range(rotateSpeed / 2, rotateSpeed);

        GameObject sps = Instantiate(singlePS.gameObject);
        sps.transform.localPosition = transform.position;

        seq.Append(transform.DOScale(originalScale * growScale, coinGrowSecond).SetEase(growEase));

        seq.Append(transform.DOScale(originalScale, coinShrinkSecond).SetEase(shrinkEase));

        seq.OnComplete(() =>
        {
            transform.DOScale(originalScale * 0.05f, moveTime);

            transform.DOMove(goldGui.position, UnityEngine.Random.Range(randomMoveSpeedMinMax.x, randomMoveSpeedMinMax.y)).SetSpeedBased(true).OnComplete(() => {

                tweenEnd.Raise(value);

                seq.Kill();

                Destroy(this.gameObject, 1f);
            });
        });
    }
}
