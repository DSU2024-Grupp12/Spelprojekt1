using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField]
    private GameObject leader;
 
    [SerializeField]
    private ComponentMask ignorePosition, ignoreRotation;

    // Update is called once per frame
    void Update() {
        transform.position = GetMaskedVector(
            leader.transform.position,
            transform.position,
            ignorePosition
        );
        transform.eulerAngles = GetMaskedVector(
            leader.transform.eulerAngles,
            transform.eulerAngles,
            ignoreRotation
        );
    }

    private Vector3 GetMaskedVector(Vector3 leaderVector, Vector3 followerVector, ComponentMask mask) {
        return new Vector3(
            mask.x ? followerVector.x : leaderVector.x,
            mask.y ? followerVector.y : leaderVector.y,
            mask.z ? followerVector.z : leaderVector.z
        );
    }
}

[System.Serializable]
public struct ComponentMask
{
    public bool x, y, z;
}