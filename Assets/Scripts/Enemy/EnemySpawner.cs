using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private Transform defaultTarget;

    [SerializeField]
    private EnemyWave[] waves;

    [SerializeField]
    private float timeBetweenWaves;

    private float startOfScene;

    private float timeUntilNextWave;
    public static bool InWave { get; private set; }
    private static bool locked;

    private float halfDiagonal = 15f;
    private Unity.Mathematics.Random random;

    private float remainingTimeWhenLocked;
    private static bool remainingTimeSet;

    public EnemySpawnEvents enemySpawnEvents;

    // Start is called before the first frame update
    void Start() {
        mainCamera = Camera.main;
        timeUntilNextWave = Time.time + timeBetweenWaves;
        startOfScene = Time.time;
        InWave = false;

        random = new((uint)Random.Range(0, int.MaxValue));

        if (mainCamera) {
            float camHalfHeight = mainCamera.orthographicSize;
            float camHalfWidth = camHalfHeight * mainCamera.aspect;
            halfDiagonal = Mathf.Sqrt(Mathf.Pow(camHalfHeight, 2) + Mathf.Pow(camHalfWidth, 2));
        }
    }

    // Update is called once per frame
    void Update() {
        if (locked) {
            if (!remainingTimeSet) {
                remainingTimeWhenLocked = timeUntilNextWave - Time.time;
                remainingTimeSet = true;
            }
            timeUntilNextWave = Time.time + remainingTimeWhenLocked;
            return;
        }
        if (!InWave && Time.time >= timeUntilNextWave) {
            StartCoroutine(ProcessWave());
        }
    }

    public IEnumerator ProcessWave() {
        InWave = true;

        EnemyWave[] validWaves = waves.Where(w => w.dontSpawnUntilTimeHasPassed < Time.time - startOfScene).ToArray();
        if (validWaves.Length > 0) {
            enemySpawnEvents.OnWaveStarted?.Invoke();

            EnemyWave randomWave = validWaves[Random.Range(0, validWaves.Length)];
            foreach (EnemyGroup group in randomWave.enemyGroups) {
                for (int i = 0; i < group.number; i++) {
                    bool maxEnemiesPresent = FindObjectsOfType<EnemyPilot>().Length >= randomWave.maxEnemiesAtOnce;
                    yield return new WaitUntil(() => !maxEnemiesPresent);
                    SpawnEnemy(group.enemyType);
                    yield return new WaitForSeconds(1f);
                }
            }

            while (FindObjectsOfType<EnemyPilot>().Length > 0) {
                yield return null;
            }
            timeUntilNextWave = Time.time + randomWave.recoveryTime + timeBetweenWaves;

            enemySpawnEvents.OnWaveFinished?.Invoke();
        }
        else {
            timeUntilNextWave = Time.time + timeBetweenWaves;
        }
        InWave = false;
    }

    public void SpawnEnemy(EnemyPilot enemyType) {
        Vector2 position = random.GetNextPolarCoordinate(
            halfDiagonal + 10,
            halfDiagonal + 30,
            mainCamera.transform.position
        );

        EnemyPilot enemyPilot = Instantiate(enemyType, position, Quaternion.identity);
        enemyPilot.target = defaultTarget;
    }

    public static void LockEnemySpawning() {
        locked = true;
    }

    public static void UnlockEnemySpawning() {
        locked = false;
        remainingTimeSet = false;
    }
}

[System.Serializable]
public class EnemyWave
{
    public float dontSpawnUntilTimeHasPassed;
    public int maxEnemiesAtOnce;
    [Tooltip(
        "The amount of time that will pass after this wave is over before the default wave timer starts counting down. " +
        "That is, the total time until the next wave is equal to recovery time + time between waves")]
    public float recoveryTime;
    public EnemyGroup[] enemyGroups;
}

[System.Serializable]
public class EnemyGroup
{
    public EnemyPilot enemyType;
    [Min(0)]
    public int number;
}

[System.Serializable]
public class EnemySpawnEvents
{
    public UnityEvent
        OnWaveStarted,
        OnWaveFinished;
}