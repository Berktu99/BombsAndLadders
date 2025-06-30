using UnityEngine;
using MyBox;
using UnityEngine.UI;

public class SpinMiniGame : MonoBehaviour
{
    private const float fillAmountMax = 0.5f;

    private bool shouldAnimate = false;

    [System.Serializable]
    public class SinglePiece
    {
        public float percent;
        public float multiply;
    }

    [Foldout("Wheel Stuff", true)]
    [SerializeField] private SinglePiece[] wheel;

    [SerializeField] private Transform piecesParent;

    [SerializeField] private Transform textParent;

    [SerializeField] private float textDistance = 101f;
    private float[] piecesAngles;

    private Color32[] originalColors;
    [SerializeField] private Color32 highlightColor;

    private float currentMultiply = 0;

    [Foldout("Led Lights Stuff", true)]
    [SerializeField] private Image[] lights;
    [SerializeField] private Color32 normalColor;
    [SerializeField] private Color32 ledHighlightColor;
    [SerializeField] private float timer;
    [SerializeField] private float time;
    private bool evenLightsHighlight = true;


    [Foldout("Button Stuff", true)]
    [SerializeField] private TMPro.TextMeshProUGUI coinText;

    [SerializeField] private RectTransform arrow;
    private float arrowCurrentAngle;

    [SerializeField] private Vector2 arrowAngleInterval = new Vector2(0f, -150f);
    [SerializeField] private float arrowRotateSpeed = 20f;

    [SerializeField] private IntVariable gainedCoin_Raw;

    [SerializeField] private IntEvent multGoldEvent;
    

    private void Start()
    {
        prepWheel();
    }

    private void prepWheel()
    {
        timer = time;

        piecesAngles = new float[wheel.Length - 1];
        originalColors = new Color32[wheel.Length];

        float sumPercent = 0;
        for (int i = 0; i < wheel.Length; i++)
        {
            piecesParent.GetChild(4 - i).GetComponent<Image>().fillAmount = fillAmountMax * (wheel[i].percent + sumPercent) * 0.01f;

            float textFillDegree = 180 - fillAmountMax * (sumPercent + wheel[i].percent * 0.5f) * 0.01f * 180f * 2;
            if (i < piecesAngles.Length)
            {
                piecesAngles[i] = textFillDegree;
            }

            originalColors[i] = piecesParent.GetChild(4 - i).GetComponent<Image>().color;
            
            float xPos = Mathf.Cos(textFillDegree * Mathf.Deg2Rad) * -textDistance;
            float yPos = Mathf.Sin(textFillDegree * Mathf.Deg2Rad) * textDistance;

            textParent.GetChild(4 - i).GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
            textParent.GetChild(4 - i).GetComponent<TMPro.TextMeshProUGUI>().text = "x" + wheel[i].multiply.ToString();

            sumPercent += wheel[i].percent;
        }

        shouldAnimate = true;
    }

    private void Update()
    {
        if (shouldAnimate)
        {
            doAnimation();
        }
        
    }

    private void doAnimation()
    {
        doArrow();
        void doArrow()
        {
            arrowCurrentAngle += arrowRotateSpeed * Time.deltaTime;
            if (arrowCurrentAngle > arrowAngleInterval.x)
            {
                arrowCurrentAngle =  arrowAngleInterval.x;

                arrowRotateSpeed *= -1f;
            }
            else if (arrowCurrentAngle < arrowAngleInterval.y)
            {
                arrowCurrentAngle = arrowAngleInterval.y;

                arrowRotateSpeed *= -1f;
            }

            arrow.localEulerAngles = new Vector3(0f, 0f, arrowCurrentAngle);
        }

        doLedLights();
        void doLedLights()
        {
            timer -= Time.unscaledDeltaTime;

            if (timer < 0f)
            {
                evenLightsHighlight = !evenLightsHighlight;
                timer = time;
            }

            if (evenLightsHighlight)
            {
                lights[0].color = ledHighlightColor;
                lights[2].color = ledHighlightColor;

                lights[1].color = normalColor;
                lights[3].color = normalColor;
            }
            else
            {
                
                lights[1].color = ledHighlightColor;
                lights[3].color = ledHighlightColor;

                lights[0].color = normalColor;
                lights[2].color = normalColor;
            }
            
        }

        doHighlight();
        void doHighlight()
        {
            int i;
            bool found = false;
            for (i = 0; i < piecesAngles.Length; i++)
            {
                if (piecesAngles[i] < arrowCurrentAngle + 180f)
                {
                    piecesParent.GetChild(4 - i).GetComponent<Image>().color = highlightColor;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                piecesParent.GetChild(4 - i).GetComponent<Image>().color = highlightColor;
            }


            for (int j = 0; j < wheel.Length; j++)
            {
                if (4 - i != j)
                {
                    piecesParent.GetChild(j).GetComponent<Image>().color = originalColors[j];
                }
            }
        }

        updateText();
        void updateText()
        {
            int i;
            
            for ( i = 0; i < piecesAngles.Length; i++)
            {
                if (piecesAngles[i] < arrowCurrentAngle + 180f)
                {
                    currentMultiply = wheel[i].multiply;
                    coinText.text = (gainedCoin_Raw.Value * currentMultiply).ToString();
                    return;
                }
            }

            coinText.text = (gainedCoin_Raw.Value * wheel[i].multiply).ToString();
        }
    }

    public void Button_MultGold()
    {
        multGoldEvent.Raise((int)currentMultiply);

        shouldAnimate = false;
    }
}
