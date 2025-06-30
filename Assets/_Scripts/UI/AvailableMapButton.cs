using UnityEngine;
using UnityEngine.UI;

public class AvailableMapButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image lockIcon;

    //[SerializeField] private CustomizationButtonSelectEvent customizationButtonSelectEvent;
    //CustomizationButtonSelect buttonSelect = new CustomizationButtonSelect();

    public void prepButtonVisuals(int index, bool isOwned, Sprite sprite, SaveState.Unlockable type)
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
            lockIcon.gameObject.SetActive(true);
            Color32 col = icon.color;
            col.a = 58;
            icon.color = col;
        }
        else
        {
            lockIcon.gameObject.SetActive(false);
            Color32 col = icon.color;
            col.a = 255;
            icon.color = col;
        }

        //int j = index;
        //go.GetComponent<Button>().onClick.AddListener(() => OnMapSelect(j, type));
    }

    //public void OnMapSelect(int currentIndex, SaveState.Unlockable type)
    //{
    //    buttonSelect.type = type;
    //    buttonSelect.buttonIndex = currentIndex;

    //    customizationButtonSelectEvent.Raise(buttonSelect);
    //}
}
