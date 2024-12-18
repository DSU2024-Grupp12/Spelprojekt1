using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField]
    private bool ignoreXPosition, ignoreYPosition, ignoreZPosition;

    [SerializeField]
    private GameObject leader;

    [SerializeField]
    private PlayerReference playerRef;

    private void Start() {
        if (!leader && playerRef.player) {
            leader = playerRef.player;
        }
    }

    // Update is called once per frame
    void Update() {
        if (leader) {
            Vector3 followPosition = new Vector3(
                ignoreXPosition ? transform.position.x : leader.transform.position.x,
                ignoreYPosition ? transform.position.y : leader.transform.position.y,
                ignoreZPosition ? transform.position.z : leader.transform.position.z
            );
            transform.position = followPosition;
        }
    }
}