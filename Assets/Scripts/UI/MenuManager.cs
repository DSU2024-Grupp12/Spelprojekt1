using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public static MenuManager Instance;
    [HideInInspector]
    public bool inMenu;

    [SerializeField]
    private Canvas gameplayOverlay;

    public static event Action OnReturnToGameplay;

    [SerializeField]
    private Menu[] menus;

    private void Awake() {
        if (Instance) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameplayOverlay.enabled = true;
        foreach (Menu m in menus) m.Close();
        playerInput.SwitchCurrentActionMap("Player");
        inMenu = false;
    }

    public void ReturnToGameplay() {
        gameplayOverlay.enabled = true;
        if (playerInput.currentActionMap.name != "Player") playerInput.SwitchCurrentActionMap("Player");
        if (inMenu) {
            foreach (Menu m in menus) m.Close();
            inMenu = false;
            OnReturnToGameplay?.Invoke();
        }
    }

    public void OpenMenu(string menuId, MenuInfo menuInfo) {
        PrepMenu(menuId).Open(menuInfo);
    }
    public void OpenMenu(string menuId) {
        PrepMenu(menuId).Open();
    }
    private Menu PrepMenu(string menuId) {
        inMenu = true;
        Menu menu = GetMenuByID(menuId);
        playerInput.SwitchCurrentActionMap(menu.actionMapID);
        CloseAllMenus();
        return menu;
    }

    public void ToggleMenuAsOverlay(string menuId, bool on) {
        ToggleMenuAsOverlay(menuId, null, on);
    }

    public void ToggleMenuAsOverlay(string menuId, MenuInfo info, bool on) {
        Menu menu = GetMenuByID(menuId);
        if (on) {
            if (info != null) menu.Open(info);
            else menu.Open();
        }
        else {
            menu.Close();
        }
    }

    public void HideGameplayOverlay() {
        gameplayOverlay.enabled = false;
    }
    public void UnhideGameplayOverlay() {
        gameplayOverlay.enabled = true;
    }

    private void CloseAllMenus() {
        foreach (Menu m in menus) m.Close();
    }
    private Menu GetMenuByID(string id) {
        return menus.Where(m => m.menuID == id).First();
    }
}