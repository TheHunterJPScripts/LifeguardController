using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    public Vector3 size;
    public GameObject prefab;
    public Vector2Int amountPerSpawn;
    public float spawnIntervals;
    public int spawnTotalAmount;
    int amount;
    bool continueSpawning = true;
    float time;
    Level level;
    private void Start() {
        level = GameObject.Find("Level").GetComponent<Level>();
        time = spawnIntervals;
    }
    // Start is called before the first frame update
    void Update()
    {
        continueSpawning = level.enemies.Count < level.objects[level.fase].enemyMaxAmount;
        if (continueSpawning) {
            if(time >= spawnIntervals) {
                int count = Random.Range(amountPerSpawn.x, amountPerSpawn.y);
                for (int i = 0; i < count; i++) {
                    GeneratePrefab();
                }
                time = 0;
                amount += count;
            }

            time += Time.deltaTime;
        }
    }

    void GeneratePrefab() {
        float x = Random.Range(0 ,size.x) - size.x/2;
        float y = 0.5f;
        float z = Random.Range(0 ,size.z) - size.z/2;
        GameObject obj = Instantiate(prefab);
        obj.name = "Person";
        Vector3 position = transform.position;
        position.x += x;
        position.y = y;
        position.z += z;
        obj.transform.position = position;
        level.enemies.Add(obj);
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
