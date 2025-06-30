using UnityEngine;
using MyBox;

public enum SubMenuChildren
{
    AllOfIt,
    Settings,
    Maps,
    Customize,
    Store,
}

public class UserInterfaceSubMenus : MonoBehaviour
{
    public UserInterfaceMainMenu mainMenu;

    [Foldout("Child Scripts", true)]
    public UserInterfaceSubMenus_Settings settings;
    public UserInterfaceSubMenus_Maps maps;
    public UserInterfaceSubMenus_Customize customize;
    public UserInterfaceSubMenus_Store store;

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
        hideSelfChildren(true, SubMenuChildren.AllOfIt);
    }

    public void hideSelfChildren(bool yes, SubMenuChildren subMenuChildren)
    {
        if (yes)
        {
            switch (subMenuChildren)
            {
                default:
                    {
                        Debug.LogWarning("Do not try to individually hide sub menus");
                        break;
                    }
                case SubMenuChildren.AllOfIt:
                    {
                        settings.gameObject.SetActive(false);
                        maps.gameObject.SetActive(false);
                        customize.gameObject.SetActive(false);
                        store.gameObject.SetActive(false);
                        break;
                    }
            }
        }
        else
        {
            switch (subMenuChildren)
            {
                case SubMenuChildren.Settings:
                    {
                        settings.gameObject.SetActive(true);
                        maps.gameObject.SetActive(false);
                        customize.gameObject.SetActive(false);
                        store.gameObject.SetActive(false);
                        break;
                    }
                case SubMenuChildren.Maps:
                    {
                        settings.gameObject.SetActive(false);
                        maps.gameObject.SetActive(true);
                        customize.gameObject.SetActive(false);
                        store.gameObject.SetActive(false);
                        break;
                    }
                case SubMenuChildren.Customize:
                    {
                        settings.gameObject.SetActive(false);
                        maps.gameObject.SetActive(false);
                        customize.gameObject.SetActive(true);
                        store.gameObject.SetActive(false);
                        break;
                    }
                case SubMenuChildren.Store:
                    {
                        settings.gameObject.SetActive(false);
                        maps.gameObject.SetActive(false);
                        customize.gameObject.SetActive(false);
                        store.gameObject.SetActive(true);
                        break;
                    }
                case SubMenuChildren.AllOfIt:
                    {
                        Debug.LogWarning("You cannot show every sub menu at the same time.");
                        break;
                    }
            }
        }        
    }
}
