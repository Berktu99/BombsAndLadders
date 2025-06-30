using MyBox;
using UnityEngine;


public class UserInterfaceSubMenus_Store : MonoBehaviour
{
    [SerializeField] private UserInterfaceMainMenu mainMenu;
    [SerializeField] private UserInterfaceSubMenus subMenus;


    public void Button_OnCloseStoreCanvas()
    {
        mainMenu.hideChildren(false);
        mainMenu.toggleTapToPlay(true);

        this.gameObject.SetActive(false);
        subMenus.gameObject.SetActive(false);
    }
}
