using MyBox;
using UnityEngine;

[System.Serializable]
public class Mannequin
{
    public GameObject showcase;
    public Transform gfxParent;
    public Renderer defaultSkinRenderer;
    public Animator animator;
}

[System.Serializable]
public class HumanoidMannequin : Mannequin
{
    public Transform headAccessoriesParent;
}

public class UserInterfaceSubMenus_Showcases : MonoBehaviour
{
    [SerializeField] private AllSkins allSkins;

    [SerializeField] private SaveStateObject saveStateObject;

    Transform[] toDestroy_HumanoidMannequin;

    [Foldout("Show Cases", true)]
    [SerializeField] private HumanoidMannequin humanoidMannequin;
    [SerializeField] private Mannequin bombMannequin;
    [SerializeField] private Mannequin pickUpMannequin;

    [SerializeField] private GameObject purchaseFireworkPS;
    [SerializeField] private Transform fireworksSpawnTransform;

    [SerializeField] private Camera humanCloseUp;
    [SerializeField] private Camera humanFar;

    public enum SCcameraType
    {
        HumanCloseUp,
        HumanFar,
    }

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    private void OnEnable()
    {
        hideShowcaseChildren(false, ShowCases.Humanoid);
    }

    private void OnDisable()
    {
        
    }

    public void equipHumanoidMannequin(SCcameraType cameraType = SCcameraType.HumanFar)
    {
        switch (cameraType)
        {
            case SCcameraType.HumanFar:
                {
                    humanCloseUp.gameObject.SetActive(false);
                    humanFar.gameObject.SetActive(true);
                    break;
                }
            case SCcameraType.HumanCloseUp:
                {
                    humanCloseUp.gameObject.SetActive(true);
                    humanFar.gameObject.SetActive(false);
                    break;
                }
        }

        switch (allSkins.characterSkins.availableSkins[saveStateObject._characterSkinActiveIndex.Value].skinType)
        {
            case HumanoidSkinType.Accessory:
                {
                    // destroy previously spawn shit ON BOTH ACCESSORY channel AND whole body spawn (there should be only 1 instance)    
                    toDestroy_HumanoidMannequin = humanoidMannequin.gfxParent.gameObject.FindComponentsInChildrenWithTag<Transform>("wholeBodySkin");

                    for (int i = 0; i < toDestroy_HumanoidMannequin.Length; i++)
                    {
                        Destroy(toDestroy_HumanoidMannequin[i].gameObject);
                    }

                    for (int i = humanoidMannequin.headAccessoriesParent.childCount - 1; i >= 0; i--)
                    {
                        Destroy(humanoidMannequin.headAccessoriesParent.GetChild(i).gameObject);
                    }

                    humanoidMannequin.gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").gameObject.SetActive(true);
                    humanoidMannequin.gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").SetAsFirstSibling();

                    Instantiate(allSkins.characterSkins.availableSkins[saveStateObject._characterSkinActiveIndex.Value].skinPrefab, humanoidMannequin.headAccessoriesParent);

                    humanoidMannequin.animator.Rebind();

                    break;
                }
            case HumanoidSkinType.WholeBody:
                {
                    // this time, do NOT destroy default skin, animator work for only the 0th child under gfx transform
                    // make sure there is only one whole body spawn (except of course deafult skin) at any given time                   

                    // REMEMBER
                    // when instantiating it is all good but when destroying it updates the end of frame

                    toDestroy_HumanoidMannequin = humanoidMannequin.gfxParent.gameObject.FindComponentsInChildrenWithTag<Transform>("wholeBodySkin");

                    if (toDestroy_HumanoidMannequin.Length > 1)
                    {
                        Debug.LogError("There should not have been more than 1 whole body skin");
                        return;
                    }
                    else if (toDestroy_HumanoidMannequin.Length == 1)
                    {
                        Destroy(toDestroy_HumanoidMannequin[0].gameObject);
                    }

                    Instantiate(allSkins.characterSkins.availableSkins[saveStateObject._characterSkinActiveIndex.Value].skinPrefab, humanoidMannequin.gfxParent).transform.SetAsFirstSibling();
                    humanoidMannequin.gfxParent.GetChild(0).tag = "wholeBodySkin";

                    humanoidMannequin.gfxParent.gameObject.FindComponentInChildWithTag<Transform>("defaultSkin").gameObject.SetActive(false);

                    humanoidMannequin.animator.Rebind();
                    break;
                }
            default:
                {
                    Debug.Log("this enum was not assigned for");
                    break;
                }
        }

        humanoidMannequin.defaultSkinRenderer.material.color = allSkins.colorSkins.availableSkins[saveStateObject._colorSkinActiveIndex.Value].skinColor;


        Transform wholeBody = humanoidMannequin.gfxParent.gameObject.FindComponentInChildWithTag<Transform>("wholeBodySkin");
        if (wholeBody != null)
        {
            Renderer rend = wholeBody.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                foreach (Material material in rend.materials)
                {
                    if (material.name == "Humanoid.001 (Instance)")
                    {
                        material.color = allSkins.colorSkins.availableSkins[saveStateObject._colorSkinActiveIndex.Value].skinColor;
                        break;
                    }
                }
            }
        }
    }

    public void equipBombMannequin()
    {
        // destroy previously spawn shit
        for (int i = bombMannequin.gfxParent.childCount - 1; i >= 0; i--)
        {
            Destroy(bombMannequin.gfxParent.GetChild(i).gameObject);
        }

        Instantiate(allSkins.bombSkins.availableSkins[saveStateObject._bombSkinActiveIndex.Value].skinPrefab, bombMannequin.gfxParent);
    }

    public void equipPickUpMannequin()
    {
        // destroy previously spawn shit
        for (int i = pickUpMannequin.gfxParent.childCount - 1; i >= 0; i--)
        {
            Destroy(pickUpMannequin.gfxParent.GetChild(i).gameObject);
        }

        GameObject go = Instantiate(allSkins.pickUpSkins.availableSkins[saveStateObject._pickUpSkinActiveIndex.Value].skinPrefab, pickUpMannequin.gfxParent);

        go.GetComponent<Renderer>().material.color = allSkins.colorSkins.availableSkins[saveStateObject._colorSkinActiveIndex.Value].skinColor;
    }

    public void hideShowcaseChildren(bool shouldHide, ShowCases showCase)
    {
        if (shouldHide)
        {
            switch (showCase)
            {
                case ShowCases.AllOfIt:
                    {
                        humanoidMannequin.showcase.SetActive(false);
                        bombMannequin.showcase.SetActive(false);
                        pickUpMannequin.showcase.SetActive(false);
                        break;
                    }
                default:
                    {
                        Debug.Log("You did not account for this shit to happen.");
                        break;
                    }
            }
        }
        else
        {
            switch (showCase)
            {
                case ShowCases.Humanoid:
                    {
                        humanoidMannequin.showcase.SetActive(true);
                        bombMannequin.showcase.SetActive(false);
                        pickUpMannequin.showcase.SetActive(false);
                        break;
                    }
                case ShowCases.Bomb:
                    {
                        humanoidMannequin.showcase.SetActive(false);
                        bombMannequin.showcase.SetActive(true);
                        pickUpMannequin.showcase.SetActive(false);
                        break;
                    }
                case ShowCases.PickUp:
                    {
                        humanoidMannequin.showcase.SetActive(false);
                        bombMannequin.showcase.SetActive(false);
                        pickUpMannequin.showcase.SetActive(true);
                        break;
                    }
                case ShowCases.AllOfIt:
                    {
                        humanoidMannequin.showcase.SetActive(false);
                        bombMannequin.showcase.SetActive(false);
                        pickUpMannequin.showcase.SetActive(false);
                        break;
                    }
                default:
                    {
                        Debug.Log("You did not account for this shit to happen.");
                        break;
                    }
            }
        }
    }

    public void doFireworks()
    {
        Instantiate(purchaseFireworkPS, fireworksSpawnTransform);       
    }

}
