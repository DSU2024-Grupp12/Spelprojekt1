using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializedInterface<T> : ISerializationCallbackReceiver where T : IInterfaceIdentity
{
    [SerializeField]
    private GameObject _object;
    [SerializeField]
    private string id;

    public T extract {
        get {
            if (id != "" && _object) {
                return extractMany.Where(i => i.GetID() == id).First();
            }
            else return _object.GetComponent<T>();
        }
    }
    public T[] extractMany => _object.GetComponents<T>();
    public GameObject gameObject => _object;

    public T this[string s] {
        get { return extractMany.Where(i => i.GetID() == s).First(); }
    }

    public bool Defined() {
        return _object;
    }

    /// <inheritdoc />
    public void OnBeforeSerialize() {
        if (_object) {
            if (_object.GetComponent<T>() == null) _object = null;
        }
    }
    /// <inheritdoc />
    public void OnAfterDeserialize() { }
}