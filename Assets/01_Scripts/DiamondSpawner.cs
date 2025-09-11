using UnityEngine;

public class DiamondSpawner : MonoBehaviour
{
    public GameObject diamondPrefab;
    public LayerMask obstacleLayer; // ��ֹ� ���̾�
    public float spawnRadius = 350f; // ������ ���� �ִ� ���� �ݰ�
    public float checkRadius = 20f; // ���� ��ġ�� �������� Ȯ���� �ݰ�

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
            randomPoint.y = 18f; // ���̾Ƹ�尡 �ٴڿ� ���� �ʰ� Y�� ����

            // Physics.OverlapSphere�� ���� ��ġ�� ��ֹ��� �ִ��� Ȯ���մϴ�.
            Collider[] colliders = Physics.OverlapSphere(randomPoint, checkRadius, obstacleLayer);
            if (colliders.Length == 0)
            {
                return randomPoint;
            }
        }
        return Vector3.zero;
    }
}