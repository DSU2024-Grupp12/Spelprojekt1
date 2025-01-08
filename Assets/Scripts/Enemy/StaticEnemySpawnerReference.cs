using UnityEngine;

public class StaticEnemySpawnerReference : MonoBehaviour
{
    public void LockEnemySpawning(string lockName) {
        EnemySpawner.LockEnemySpawning(lockName);
    }

    public void UnlockEnemySpawning(string lockName) {
        EnemySpawner.UnlockEnemySpawning(lockName);
    }
}