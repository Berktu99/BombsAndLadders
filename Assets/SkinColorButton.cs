using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinColorButton : MonoBehaviour
{
    public Image icon;

    public GameObject costVideoPanel;
    public GameObject costGoldPanel;

    public Transform costParent;

    public IntEvent skinButton;
    public void prepButtonVisuals(int i, bool isOwned, SingleSkin singleSkin, Color32 color)
    {
        GameObject go = this.gameObject;

        if (icon != null)
        {
            icon.color = color;
        }
        else
        {
            Debug.Log("icon wan not assigned!");
        }


        if (!isOwned)
        {
            if (singleSkin.isCostVideo)
            {
                GameObject c = Instantiate(costVideoPanel, costParent) as GameObject;
                c.name = "costPanel";
                TextMeshProUGUI videoPercentText = c.FindChildWithTagBreadthFirst<TextMeshProUGUI>("cost");
                videoPercentText.text = singleSkin.videoWatched.ToString() + "/" + singleSkin.videoCost.ToString();
            }
            else
            {
                GameObject c = Instantiate(costGoldPanel, costParent) as GameObject;
                c.name = "costPanel";
                TextMeshProUGUI coinText = c.FindChildWithTagBreadthFirst<TextMeshProUGUI>("cost");
                coinText.text = singleSkin.cost.ToString();
            }
        }


        int index = i;
        go.GetComponent<Button>().onClick.AddListener(() => onCharacterSkinSelect(index));
    }
    public void onCharacterSkinSelect(int currentIndex)
    {
        skinButton.Raise(currentIndex);
    }
}
