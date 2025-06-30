using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShowCases
{
    Humanoid,
    Bomb,
    PickUp,
    AllOfIt,
}


[System.Serializable]
public class ChooseCustomizationButtons
{
    public enum CustomizationButtons
    {
        characterSkinButton,
        bombSkinButton,
        pickUpSkinButton,
        colorSkinButton,
    }

    public Button characterSkinButton;
    public Button bombSkinButton;
    public Button pickUpSkinButton;
    public Button colorSkinButton;

    public Color32 originalColor, highlightColor;
    public Vector3 originalSize, highlightSize;
    public float highlightBottomValue;


    [HideInInspector] public Vector4 originalLRTB;

    private static bool initComplete = false;

    public void init()
    {
        if (initComplete)
        {
            return;
        }
        float originalLeft = characterSkinButton.transform.GetChild(1).GetComponent<RectTransform>().offsetMin.x;
        float originalRight = -characterSkinButton.transform.GetChild(1).GetComponent<RectTransform>().offsetMax.x;
        float originalTop = -characterSkinButton.transform.GetChild(1).GetComponent<RectTransform>().offsetMax.y;
        float originalBottom = characterSkinButton.transform.GetChild(1).GetComponent<RectTransform>().offsetMin.y;
        originalLRTB = new Vector4(originalLeft, originalRight, originalTop, originalBottom);

        initComplete = true;
    }
}

[System.Serializable]
public class Buttons
{
    public GameObject nullButton;
    public GameObject availableButton;
    public GameObject skinColorButton;
    public GameObject workInProgressButton;
    public GameObject comingSoonButton;
}

[System.Serializable]
public class RewardTarget
{
    public SaveState.Unlockable targetPurchase;
    public int targetCurrentIndex;

    public RewardTarget()
    {
        targetPurchase = SaveState.Unlockable.Null;
        targetCurrentIndex = int.MinValue;
    }

    public RewardTarget(SaveState.Unlockable purchasable, int index)
    {
        targetPurchase = purchasable;
        targetCurrentIndex = index;
    }
}

public class UserInterfaceSubMenus_Customize : MonoBehaviour
{
    [System.Serializable]
    public class Scrollers
    {
        public GameObject characterSkinsScroller;
        public GameObject bombSkinsScroller;
        public GameObject pickUpSkinsScroller;
        public GameObject colorSkinsScroller;

        public Grids grids;
        public class Grids
        {
            public GameObject characterSkinsGrid;
            public GameObject bombSkinsGrid;
            public GameObject pickUpSkinsGrid;
            public GameObject colorSkinsGrid;
        }

        public void init()
        {
            grids = new Grids();

            grids.characterSkinsGrid = characterSkinsScroller.transform.GetChild(0).GetChild(0).gameObject;
            grids.bombSkinsGrid = bombSkinsScroller.transform.GetChild(0).GetChild(0).gameObject;
            grids.pickUpSkinsGrid = pickUpSkinsScroller.transform.GetChild(0).GetChild(0).gameObject;
            grids.colorSkinsGrid = colorSkinsScroller.transform.GetChild(0).GetChild(0).gameObject;
        }
    }

    [SerializeField] private UserInterfaceMainMenu mainMenu;
    [SerializeField] private UserInterfaceSubMenus subMenus;

    [SerializeField] private GameObject customizeCanvas;

    private RewardTarget rewardTarget = new RewardTarget();

    [SerializeField] UserInterfaceSubMenus_Showcases showcases;

    [Foldout("Customization", true)]
    public Scrollers customizationScrollers;

    public ChooseCustomizationButtons customizationButtons;

    public GameObject costGoldPanel;
    public GameObject costVideoPanel;

    public Color32 normalColor, highlightColor;

    public Buttons buttons;

    [Foldout("Scriptable Objects Stuff", true)]
    public AllSkins allSkins;
    public SaveStateObject saveStateObject;

    [Separator("Void Events", true)]
    public VoidEvent OnCloseCustomizationPanel;
    public VoidEvent OnRewardedAdButton;

    [Separator("Humanoid Equip Events", true)]
    public HumanoidEquipEvent equipSkinEvent;
    public HumanoidEquipEvent equipColorEvent;
    

    [Separator("SaveStateChange Event", true)]
    public SaveStateChangeEvent saveStateChangeEvent;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        initializeCustomizeScreen();
    }
    private void initializeCustomizeScreen()
    {
        customizationScrollers.init();
        customizationButtons.init();

        showcases.equipHumanoidMannequin();

        doCharacterSkin();
        void doCharacterSkin()
        {
            for (int i = customizationScrollers.grids.characterSkinsGrid.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(customizationScrollers.grids.characterSkinsGrid.transform.GetChild(i).gameObject);
            }

            int j = 0;
            // AVAILABLE SKINS
            for (int i = 0; i < allSkins.characterSkins.availableSkins.Count; i++)
            {
                GameObject button = Instantiate(buttons.availableButton, customizationScrollers.grids.characterSkinsGrid.transform) as GameObject;
                button.GetComponent<AvailableSkinButton>().prepButtonVisuals(i, saveStateObject.isUnlocked(SaveState.Unlockable.CharacterSkins, i), allSkins.characterSkins.availableSkins[i], allSkins.characterSkins.availableSkins[i].sprite, SaveState.Unlockable.CharacterSkins);

                int index = j;
                button.GetComponent<Button>().onClick.AddListener(() => onCharacterSkinSelect(index));
                //go.GetComponent<Button>().onClick.AddListener(() => onCharacterSkinSelect2(index));
                j++;
            }

            // WORK-IN PROGRESS
            for (int i = 0; i < allSkins.characterSkins.workInProgressSkins.Count; i++)
            {
                GameObject go = Instantiate(buttons.workInProgressButton, customizationScrollers.grids.characterSkinsGrid.transform) as GameObject;

                int index = j;
                Destroy(go.GetComponent<Button>());
                //go.GetComponent<Button>().onClick.AddListener(() => AddButtonListener.onCharacterSkinSelect(index));
                //j++;
            }

            // COMING SOON SKINS
            for (int i = 0; i < allSkins.characterSkins.comingSoonSkins.Count; i++)
            {
                GameObject go = Instantiate(buttons.comingSoonButton, customizationScrollers.grids.characterSkinsGrid.transform) as GameObject;

                int index = j;
                Destroy(go.GetComponent<Button>());
                //go.GetComponent<Button>().onClick.AddListener(() => AddButtonListener.onCharacterSkinSelect(index));
                //j++;
            }

            // NULL SKINS FOR GRID CORRECTION
            for (int i = 0; i < 4; i++)
            {
                GameObject go = Instantiate(buttons.nullButton, customizationScrollers.grids.characterSkinsGrid.transform) as GameObject;
            }
        }

        doPickUpSkin();
        void doPickUpSkin()
        {
            for (int i = customizationScrollers.grids.pickUpSkinsGrid.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(customizationScrollers.grids.pickUpSkinsGrid.transform.GetChild(i).gameObject);
            }

            int j = 0;
            // AVAILABLE SKINS
            for (int i = 0; i < allSkins.pickUpSkins.availableSkins.Count; i++)
            {
                GameObject button = Instantiate(buttons.availableButton, customizationScrollers.grids.pickUpSkinsGrid.transform) as GameObject;
                button.GetComponent<AvailableSkinButton>().prepButtonVisuals(i, saveStateObject.isUnlocked(SaveState.Unlockable.PickUpSkins, i), allSkins.pickUpSkins.availableSkins[i], allSkins.pickUpSkins.availableSkins[i].sprite, SaveState.Unlockable.PickUpSkins);

                int index = j;
                button.GetComponent<Button>().onClick.AddListener(() => onPickUpsSkinSelect(index));
                j++;
            }

            // WORK-IN PROGRESS
            for (int i = 0; i < allSkins.pickUpSkins.workInProgressSkins.Count; i++)
            {
                GameObject go = Instantiate(buttons.workInProgressButton, customizationScrollers.grids.pickUpSkinsGrid.transform) as GameObject;

                int index = j;
                Destroy(go.GetComponent<Button>());
                //go.GetComponent<Button>().onClick.AddListener(() => AddButtonListener.onPickUpsSkinSelect(index));
                //j++;
            }

            // COMING SOON SKINS
            for (int i = 0; i < allSkins.pickUpSkins.comingSoonSkins.Count; i++)
            {
                GameObject go = Instantiate(buttons.comingSoonButton, customizationScrollers.grids.pickUpSkinsGrid.transform) as GameObject;

                int index = j;
                Destroy(go.GetComponent<Button>());
                //go.GetComponent<Button>().onClick.AddListener(() => AddButtonListener.onPickUpsSkinSelect(index));
                //j++;
            }

            // NULL SKINS
            for (int i = 0; i < 4; i++)
            {
                GameObject go = Instantiate(buttons.nullButton, customizationScrollers.grids.pickUpSkinsGrid.transform) as GameObject;
            }
        }

        doBombSkin();
        void doBombSkin()
        {
            for (int i = customizationScrollers.grids.bombSkinsGrid.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(customizationScrollers.grids.bombSkinsGrid.transform.GetChild(i).gameObject);
            }

            int j = 0;
            // AVAILABLE SKINS
            for (int i = 0; i < allSkins.bombSkins.availableSkins.Count; i++)
            {
                GameObject button = Instantiate(buttons.availableButton, customizationScrollers.grids.bombSkinsGrid.transform) as GameObject;
                button.GetComponent<AvailableSkinButton>().prepButtonVisuals(i, saveStateObject.isUnlocked(SaveState.Unlockable.BombSkins, i), allSkins.bombSkins.availableSkins[i], allSkins.bombSkins.availableSkins[i].sprite, SaveState.Unlockable.BombSkins);

                int index = j;
                button.GetComponent<Button>().onClick.AddListener(() => onBombSkinSelect(index));
                j++;
            }

            // WORK-IN PROGRESS
            for (int i = 0; i < allSkins.bombSkins.workInProgressSkins.Count; i++)
            {
                GameObject go = Instantiate(buttons.workInProgressButton, customizationScrollers.grids.bombSkinsGrid.transform) as GameObject;

                int index = j;
                Destroy(go.GetComponent<Button>());
                //go.GetComponent<Button>().onClick.AddListener(() => AddButtonListener.onBombSkinSelect(index));
                //j++;
            }

            // COMING SOON SKINS
            for (int i = 0; i < allSkins.bombSkins.comingSoonSkins.Count; i++)
            {
                GameObject go = Instantiate(buttons.comingSoonButton, customizationScrollers.grids.bombSkinsGrid.transform);

                int index = j;
                Destroy(go.GetComponent<Button>());
                //go.GetComponent<Button>().onClick.AddListener(() => AddButtonListener.onBombSkinSelect(index));
                //j++;
            }

            // NULL SKINS
            for (int i = 0; i < 4; i++)
            {
                GameObject go = Instantiate(buttons.nullButton, customizationScrollers.grids.bombSkinsGrid.transform);
            }
        }

        doColorSkin();
        void doColorSkin()
        {
            // CLEAN
            for (int i = customizationScrollers.grids.colorSkinsGrid.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(customizationScrollers.grids.colorSkinsGrid.transform.GetChild(i).gameObject);
            }

            int j = 0;
            // AVAILABLE SKINS
            for (int i = 0; i < allSkins.colorSkins.availableSkins.Count; i++)
            {
                GameObject button = Instantiate(buttons.skinColorButton, customizationScrollers.grids.colorSkinsGrid.transform) as GameObject;
                button.GetComponent<SkinColorButton>().prepButtonVisuals(i, saveStateObject.isUnlocked(SaveState.Unlockable.ColorSkins, i), allSkins.colorSkins.availableSkins[i], allSkins.colorSkins.availableSkins[i].skinColor);

                int index = j;
                button.GetComponent<Button>().onClick.AddListener(() => onColorSkinSelect(index));
                j++;
            }

            // WORK-IN PROGRESS
            for (int i = 0; i < allSkins.colorSkins.workInProgressSkins.Count; i++)
            {
                GameObject go = Instantiate(buttons.workInProgressButton, customizationScrollers.grids.colorSkinsGrid.transform) as GameObject;

                int index = j;
                Destroy(go.GetComponent<Button>());
                //go.GetComponent<Button>().onClick.AddListener(() => AddButtonListener.onColorSkinSelect(index));
                //j++;
            }

            // COMING SOON SKINS
            for (int i = 0; i < allSkins.colorSkins.comingSoonSkins.Count; i++)
            {
                GameObject go = Instantiate(buttons.comingSoonButton, customizationScrollers.grids.colorSkinsGrid.transform) as GameObject;

                int index = j;
                Destroy(go.GetComponent<Button>());
                //go.GetComponent<Button>().onClick.AddListener(() => AddButtonListener.onColorSkinSelect(index));
                //j++;
            }

            // NULL SKINS
            for (int i = 0; i < 4; i++)
            {
                GameObject go = Instantiate(buttons.nullButton, customizationScrollers.grids.colorSkinsGrid.transform) as GameObject;
            }
        }

        highlightActive();
    }

    private void OnEnable()
    {
        showcases.gameObject.SetActive(true);

        customizeCanvas.SetActive(true);
        highlightCustomizationButtons(ChooseCustomizationButtons.CustomizationButtons.characterSkinButton);
    }

    private void highlightActive()
    {
        customizationScrollers.grids.characterSkinsGrid.transform.GetChild(saveStateObject._characterSkinActiveIndex.Value).GetChild(0).GetComponent<Image>().color = highlightColor;
        customizationScrollers.grids.bombSkinsGrid.transform.GetChild(saveStateObject._bombSkinActiveIndex.Value).GetChild(0).GetComponent<Image>().color = highlightColor;
        customizationScrollers.grids.pickUpSkinsGrid.transform.GetChild(saveStateObject._pickUpSkinActiveIndex.Value).GetChild(0).GetComponent<Image>().color = highlightColor;
        customizationScrollers.grids.colorSkinsGrid.transform.GetChild(saveStateObject._colorSkinActiveIndex.Value).GetChild(0).GetComponent<Image>().color = highlightColor;
    }

    public void highlightCustomizationButtons(ChooseCustomizationButtons.CustomizationButtons button)
    {
        if (customizationButtons.originalLRTB == null)
        {
            customizationButtons.init();
        }

        customizationButtons.characterSkinButton.enabled = true;
        customizationButtons.characterSkinButton.transform.GetChild(0).gameObject.SetActive(false);
        customizationButtons.characterSkinButton.GetComponent<RectTransform>().localScale = customizationButtons.originalSize;
        customizationButtons.characterSkinButton.transform.GetChild(1).GetComponent<Image>().color = customizationButtons.originalColor;
        customizationButtons.characterSkinButton.transform.GetChild(1).GetComponent<RectTransform>().changeRectTransform(customizationButtons.originalLRTB.x, customizationButtons.originalLRTB.y, customizationButtons.originalLRTB.z, customizationButtons.originalLRTB.w);
        customizationScrollers.characterSkinsScroller.SetActive(false);

        customizationButtons.bombSkinButton.enabled = true;
        customizationButtons.bombSkinButton.transform.GetChild(0).gameObject.SetActive(false);
        customizationButtons.bombSkinButton.GetComponent<RectTransform>().localScale = customizationButtons.originalSize;
        customizationButtons.bombSkinButton.transform.GetChild(1).GetComponent<Image>().color = customizationButtons.originalColor;
        customizationButtons.bombSkinButton.transform.GetChild(1).GetComponent<RectTransform>().changeRectTransform(customizationButtons.originalLRTB.x, customizationButtons.originalLRTB.y, customizationButtons.originalLRTB.z, customizationButtons.originalLRTB.w);
        customizationScrollers.bombSkinsScroller.SetActive(false);

        customizationButtons.pickUpSkinButton.enabled = true;
        customizationButtons.pickUpSkinButton.transform.GetChild(0).gameObject.SetActive(false);
        customizationButtons.pickUpSkinButton.GetComponent<RectTransform>().localScale = customizationButtons.originalSize;
        customizationButtons.pickUpSkinButton.transform.GetChild(1).GetComponent<Image>().color = customizationButtons.originalColor;
        customizationButtons.pickUpSkinButton.transform.GetChild(1).GetComponent<RectTransform>().changeRectTransform(customizationButtons.originalLRTB.x, customizationButtons.originalLRTB.y, customizationButtons.originalLRTB.z, customizationButtons.originalLRTB.w);
        customizationScrollers.pickUpSkinsScroller.SetActive(false);

        customizationButtons.colorSkinButton.enabled = true;
        customizationButtons.colorSkinButton.transform.GetChild(0).gameObject.SetActive(false);
        customizationButtons.colorSkinButton.GetComponent<RectTransform>().localScale = customizationButtons.originalSize;
        customizationButtons.colorSkinButton.transform.GetChild(1).GetComponent<Image>().color = customizationButtons.originalColor;
        customizationButtons.colorSkinButton.transform.GetChild(1).GetComponent<RectTransform>().changeRectTransform(customizationButtons.originalLRTB.x, customizationButtons.originalLRTB.y, customizationButtons.originalLRTB.z, customizationButtons.originalLRTB.w);
        customizationScrollers.colorSkinsScroller.SetActive(false);

        switch (button)
        {
            case ChooseCustomizationButtons.CustomizationButtons.characterSkinButton:
                {
                    customizationButtons.characterSkinButton.enabled = false;
                    customizationButtons.characterSkinButton.GetComponent<RectTransform>().localScale = customizationButtons.highlightSize;

                    customizationButtons.characterSkinButton.transform.GetChild(0).gameObject.SetActive(true);
                    customizationButtons.characterSkinButton.transform.GetChild(1).GetComponent<Image>().color = customizationButtons.highlightColor;

                    customizationButtons.characterSkinButton.transform.GetChild(1).GetComponent<RectTransform>().SetBottom(customizationButtons.highlightBottomValue);

                    customizationScrollers.characterSkinsScroller.SetActive(true);
                }
                break;
            case ChooseCustomizationButtons.CustomizationButtons.bombSkinButton:
                {
                    customizationButtons.bombSkinButton.enabled = false;
                    customizationButtons.bombSkinButton.GetComponent<RectTransform>().localScale = customizationButtons.highlightSize;

                    customizationButtons.bombSkinButton.transform.GetChild(0).gameObject.SetActive(true);
                    customizationButtons.bombSkinButton.transform.GetChild(1).GetComponent<Image>().color = customizationButtons.highlightColor;

                    customizationButtons.bombSkinButton.transform.GetChild(1).GetComponent<RectTransform>().SetBottom(customizationButtons.highlightBottomValue);

                    customizationScrollers.bombSkinsScroller.SetActive(true);
                }
                break;
            case ChooseCustomizationButtons.CustomizationButtons.pickUpSkinButton:
                {
                    customizationButtons.pickUpSkinButton.enabled = false;
                    customizationButtons.pickUpSkinButton.GetComponent<RectTransform>().localScale = customizationButtons.highlightSize;

                    customizationButtons.pickUpSkinButton.transform.GetChild(0).gameObject.SetActive(true);
                    customizationButtons.pickUpSkinButton.transform.GetChild(1).GetComponent<Image>().color = customizationButtons.highlightColor;

                    customizationButtons.pickUpSkinButton.transform.GetChild(1).GetComponent<RectTransform>().SetBottom(customizationButtons.highlightBottomValue);

                    customizationScrollers.pickUpSkinsScroller.SetActive(true);
                }
                break;
            case ChooseCustomizationButtons.CustomizationButtons.colorSkinButton:
                {
                    customizationButtons.colorSkinButton.enabled = false;
                    customizationButtons.colorSkinButton.GetComponent<RectTransform>().localScale = customizationButtons.highlightSize;

                    customizationButtons.colorSkinButton.transform.GetChild(0).gameObject.SetActive(true);
                    customizationButtons.colorSkinButton.transform.GetChild(1).GetComponent<Image>().color = customizationButtons.highlightColor;

                    customizationButtons.colorSkinButton.transform.GetChild(1).GetComponent<RectTransform>().SetBottom(customizationButtons.highlightBottomValue);

                    customizationScrollers.colorSkinsScroller.SetActive(true);
                }
                break;
        }
    }

    public void equipSelection(SaveState.Unlockable toBeEquipped, int equipIndex)
    {
        switch (toBeEquipped)
        {
            case SaveState.Unlockable.CharacterSkins:
                equipCharacterSkin();
                break;
            case SaveState.Unlockable.PickUpSkins:
                equipPickUpSkin();
                break;
            case SaveState.Unlockable.BombSkins:
                equipBombSkin();
                break;
            case SaveState.Unlockable.ColorSkins:
                equipColorSkin();
                break;            
        }

        void equipCharacterSkin()
        {
            saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.characterSkinActive, equipIndex));

            equipSkinEvent.Raise(new HumanoidEquip(allSkins.characterSkins.availableSkins[equipIndex].skinType, allSkins.characterSkins.availableSkins[equipIndex].skinPrefab));

            equipColorEvent.Raise(new HumanoidEquip(allSkins.colorSkins.availableSkins[saveStateObject._colorSkinActiveIndex.Value].skinColor));
        }

        void equipPickUpSkin()
        {
            saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.pickUpSkinActive, equipIndex));
        }

        void equipBombSkin()
        {
            saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.bombSkinActive, equipIndex));
        }

        void equipColorSkin()
        {
            saveStateChangeEvent.Raise(new SaveStateChange(SaveState.SaveStateChangeableVariables.colorSkinActive, equipIndex));
            //saveManager.changeSaveState(SaveState.SaveStateChangeableVariables.colorSkinActive, equipIndex);

            equipColorEvent.Raise(new HumanoidEquip(allSkins.colorSkins.availableSkins[saveStateObject._colorSkinActiveIndex.Value].skinColor));
            //playerController.equipNewColorSkin(saveManager.colorSkins.availableSkins[equipIndex].skinColor);            
        }
    }

    private void dehighlightActive(int index, SaveState.Unlockable which)
    {
        switch (which)
        {
            case SaveState.Unlockable.CharacterSkins:
                {
                    customizationScrollers.grids.characterSkinsGrid.transform.GetChild(index).GetChild(0).GetComponent<Image>().color = normalColor;
                    break;
                }
            case SaveState.Unlockable.PickUpSkins:
                {
                    customizationScrollers.grids.pickUpSkinsGrid.transform.GetChild(index).GetChild(0).GetComponent<Image>().color = normalColor;
                    break;
                }
            case SaveState.Unlockable.BombSkins:
                {
                    customizationScrollers.grids.bombSkinsGrid.transform.GetChild(index).GetChild(0).GetComponent<Image>().color = normalColor;
                    break;
                }
            case SaveState.Unlockable.ColorSkins:
                {
                    customizationScrollers.grids.colorSkinsGrid.transform.GetChild(index).GetChild(0).GetComponent<Image>().color = normalColor;
                    break;
                }           
            default:
                break;
        }
    }

    public void purchase(SaveState.Unlockable toPurchase, int purchasedIndex, int cost, Transform button)
    {
        Debug.Log("fucking hell bitch");
        showcases.doFireworks();

        Unlocker.getInstance().unlockStuff(toPurchase, purchasedIndex, cost);

        if (button.FindChildWithNameBreadthFirst("costPanel") != null)
        {
            Destroy(button.FindChildWithNameBreadthFirst("costPanel").gameObject);
        }
        else
        {
            Debug.Log("cost panel name could not found in this button");
        }
    }

    private void managePurchaseLogic(SaveState.Unlockable toEquip, int purchaseIndex)
    {
        switch (toEquip)
        {
            case SaveState.Unlockable.CharacterSkins:
                {
                    dehighlightActive(saveStateObject._characterSkinActiveIndex.Value, toEquip);

                    purchase(toEquip, purchaseIndex, allSkins.characterSkins.availableSkins[purchaseIndex].cost,
                        customizationScrollers.grids.characterSkinsGrid.transform.GetChild(purchaseIndex));

                    equipSelection(toEquip, purchaseIndex);

                    highlightActive();

                    showcases.equipHumanoidMannequin();
                    break;
                }
            case SaveState.Unlockable.PickUpSkins:
                {
                    dehighlightActive(saveStateObject._pickUpSkinActiveIndex.Value, toEquip);

                    purchase(toEquip, purchaseIndex, allSkins.pickUpSkins.availableSkins[purchaseIndex].cost,
                        customizationScrollers.grids.pickUpSkinsGrid.transform.GetChild(purchaseIndex));

                    equipSelection(toEquip, purchaseIndex);

                    highlightActive();

                    showcases.equipPickUpMannequin();
                    break;
                }
            case SaveState.Unlockable.BombSkins:
                {
                    dehighlightActive(saveStateObject._bombSkinActiveIndex.Value, toEquip);

                    purchase(toEquip, purchaseIndex, allSkins.bombSkins.availableSkins[purchaseIndex].cost,
                        customizationScrollers.grids.bombSkinsGrid.transform.GetChild(purchaseIndex));

                    equipSelection(toEquip, purchaseIndex);

                    highlightActive();

                    showcases.equipBombMannequin();
                    break;
                }
            case SaveState.Unlockable.ColorSkins:
                {
                    dehighlightActive(saveStateObject._colorSkinActiveIndex.Value, toEquip);

                    purchase(toEquip, purchaseIndex, allSkins.colorSkins.availableSkins[purchaseIndex].cost,
                        customizationScrollers.grids.colorSkinsGrid.transform.GetChild(purchaseIndex));

                    equipSelection(toEquip, purchaseIndex);

                    highlightActive();

                    showcases.equipHumanoidMannequin();
                    break;
                }
            case SaveState.Unlockable.Null:
                {
                    Debug.LogWarning("this should not have happened at all.");
                    break;
                }
            default:
                break;
        }
    }

    public void rewardVideoWatched()
    {
        switch (rewardTarget.targetPurchase)
        {
            case SaveState.Unlockable.CharacterSkins:
                {
                    allSkins.characterSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched++;

                    if (allSkins.characterSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched >= allSkins.characterSkins.availableSkins[rewardTarget.targetCurrentIndex].videoCost)
                    {
                        managePurchaseLogic(SaveState.Unlockable.CharacterSkins, rewardTarget.targetCurrentIndex);
                    }
                    else
                    {
                        Transform button = customizationScrollers.grids.characterSkinsGrid.transform.GetChild(rewardTarget.targetCurrentIndex);

                        TextMeshProUGUI videoPercentText = button.gameObject.FindChildWithTagBreadthFirst<TextMeshProUGUI>("cost");
                        videoPercentText.text = allSkins.characterSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched.ToString() + "/" + allSkins.characterSkins.availableSkins[rewardTarget.targetCurrentIndex].videoCost.ToString();

                    }

                    break;
                }
            case SaveState.Unlockable.PickUpSkins:
                {
                    allSkins.pickUpSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched++;

                    if (allSkins.pickUpSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched >= allSkins.pickUpSkins.availableSkins[rewardTarget.targetCurrentIndex].videoCost)
                    {
                        managePurchaseLogic(SaveState.Unlockable.PickUpSkins, rewardTarget.targetCurrentIndex);
                    }
                    else
                    {
                        Transform button = customizationScrollers.grids.pickUpSkinsGrid.transform.GetChild(rewardTarget.targetCurrentIndex);

                        TextMeshProUGUI videoPercentText = button.gameObject.FindChildWithTagBreadthFirst<TextMeshProUGUI>("cost");
                        videoPercentText.text = allSkins.pickUpSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched.ToString() + "/" + allSkins.pickUpSkins.availableSkins[rewardTarget.targetCurrentIndex].videoCost.ToString();

                    }

                    break;
                }
            case SaveState.Unlockable.BombSkins:
                {
                    allSkins.bombSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched++;

                    if (allSkins.bombSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched >= allSkins.bombSkins.availableSkins[rewardTarget.targetCurrentIndex].videoCost)
                    {
                        managePurchaseLogic(SaveState.Unlockable.BombSkins, rewardTarget.targetCurrentIndex);
                    }
                    else
                    {
                        Transform button = customizationScrollers.grids.bombSkinsGrid.transform.GetChild(rewardTarget.targetCurrentIndex);

                        TextMeshProUGUI videoPercentText = button.gameObject.FindChildWithTagBreadthFirst<TextMeshProUGUI>("cost");
                        videoPercentText.text = allSkins.bombSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched.ToString() + "/" + allSkins.bombSkins.availableSkins[rewardTarget.targetCurrentIndex].videoCost.ToString();

                    }

                    break;
                }
            case SaveState.Unlockable.ColorSkins:
                {
                    allSkins.colorSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched++;

                    if (allSkins.colorSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched >= allSkins.colorSkins.availableSkins[rewardTarget.targetCurrentIndex].videoCost)
                    {
                        managePurchaseLogic(SaveState.Unlockable.ColorSkins, rewardTarget.targetCurrentIndex);
                    }
                    else
                    {
                        Transform button = customizationScrollers.grids.colorSkinsGrid.transform.GetChild(rewardTarget.targetCurrentIndex);

                        TextMeshProUGUI videoPercentText = button.gameObject.FindChildWithTagBreadthFirst<TextMeshProUGUI>("cost");
                        videoPercentText.text = allSkins.colorSkins.availableSkins[rewardTarget.targetCurrentIndex].videoWatched.ToString() + "/" + allSkins.colorSkins.availableSkins[rewardTarget.targetCurrentIndex].videoCost.ToString();

                    }

                    break;
                }           
            case SaveState.Unlockable.Null:
                {
                    Debug.Log("This should NEVER have happened.");
                    break;
                }
        }
    }

    public void rewardVideoWASNTwatched()
    {
        rewardTarget = new RewardTarget();
    }

    #region BUTTONS

    #region SKIN SELECT BUTTONS
    public void onCustomizationButtonPressed(CustomizationButtonSelect customizationButtonSelect)
    {
        Debug.Log("altough this makes it a little bit more better " +
            "in a way that decoupling, it doesnt help jak shit with generic use of" +
            "the 4 methods below. figure it out pls.");
    }

    public void onCharacterSkinSelect(int currentIndex)
    {
        if (saveStateObject.isUnlocked(SaveState.Unlockable.CharacterSkins, currentIndex))
        {
            dehighlightActive(saveStateObject._characterSkinActiveIndex.Value, SaveState.Unlockable.CharacterSkins);

            equipSelection(SaveState.Unlockable.CharacterSkins, currentIndex);

            highlightActive();

            showcases.equipHumanoidMannequin();
        }
        else
        {
            if (allSkins.characterSkins.availableSkins[currentIndex].isCostVideo)
            {
                if (allSkins.characterSkins.availableSkins[currentIndex].videoWatched < allSkins.characterSkins.availableSkins[currentIndex].videoCost)
                {
                    rewardTarget = new RewardTarget(SaveState.Unlockable.CharacterSkins, currentIndex);
                    OnRewardedAdButton.Raise();
                }
            }
            else
            {
                if (allSkins.characterSkins.availableSkins[currentIndex].cost <= saveStateObject._goldAmount.Value)
                {
                    managePurchaseLogic(SaveState.Unlockable.CharacterSkins, currentIndex);
                }
                else
                {
                    Debug.Log("Not enough gold, gold amount:  " + saveStateObject._goldAmount.Value);
                }
            }
        }
    }
    public void onBombSkinSelect(int currentIndex)
    {
        if (saveStateObject.isUnlocked(SaveState.Unlockable.BombSkins, currentIndex))
        {
            dehighlightActive(saveStateObject._bombSkinActiveIndex.Value, SaveState.Unlockable.BombSkins);

            equipSelection(SaveState.Unlockable.BombSkins, currentIndex);

            highlightActive();

            showcases.equipBombMannequin();
        }
        else
        {
            if (allSkins.bombSkins.availableSkins[currentIndex].isCostVideo)
            {
                if (allSkins.bombSkins.availableSkins[currentIndex].videoWatched < allSkins.bombSkins.availableSkins[currentIndex].videoCost)
                {
                    rewardTarget = new RewardTarget(SaveState.Unlockable.BombSkins, currentIndex);
                    OnRewardedAdButton.Raise();
                }
            }
            else
            {
                if (allSkins.bombSkins.availableSkins[currentIndex].cost <= saveStateObject._goldAmount.Value)
                {
                    managePurchaseLogic(SaveState.Unlockable.BombSkins, currentIndex);
                }
                else
                {
                    // nope, dont do anything
                    Debug.Log("Not enough gold, gold amount:  " + saveStateObject._goldAmount.Value);
                }
            }
        }
    }
    public void onPickUpsSkinSelect(int currentIndex)
    {
        if (saveStateObject.isUnlocked(SaveState.Unlockable.PickUpSkins, currentIndex))
        {
            dehighlightActive(saveStateObject._pickUpSkinActiveIndex.Value, SaveState.Unlockable.PickUpSkins);

            equipSelection(SaveState.Unlockable.PickUpSkins, currentIndex);

            highlightActive();

            showcases.equipPickUpMannequin();
        }
        else
        {
            if (allSkins.pickUpSkins.availableSkins[currentIndex].isCostVideo)
            {
                if (allSkins.pickUpSkins.availableSkins[currentIndex].videoWatched < allSkins.pickUpSkins.availableSkins[currentIndex].videoCost)
                {
                    rewardTarget = new RewardTarget(SaveState.Unlockable.PickUpSkins, currentIndex);
                    OnRewardedAdButton.Raise();
                }
            }
            else
            {
                if (allSkins.pickUpSkins.availableSkins[currentIndex].cost <= saveStateObject._goldAmount.Value)
                {
                    managePurchaseLogic(SaveState.Unlockable.PickUpSkins, currentIndex);
                }
                else
                {
                    // nope, dont do anything
                    Debug.Log("Not enough gold, gold amount:  " + saveStateObject._goldAmount.Value);
                }
            }
        }
    }
    public void onColorSkinSelect(int currentIndex)
    {
        if (saveStateObject.isUnlocked(SaveState.Unlockable.ColorSkins, currentIndex))
        {
            dehighlightActive(saveStateObject._colorSkinActiveIndex.Value, SaveState.Unlockable.ColorSkins);

            equipSelection(SaveState.Unlockable.ColorSkins, currentIndex);

            highlightActive();

            showcases.equipHumanoidMannequin();
        }
        else
        {
            if (allSkins.colorSkins.availableSkins[currentIndex].isCostVideo)
            {
                if (allSkins.colorSkins.availableSkins[currentIndex].videoWatched < allSkins.colorSkins.availableSkins[currentIndex].videoCost)
                {
                    rewardTarget = new RewardTarget(SaveState.Unlockable.ColorSkins, currentIndex);
                    OnRewardedAdButton.Raise();
                }
            }
            else
            {
                if (allSkins.colorSkins.availableSkins[currentIndex].cost <= saveStateObject._goldAmount.Value)
                {
                    managePurchaseLogic(SaveState.Unlockable.ColorSkins, currentIndex);
                }
                else
                {
                    // nope, dont do anything
                    Debug.Log("Not enough gold, gold amount:  " + saveStateObject._goldAmount.Value);
                }
            }
        }
    }
    #endregion

    #region CHOOSE CUSTOMIZATIN BUTTONS
    public void Button_CharacterSkin()
    {
        highlightCustomizationButtons(ChooseCustomizationButtons.CustomizationButtons.characterSkinButton);
        showcases.hideShowcaseChildren(false, ShowCases.Humanoid);
    }

    public void Button_BombSkin()
    {
        highlightCustomizationButtons(ChooseCustomizationButtons.CustomizationButtons.bombSkinButton);
        showcases.hideShowcaseChildren(false, ShowCases.Bomb);
    }

    public void Button_PickUpSkin()
    {
        highlightCustomizationButtons(ChooseCustomizationButtons.CustomizationButtons.pickUpSkinButton);
        showcases.hideShowcaseChildren(false, ShowCases.PickUp);
    }

    public void Button_ColorSkin()
    {
        highlightCustomizationButtons(ChooseCustomizationButtons.CustomizationButtons.colorSkinButton);
        showcases.hideShowcaseChildren(false, ShowCases.Humanoid);
    }
    #endregion

    public void Button_OnCloseCustomizeCanvas()
    {
        OnCloseCustomizationPanel.Raise();

        mainMenu.hideChildren(false);
        mainMenu.toggleTapToPlay(true);

        this.gameObject.SetActive(false);
        showcases.gameObject.SetActive(false);
        customizeCanvas.SetActive(false);
        subMenus.gameObject.SetActive(false);
    }

    #endregion
}
