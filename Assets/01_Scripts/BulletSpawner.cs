using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 3f;

    // �����ʰ� ������ ���ٴϴ� �ӵ�
    public float followSpeed = 2f;
    // �������� ���� ����
    public float hoverHeight = 5f;

    private Transform target;
    private float spawnRate;
    private float timeAfterSpawn;

    // Start ��� Awake�� ����� ������Ʈ ������ ���� Ȯ��
    void Awake()
    {
        target = FindFirstObjectByType<PlayerController>()?.transform;
        if (target == null)
        {
            Debug.LogError("PlayerController�� ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        if (target == null) return;

        // 1. �����ʰ� �÷��̾��� Y���� ���� ȸ���ϵ��� ��
        Vector3 directionToTarget = target.position - transform.position;
        // Vector3.up ���� �������� �÷��̾ �ٶ󺸰� ����ϴ�.
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // 2. �����ʰ� �÷��̾��� �̵��� ���� ������ ���󰡵��� ��
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + hoverHeight, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // 3. ������ �߻� ������ �״�� ����
        timeAfterSpawn += Time.deltaTime;
        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;
            GameObject fireball = Instantiate(fireballPrefab, transform.position, transform.rotation);

            // ���� LookAt()�� �ʿ� �����Ƿ� ����
            // fireball.transform.LookAt(target); 

            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }
}