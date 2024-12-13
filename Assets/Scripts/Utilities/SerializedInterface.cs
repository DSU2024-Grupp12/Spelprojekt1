using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializedInterface<T> : ISerializationCallbackReceiver where T : IInterfaceIdentity
{
    public GameObject _object;
    public string id;

    public T extract {
        get {
            if (id != "") {
                return extractMany.Where(i => i.GetID() == id).First();
            }
            else return _object.GetComponent<T>();
        }
    }
    public T[] extractMany => _object.GetComponents<T>();

    public T this[string s] {
        get { return extractMany.Where(i => i.GetID() == s).First(); }
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