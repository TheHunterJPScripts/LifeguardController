/*
 * author: Mikel Jauregui
 * last update: 12/07/2019
 * description: Se encarga de generar la matriz
 * que contiene el "fog"
 */

using UnityEngine;

public class Generador : MonoBehaviour {

    public Vector2Int grid;
    const int seed = 1564;

    [Header("Noise Settings:")]
    [Range(1, 10)]
    public int octaves = 8;
    [Range(0, 1)]
    [Tooltip("Smoothness of the land.")]
    public float persistance = 0.8f;
    [Range(0, 2)]
    [Tooltip("Land details.")]
    public float lacunarity = 1.3f;
    public float noiseScale = 20.0f;
    public float noiseMultiplier = 10.0f;
    public Vector2 offset = new Vector2(1000.0f, 1000.0f);

    [Space(5)]

    public bool X;
    public bool minusX;
    public bool Z;
    public bool minusZ;
    [Space(5)]
    public bool XZ;
    public bool minusXZ;
    public bool minusXminusZ;
    public bool minusZX;

    public AnimationCurve sideCurve;
    public float minDistance;

    public float multiplier;
    public Material material;
    public GameObject prefab;

    private void Start() {
        Instantiate();
    }

    void Instantiate() {
        // Sanity Checks.
        if (grid.x <= 0 && grid.y <= 0)
            return;
        if (octaves < 0)
            octaves = 0;
        if (lacunarity < 1)
            lacunarity = 1;

        // Generamos una matrix que contiene la deformacion del terreno.
        float[,] noiseMap = Noise.GenerateNoiseMap(grid.x, grid.y, seed, noiseScale, octaves, persistance, lacunarity, offset, new Vector2(transform.parent.position.x / 2, transform.parent.position.z / 2));

        // Hacemos una iteracion por cada elemento de la matriz.
        for (int x = 0; x < grid.x; x++) {
            for (int y = 0; y < grid.y; y++) {
                // Creamos el objeto en cuestion.
                GameObject obj = Instantiate(prefab);
                obj.transform.parent = this.transform;
                Vector3 position = transform.position;
                Vector3 scale = obj.transform.localScale;

                position.x += x * obj.transform.lossyScale.x + 1 - grid.x / 2 * obj.transform.lossyScale.x;
                position.z += y * obj.transform.lossyScale.z + 1 - grid.y / 2 * obj.transform.lossyScale.z;

                // Calculamos el ruido en la posicion.
                float height = Mathf.Clamp(noiseMap[x, y] * noiseMultiplier, 2.5f, 7.0f);
                float lerp = 1;
                if (X) {
                    float sideLerp = Mathf.InverseLerp(0, ( grid.x * scale.x ) / 2 - minDistance, position.x - transform.position.x);
                    float slope = sideCurve.Evaluate(1 - sideLerp);
                    lerp *= slope;
                }
                if (minusX) {
                    float sideLerp = Mathf.InverseLerp(0, -( grid.x * scale.x ) / 2 + minDistance, position.x - transform.position.x);
                    float slope = sideCurve.Evaluate(1 - sideLerp);
                    lerp *= slope;
                }
                if (Z) {
                    float sideLerp = Mathf.InverseLerp(0, ( grid.y * scale.z ) / 2 - minDistance, position.z - transform.position.z);
                    float slope = sideCurve.Evaluate(1 - sideLerp);
                    lerp *= slope;
                }
                if (minusZ) {
                    float sideLerp = Mathf.InverseLerp(0, -( grid.y * scale.z ) / 2 + minDistance, position.z - transform.position.z);
                    float slope = sideCurve.Evaluate(1 - sideLerp);
                    lerp *= slope;
                }

                // Segun si tiene orilla en algun costado le aplicamos
                // la variacion correspondiente.
                if (XZ) {
                    float sideLerpX = Mathf.InverseLerp(0, ( grid.x * scale.x ) / 2 - minDistance, position.x - transform.position.x);
                    float slopeX = sideCurve.Evaluate(1 - sideLerpX);

                    float sideLerpZ = Mathf.InverseLerp(0, ( grid.y * scale.z ) / 2 - minDistance, position.z - transform.position.z);
                    float slopeZ = sideCurve.Evaluate(1 - sideLerpZ);

                    lerp *= Mathf.Max(slopeX, slopeZ);
                }
                if (minusZX) {
                    float sideLerpMinusX = Mathf.InverseLerp(0, ( grid.x * scale.x ) / 2 - minDistance, position.x - transform.position.x);
                    float slopeMinusX = sideCurve.Evaluate(1 - sideLerpMinusX);

                    float sideLerpMinusZ = Mathf.InverseLerp(0, -( grid.y * scale.z ) / 2 + minDistance, position.z - transform.position.z);
                    float slopeMinusZ = sideCurve.Evaluate(1 - sideLerpMinusZ);

                    lerp *= Mathf.Max(slopeMinusX, slopeMinusZ);
                }
                if (minusXminusZ) {
                    float sideLerpX = Mathf.InverseLerp(0, -( grid.x * scale.x ) / 2 + minDistance, position.x - transform.position.x);
                    float slopeX = sideCurve.Evaluate(1 - sideLerpX);

                    float sideLerpMinusZ = Mathf.InverseLerp(0, -( grid.y * scale.z ) / 2 + minDistance, position.z - transform.position.z);
                    float slopeMinusZ = sideCurve.Evaluate(1 - sideLerpMinusZ);

                    lerp *= Mathf.Max(slopeX, slopeMinusZ);
                }
                if (minusXZ) {
                    float sideLerpMinusX = Mathf.InverseLerp(0, -( grid.x * scale.x ) / 2 + minDistance, position.x - transform.position.x);
                    float slopeMinusX = sideCurve.Evaluate(1 - sideLerpMinusX);

                    float sideLerpZ = Mathf.InverseLerp(0, ( grid.y * scale.z ) / 2 - minDistance, position.z - transform.position.z);
                    float slopeZ = sideCurve.Evaluate(1 - sideLerpZ);

                    lerp *= Mathf.Max(slopeMinusX, slopeZ);
                }

                // Aplicamos la escala.
                scale.x = scale.x * lerp;
                scale.y = Mathf.Clamp(height, 2.5f, 7.0f) * lerp;
                scale.z = scale.z * lerp;

                obj.transform.position = position;
                obj.transform.localScale = scale;
                MeshFilter mf = obj.GetComponent<MeshFilter>();

                // Si el objeto es demasiado pequeño
                // lo eliminamos por razones esteticas.
                if (scale.x * lerp <= 0.3f)
                    GameObject.Destroy(obj);
            }
        }
    }
}
