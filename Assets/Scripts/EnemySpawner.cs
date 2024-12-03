using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private EnemyPilot[] enemies;

    [SerializeField]
    private Transform defaultTarget;

    [SerializeField]
    private float spawnRate;
    private float nextSpawnTime;

    [SerializeField]
    private int maximumEnemies;

    [SerializeField]
    private Rect bounds;

    private bool boundsTooSmall;

    // Start is called before the first frame update
    void Start() {
        mainCamera = Camera.main;
        nextSpawnTime = Time.time + spawnRate;
    }

    // Update is called once per frame
    void Update() {
        float cameraArea = (mainCamera.orthographicSize * 2) * (mainCamera.orthographicSize * 2 * mainCamera.aspect);
        float spawnBoundsArea = bounds.height * bounds.width;
        if (spawnBoundsArea / cameraArea < 1f) {
            Debug.LogError("Spawn bounds smaller than camera, no enemies will spawn");
        }
        else if (spawnBoundsArea / cameraArea < 2f) {
            Debug.LogWarning("Enemy spawn bounds too small");
        }

        if (Time.time >= nextSpawnTime) {
            nextSpawnTime = Time.time + spawnRate;
            SpawnEnemy();
        }
    }

    public void SpawnEnemy() {
        if (boundsTooSmall) return;
        if (FindObjectsOfType<EnemyPilot>().Length >= maximumEnemies) return;

        bool validPosition = false;
        Vector2 position = new Vector2();
        for (int i = 0; !validPosition; i++) {
            if (i > 10) break;

            position = new Vector2(
                bounds.x + Random.Range(0f, bounds.width),
                bounds.y - Random.Range(0f, bounds.height)
            );
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(position);
            validPosition = (screenPoint.x < 0 || screenPoint.x > mainCamera.pixelWidth)
                            &&
                            (screenPoint.y < 0 || screenPoint.y > mainCamera.pixelHeight);
        }
        int randomIndex = Random.Range(0, enemies.Length);
        if (validPosition) {
            EnemyPilot enemyPilot = Instantiate(enemies[randomIndex], position, Quaternion.identity);
            enemyPilot.target = defaultTarget;
        }
    }
}