using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public LayerMask obstacleLayer; // ��ֹ� ���̾�
    public float spawnRadius = 350f; // ������ ���� �ִ� ���� �ݰ�
    public float checkRadius = 20f; // ���� ��ġ�� �������� Ȯ���� �ݰ�

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
            randomPoint.y = 16f; // ���Ͱ� �ٴڿ� ���� �ʰ� Y�� ����

            // Physics.OverlapSphere�� ���� ��ġ�� ��ֹ��� �ִ��� Ȯ��
            Collider[] colliders = Physics.OverlapSphere(randomPoint, checkRadius, obstacleLayer);
            if (colliders.Length == 0)
            {
                Debug.Log("������ ���� ��ġ�� ã�ҽ��ϴ�: " + randomPoint);
                return randomPoint;
            }
        }
        Debug.LogError("������ ���� ��ġ�� 30�� �õ������� ã�� ���߽��ϴ�.");
        return Vector3.zero;
    }
}