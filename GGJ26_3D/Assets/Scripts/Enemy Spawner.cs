using UnityEngine;

using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public GameObject player;
    public int enemiesToSpawn = 1;
    public LayerMask collisionMask;
    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float spawnCheckRadius = 0.5f;
    public float maxRaycastHeight = 100f;

    private int waveCount = 0;

    void Start()
    {

        StartCoroutine(SpawnWaves());
    }

    private System.Collections.IEnumerator SpawnWaves()
    {
        while (true)
        {
            SpawnEnemies();
            waveCount++;
            if (waveCount % 5 == 0)
            {
                enemiesToSpawn++;
            }
            float waitTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPos = GetValidSpawnPosition();
            GameObject prefab = GetRandomEnemy();
            if (prefab != null)
            {
                float yOffset = 1f;
                Vector3 adjustedSpawnPos = spawnPos + Vector3.up * yOffset;
                GameObject enemy = Instantiate(prefab, adjustedSpawnPos, Quaternion.identity);
         
                if (prefab.name.ToLower().Contains("attacker"))
                {
                    yOffset = 1.5f;
                    var knight = enemy.GetComponent<KnightBehavior>();
                    knight.player = player;
                }
                else { 
                    var npc = enemy.GetComponent<NPCBehavior>();
                    npc.player = player;
                }

                // Assign the player reference to the spawned enemy's NPCBehavior, if present


            }
        }
    }

    private GameObject GetRandomEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return null;
        int idx = Random.Range(0, enemyPrefabs.Count);
        return enemyPrefabs[idx];
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 playerPos = player.transform.position;
        float defaultGroundY = 0f; // Set this to your ground's Y if not at 0
        // Random direction and distance
        Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);
        Vector3 candidate = playerPos + new Vector3(circle.x, 0, circle.y);

        // Raycast down from above to find top of object
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(candidate.x, maxRaycastHeight, candidate.z);
        if (Physics.SphereCast(rayOrigin, spawnCheckRadius, Vector3.down, out hit, maxRaycastHeight * 2, collisionMask))
        {
            // Place on top of hit object
            return hit.point + Vector3.up * 0.5f; // 0.5f offset to avoid clipping
        }
        else
        {
            // No object below, spawn at ground level (defaultGroundY)
            // Only return if the candidate position is valid (optional: add extra checks here)
            return new Vector3(candidate.x, defaultGroundY + 0.5f, candidate.z);
        }
    }
}
