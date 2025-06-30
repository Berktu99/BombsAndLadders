using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceSubMenus_Maps : MonoBehaviour
{
    [SerializeField] private UserInterfaceMainMenu mainMenu;
    [SerializeField] private UserInterfaceSubMenus subMenus;

    [SerializeField] private MapsSO maps;
    [SerializeField] private GameObject mapsGrid;

    [SerializeField] private SaveStateObject saveStateObject;

    [Foldout("Buttons", true)]
    public GameObject nullButton;
    public GameObject mapAvailableButton;
    public GameObject workInProgressButton;
    public GameObject comingSoonButton;

    private void Awake() => initializeMapsScreen();

    private void initializeMapsScreen()
    {
        for (int i = mapsGrid.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(mapsGrid.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < 1; i++)
        {
            GameObject go = Instantiate(nullButton, mapsGrid.transform) as GameObject;
        }

        int j = 0;
        for (int i = 0; i < maps.availableMaps.Count; i++)
        {
            GameObject button = Instantiate(mapAvailableButton, mapsGrid.transform) as GameObject;
            button.GetComponent<AvailableMapButton>().prepButtonVisuals(i, saveStateObject.isUnlocked(SaveState.Unlockable.Maps, i), maps.availableMaps[i].sprite, SaveState.Unlockable.Maps);

            int index = j;
            if (saveStateObject.isUnlocked(SaveState.Unlockable.Maps, i))
            {
                button.GetComponent<Button>().onClick.AddListener(() => onMapSelect(index));
            }                    
            j++;
        }

        for (int i = 0; i < maps.workInProgressMaps.Count; i++)
        {
            GameObject go = Instantiate(workInProgressButton, mapsGrid.transform) as GameObject;

            Destroy(go.GetComponent<Button>());
        }

        for (int i = 0; i < maps.comingSoonMaps.Count; i++)
        {
            GameObject go = Instantiate(comingSoonButton, mapsGrid.transform) as GameObject;

            Destroy(go.GetComponent<Button>());
        }

        for (int i = 0; i < 1; i++)
        {
            GameObject go = Instantiate(nullButton, mapsGrid.transform) as GameObject;
        }
    }

    public void onMapSelect(int currentIndex)
    {
        StartCoroutine(wait());
        System.Collections.IEnumerator wait()
        {
            yield return Yielders.CachedWaitForSeconds(0.1f);
            Loader.loadMap(currentIndex);
        }         
    }

    public void Button_OnCloseMapsCanvas()
    {
        mainMenu.hideChildren(false);
        mainMenu.toggleTapToPlay(true);

        this.gameObject.SetActive(false);
        subMenus.gameObject.SetActive(false);
    }
}
