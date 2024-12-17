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
        ReturnToGameplay();
    }

    public void ReturnToGameplay() {
        if (gameplayOverlay.enabled) return;
        foreach (Menu m in menus) m.Close();
        gameplayOverlay.enabled = true;
        playerInput.SwitchCurrentActionMap("Player");
        inMenu = false;
        OnReturnToGameplay?.Invoke();
    }

    public void OpenMenu(string menuId, MenuInfo menuInfo) {
        gameplayOverlay.enabled = false;
        Menu menu = GetMenuByID(menuId);
        playerInput.SwitchCurrentActionMap(menu.actionMapID);
        CloseAllMenus();
        menu.Open(menuInfo);
    }
    public void OpenMenu(string menuId) {
        gameplayOverlay.enabled = false;
        Menu menu = GetMenuByID(menuId);
        playerInput.SwitchCurrentActionMap(menu.actionMapID);
        CloseAllMenus();
        menu.Open();
    }

    private void CloseAllMenus() {
        foreach (Menu m in menus) m.Close();
    }
    private Menu GetMenuByID(string id) {
        return menus.Where(m => m.menuID == id).First();
    }
}