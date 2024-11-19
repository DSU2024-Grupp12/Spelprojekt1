using UnityEngine;
using Random = Unity.Mathematics.Random;

public class Background : MonoBehaviour
{
    public Camera mainCamera;

    [SerializeField]
    private GameObject starPrefab;

    [SerializeField]
    private uint seed, numberOfStars;

    [SerializeField]
    private float width, height;

    // private List<GameObject> stars;

    // Start is called before the first frame update
    void Start() {
        // stars = new()

        GenerateStars();
    }

    private void Update() {
        // implement paralax
    }

    public void GenerateStars() {
        ClearStars();

        Vector3 startingPosition = new Vector3(-width / 2, -height / 2, 0);
        Random random = new Random(seed);

        for (int i = 0; i < numberOfStars; i++) {
            GameObject star = GameObject.Instantiate(starPrefab, transform, true);
            float randomX = random.NextFloat() * width;
            float randomY = random.NextFloat() * height;
            star.transform.position = startingPosition + new Vector3(randomX, randomY, 900);
        }
    }

    public void ClearStars() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}