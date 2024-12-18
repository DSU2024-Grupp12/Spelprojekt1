using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReference : MonoBehaviour
{
    private static GameObject staticPlayer;

    public GameObject player => staticPlayer;

    public static void SetPlayer(GameObject p) {
        staticPlayer = p;
    }
}