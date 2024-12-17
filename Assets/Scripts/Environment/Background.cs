using UnityEngine;
using Random = Unity.Mathematics.Random;

public class Background : MonoBehaviour
{
    [SerializeField]
    private GameObject starPrefab;

    [SerializeField]
    private Sprite[] stars;

    [SerializeField]
    private uint seed, numberOfStars;

    [SerializeField]
    private float width, height;

    [SerializeField, Range(0f, 1f)]
    private float parallaxFactor;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start() {
        GenerateStars();
        mainCamera = Camera.main;
    }

    private void LateUpdate() {
        Vector2 parallaxPosition = mainCamera.transform.position * parallaxFactor;
        transform.position = parallaxPosition;
    }

    public void GenerateStars() {
        ClearStars();

        Vector3 startingPosition = new Vector3(-width / 2, -height / 2, 0);
        Random random;
        if (seed != 0) {
            random = new Random(seed);
        }
        else {
            random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
        }

        for (int i = 0; i < numberOfStars; i++) {
            GameObject star = GameObject.Instantiate(starPrefab, transform, true);
            star.GetComponent<SpriteRenderer>().sprite = stars[random.NextInt(stars.Length)];
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