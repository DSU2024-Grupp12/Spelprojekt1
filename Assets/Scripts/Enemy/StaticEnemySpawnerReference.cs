using UnityEngine;

public class StaticEnemySpawnerReference : MonoBehaviour
{
    public void LockEnemySpawning() {
        EnemySpawner.LockEnemySpawning();
    }

    public void UnlockEnemySpawning() {
        EnemySpawner.UnlockEnemySpawning();
    }
}