using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvailableSkinButton : MonoBehaviour
{
    public Image icon;
    
    public GameObject costVideoPanel;
    public GameObject costGoldPanel;

    public Transform costParent;

    public CustomizationButtonSelectEvent customizationButtonSelectEvent;
    CustomizationButtonSelect buttonSelect = new CustomizationButtonSelect();

    public void prepButtonVisuals(int index, bool isOwned, SingleSkin singleSkin, Sprite sprite, SaveState.Unlockable type)
    {
        GameObject go = this.gameObject;

        if (icon != null)
        {
            icon.sprite = sprite;
        }
        else
        {
            Debug.Log("icon wan not assigned!");
        }

        if (!isOwned)
        {
            if (costParent == null)
            {
                Debug.Log("cost parent not found!");

            }
            else
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
        }

        //int j = index;
        //go.GetComponent<Button>().onClick.AddListener(() => onSkinSelect(j, type));
    }

    public void onSkinSelect(int currentIndex, SaveState.Unlockable type)
    {
        buttonSelect.type = type;
        buttonSelect.buttonIndex = currentIndex;

        customizationButtonSelectEvent.Raise(buttonSelect);        
    }
}
