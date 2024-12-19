using UnityEngine;

[CreateAssetMenu(fileName = "NewLog", menuName = "Lore Log")]
public class Log : ScriptableObject
{
    public string wreckType;
    [TextArea(3, 15)]
    public string entryContent;
}