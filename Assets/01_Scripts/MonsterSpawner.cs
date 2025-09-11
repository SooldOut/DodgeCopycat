using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public LayerMask obstacleLayer; // 장애물 레이어
    public float spawnRadius = 350f; // 스포너 기준 최대 생성 반경
    public float checkRadius = 20f; // 생성 위치가 안전한지 확인할 반경

    public float spawnRate = 3f;
    private float timeAfterSpawn;

    void Update()
    {
        timeAfterSpawn += Time.deltaTime;
        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;
            Vector3 randomPos = GetRandomSpawnPosition();
            if (randomPos != Vector3.zero)
            {
                Instantiate(monsterPrefab, randomPos, Quaternion.identity);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPoint.y = 16f; // 몬스터가 바닥에 뜨지 않게 Y값 고정

            // Physics.OverlapSphere로 생성 위치에 장애물이 있는지 확인
            Collider[] colliders = Physics.OverlapSphere(randomPoint, checkRadius, obstacleLayer);
            if (colliders.Length == 0)
            {
                Debug.Log("안전한 생성 위치를 찾았습니다: " + randomPoint);
                return randomPoint;
            }
        }
        Debug.LogError("안전한 생성 위치를 30번 시도했지만 찾지 못했습니다.");
        return Vector3.zero;
    }
}