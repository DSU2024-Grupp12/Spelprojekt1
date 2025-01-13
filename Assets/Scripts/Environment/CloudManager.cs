using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CloudManager : MonoBehaviour
{
    public UnityEvent PlayerEnterCloud;
    public UnityEvent PlayerExitCloud;

    private Cloud[] clouds;

    private bool _playerInCloud;
    private bool playerInCloud {
        get => _playerInCloud;
        set {
            if (value != _playerInCloud) {
                if (value) PlayerEnterCloud?.Invoke();
                if (!value) PlayerExitCloud?.Invoke();
            }
            _playerInCloud = value;
        }
    }

    private void Start() {
        clouds = FindObjectsOfType<Cloud>();
    }

    private void Update() {
        bool isPlayerInCloud = false;
        foreach (Cloud cloud in clouds) {
            if (cloud.containsPlayer) isPlayerInCloud = true;
        }
        playerInCloud = isPlayerInCloud;
    }
}