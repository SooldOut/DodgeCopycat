using UnityEngine;

public class DiamondSpawner : MonoBehaviour
{
    public GameObject diamondPrefab;
    public LayerMask obstacleLayer; // 장애물 레이어
    public float spawnRadius = 350f; // 스포너 기준 최대 생성 반경
    public float checkRadius = 20f; // 생성 위치가 안전한지 확인할 반경

    void Start()
    {
        Vector3 randomPos = GetRandomSafePosition();
        if (randomPos != Vector3.zero)
        {
            Instantiate(diamondPrefab, randomPos, Quaternion.identity);
        }
    }

    private Vector3 GetRandomSafePosition()
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPoint.y = 18f; // 다이아몬드가 바닥에 뜨지 않게 Y값 고정

            // Physics.OverlapSphere로 생성 위치에 장애물이 있는지 확인합니다.
            Collider[] colliders = Physics.OverlapSphere(randomPoint, checkRadius, obstacleLayer);
            if (colliders.Length == 0)
            {
                return randomPoint;
            }
        }
        return Vector3.zero;
    }
}