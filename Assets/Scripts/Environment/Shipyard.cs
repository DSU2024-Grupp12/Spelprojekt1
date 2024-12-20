using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Shipyard : MonoBehaviour, IInteractable
{
    private const string UpgradeItem1Name = "UpgradeItem1";
    private const string UpgradeItem2Name = "UpgradeItem2";
    private const string UpgradeItem3Name = "UpgradeItem3";
    private const string UpgradeItem1Green = "UpgradeItem1GreenCost";
    private const string UpgradeItem2Green = "UpgradeItem2GreenCost";
    private const string UpgradeItem3Green = "UpgradeItem3GreenCost";
    private const string UpgradeItem1Purple = "UpgradeItem1PurpleCost";
    private const string UpgradeItem2Purple = "UpgradeItem2PurpleCost";
    private const string UpgradeItem3Purple = "UpgradeItem3PurpleCost";
    private const string RepairShipCost = "RepairShipCost";

    [SerializeField]
    private string shipyardMenuID;

    [SerializeField]
    private Canvas highlightPrompt;

    [SerializeField]
    private int repairCost, repairAmount;

    [SerializeField]
    private UpgradeModule[] availableUpgradeModules;

    public UnityEvent ShipyardUsed;

    private Transform player;

    private bool shipyardUpgradeUsed;
    private bool shipyardLockedSpawner;

    UpgradeModule module1;
    UpgradeModule module2;
    UpgradeModule module3;

    public void Start() {
        Unhighlight();
        shipyardUpgradeUsed = false;
    }

    public void Interact() {
        if (shipyardUpgradeUsed) {
            Popup.Display("Shipyard used", 1f);
            return;
        }
        if (EnemySpawner.InWave) {
            Popup.Display("Too many enemies nearby", 1f);
            return;
        }
        Unhighlight();
        MenuManager.Instance.OpenMenu(shipyardMenuID, BuildMenuInfo());
        if (!EnemySpawner.locked) {
            EnemySpawner.LockEnemySpawning();
            shipyardLockedSpawner = true;
        }
        if (player) {
            IInteractable.LockPlayer(player.GetComponent<Rigidbody2D>());
        }

        MenuManager.OnReturnToGameplay += ShipyardMenuClosed;
    }

    public void Highlight() {
        highlightPrompt.enabled = true;
        if (shipyardUpgradeUsed) highlightPrompt.enabled = false;
    }
    public void Unhighlight() {
        highlightPrompt.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) {
            InteractableManager.QueueInteractable(this);
            if (other.gameObject.GetComponent<Rigidbody2D>()) player = other.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerShip")) {
            InteractableManager.RemoveInteractableFromQueue(this);
            player = null;
        }
    }

    private MenuInfo BuildMenuInfo() {
        switch (availableUpgradeModules.Length) {
            case 0: return new MenuInfo();
            case > 0:
                MenuInfo info = new MenuInfo();
                if (!module1) module1 = GetRandomUpgradeModule();
                if (!module2) module2 = GetRandomUpgradeModule();
                if (!module3) module3 = GetRandomUpgradeModule();
                info.AddEntry(UpgradeItem1Name, module1.moduleDisplayName, AddModuleToPlayerDelegate(module1));
                info.AddEntry(UpgradeItem2Name, module2.moduleDisplayName, AddModuleToPlayerDelegate(module2));
                info.AddEntry(UpgradeItem3Name, module3.moduleDisplayName, AddModuleToPlayerDelegate(module3));
                info.AddEntry(UpgradeItem1Green, module1.greenResourceCost.ToString());
                info.AddEntry(UpgradeItem2Green, module2.greenResourceCost.ToString());
                info.AddEntry(UpgradeItem3Green, module3.greenResourceCost.ToString());
                info.AddEntry(UpgradeItem1Purple, module1.purpleResourceCost.ToString());
                info.AddEntry(UpgradeItem2Purple, module2.purpleResourceCost.ToString());
                info.AddEntry(UpgradeItem3Purple, module3.purpleResourceCost.ToString());
                info.AddEntry(RepairShipCost, repairCost.ToString(), RepairShipDelegate());

                return info;
            default: return new MenuInfo();
        }
    }

    private UpgradeModule GetRandomUpgradeModule() {
        return availableUpgradeModules[Random.Range(0, availableUpgradeModules.Length)];
    }

    private UnityAction AddModuleToPlayerDelegate(UpgradeModule module) {
        Debug.Assert(player); // Player should not be null if this method is called. If it is, something is wrong
        UpgradeMatrix[] matrices = player.GetComponentsInChildren<UpgradeMatrix>();
        UpgradeMatrix match = matrices.Where(m => m.matrixID == module.matrixID).First();

        return () => {
            CargoHold playerHold = player.GetComponent<CargoHold>();
            if (playerHold.Pay(module.greenResourceCost, module.purpleResourceCost)) {
                match.AttachModule(module);
                shipyardUpgradeUsed = true;
                ShipyardUsed?.Invoke();
                MenuManager.Instance.ReturnToGameplay();
            }
            else {
                Popup.Display("Not enough resources to buy that upgrade", 1.5f);
            }
        };
    }

    private UnityAction RepairShipDelegate() {
        return () => {
            Hull playerHull = player.GetComponent<Hull>();
            if (!playerHull.AtFullStrength()) {
                if (playerHull.GetComponent<CargoHold>().Pay(repairCost, 0)) {
                    shipyardUpgradeUsed = true;
                    ShipyardUsed?.Invoke();
                    MenuManager.Instance.ReturnToGameplay();
                }
                Popup.Display("Not enough resources to repair ship", 1f);
            }
            else {
                Popup.Display("Ship does not need repair.", 1f);
            }
        };
    }

    private void ShipyardMenuClosed() {
        Highlight();
        MenuManager.OnReturnToGameplay -= ShipyardMenuClosed;
        if (shipyardLockedSpawner) EnemySpawner.UnlockEnemySpawning();
        shipyardLockedSpawner = false;
    }
}