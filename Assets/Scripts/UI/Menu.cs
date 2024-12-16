using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class Menu : MonoBehaviour
{
    public string menuID;
    public string actionMapID;

    [SerializeField]
    private MenuField[] textFields;

    private Canvas canvas;

    private bool closeOnAwake;

    private void Awake() {
        canvas = GetComponent<Canvas>();
        if (closeOnAwake) Close();
    }

    public void Open(MenuInfo menuInfo) {
        canvas.enabled = true;
        LoadMenuInfo(menuInfo);
    }

    public void Open() {
        Open(new MenuInfo());
    }

    public void LoadMenuInfo(MenuInfo menuInfo) {
        if (menuInfo.Count == 0) return;

        foreach (MenuField field in textFields) {
            if (menuInfo.Contains(field.key)) {
                (string s, UnityAction a) = menuInfo[field.key];
                if (field.field) field.field.text = s;
                if (a != null && field.button) field.button.onClick.AddListener(a);
            }
        }
    }

    public void Close() {
        if (canvas) canvas.enabled = false;
        else closeOnAwake = true;
    }

    public void ReturnToGameplay() {
        MenuManager.Instance.ReturnToGameplay();
    }
}

[System.Serializable]
public class MenuField
{
    public string key;
    public TMP_Text field;
    public Button button;
}