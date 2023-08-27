using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] private int spawnNum = 10;
    [SerializeField] private float spawnInterval = 10f;
    private GameObject enemyPrefab;
    private Transform headTransform;
    private float distance = 30f;
    void Start()
    {
        enemyPrefab = Resources.Load("Prefabs/enemy") as GameObject;
        headTransform = GameObject.Find("head").transform;
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            for (int i = 0; i < spawnNum; i++)
            {
                float rad = Random.Range(0, 2 * Mathf.PI);
                float x = distance * Mathf.Cos(rad);
                float z = distance * Mathf.Sin(rad);
                Vector3 dir = headTransform.position + new Vector3(x, 0, z);
                Instantiate(enemyPrefab, dir, Quaternion.identity);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
        
    }
}
