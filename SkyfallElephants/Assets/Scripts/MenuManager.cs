using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager i;

    [SerializeField] private Menu[] menus;

    [SerializeField] private Menu openMenu;
    [SerializeField] private Menu previousMenu;

    private void Awake()
    {
        if (i == null) i = this;
        else Destroy(gameObject);

        CloseAllMenus();
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                if (menus[i].isActive) 
                    return;

                CloseAllMenus();

                previousMenu = openMenu;
                openMenu = menus[i];
                openMenu.Open();

                return;
            }
        }
        Debug.LogWarning($"Menu with name {menuName} not found.");
    }

    public void OpenMenu(Menu menu)
    {
        if (menu.isActive) 
            return;

        CloseAllMenus();
        previousMenu = openMenu;
        openMenu = menu;
        openMenu.Open();
    }

    public void OpenPreviousMenu()
    {
        if (previousMenu != null)
        {
            OpenMenu(previousMenu);
        }
        else
        {
            OpenMenu("Main");
        }
    }

    private void CloseAllMenus()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].Close();
        }
    }

    public Menu GetMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                return menus[i];
            }
        }
        Debug.LogWarning($"Menu with name {menuName} not found.");
        return null;
    }
}
