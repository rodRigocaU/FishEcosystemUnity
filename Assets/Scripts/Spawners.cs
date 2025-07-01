using UnityEngine;
using System.Collections;

public class Spawners : MonoBehaviour
{
    public GameObject[] spawners;
    public GameObject foodPrefab;

    public float spawnInterval = 9f;       // Tiempo entre chequeos
    public float spawnChance = 0.09f;       // 30% de probabilidad
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            foreach (GameObject spawner in spawners)
            {
                if (Random.value < spawnChance)
                {
                    Instantiate(foodPrefab, spawner.transform.position, Quaternion.identity);
                }
            }
        }
    }
}
