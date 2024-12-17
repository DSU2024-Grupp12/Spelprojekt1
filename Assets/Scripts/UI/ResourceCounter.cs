using TMPro;
using UnityEngine;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField]
    private Resource.Color counterColor;

    [SerializeField]
    private TextMeshProUGUI counterText;

    [SerializeField]
    private CargoHold playerCargo;

    // Update is called once per frame
    void Update() {
        counterText.text = playerCargo.GetCargoContent(counterColor).ToString();
    }
}