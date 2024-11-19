using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class Background : MonoBehaviour
{
    public Camera mainCamera;

    [SerializeField]
    private GameObject starPrefab;

    [SerializeField, Range(0, 1)]
    private float density;

    [SerializeField]
    private Vector2 resolution, sampleOrigin;

    // private List<GameObject> stars;

    // Start is called before the first frame update
    void Start() {
        // stars = new()

        GenerateStars();
    }

    public void GenerateStars() {
        Debug.ClearDeveloperConsole();
        for (int i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        Vector2 startingPosition =
            new Vector2(-mainCamera!.aspect * mainCamera.orthographicSize, mainCamera.orthographicSize);

        Random random = new Random((uint)UnityEngine.Random.Range(1, 100000));

        float height = mainCamera.orthographicSize * 2;
        float width = height * mainCamera.aspect;

        for (int i = 0; i < resolution.y; i++) {
            for (int j = 0; j < resolution.x; j++) {
                Debug.Log(new Vector2(sampleOrigin.x + j, sampleOrigin.y + i));
                Debug.Log(Mathf.PerlinNoise(sampleOrigin.x + j, sampleOrigin.y + i));
                if (Mathf.PerlinNoise(sampleOrigin.x + j, sampleOrigin.y + i) > density) continue;

                GameObject star = GameObject.Instantiate(starPrefab, transform, true);
                star.transform.localPosition = new Vector3(
                    startingPosition.x + j * width / resolution.x,
                    startingPosition.y - i * height / resolution.y,
                    900
                );
                star.transform.Rotate(Vector3.forward, random.NextFloat() * 360);
                Debug.Log("star");
            }
        }
    }
}