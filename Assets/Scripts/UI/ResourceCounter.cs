using TMPro;
using UnityEngine;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField]
    private Resource.Color counterColor;

    [SerializeField]
    private TextMeshProUGUI counterText;

    [SerializeField]
    private PlayerReference playerRef;

    private CargoHold playerCargo;

    // Update is called once per frame
    void Update() {
        if (!playerRef.player) return;
        if (!playerCargo) playerCargo = playerRef.player.GetComponent<CargoHold>();
        counterText.text = playerCargo.GetCargoContent(counterColor).ToString();
    }
}