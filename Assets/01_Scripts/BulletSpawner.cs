using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 3f;

    // 스포너가 공중을 떠다니는 속도
    public float followSpeed = 2f;
    // 스포너의 공중 높이
    public float hoverHeight = 5f;

    private Transform target;
    private float spawnRate;
    private float timeAfterSpawn;

    // Start 대신 Awake를 사용해 컴포넌트 참조를 먼저 확보
    void Awake()
    {
        target = FindFirstObjectByType<PlayerController>()?.transform;
        if (target == null)
        {
            Debug.LogError("PlayerController를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (target == null) return;

        // 1. 스포너가 플레이어의 Y축을 따라 회전하도록 함
        Vector3 directionToTarget = target.position - transform.position;
        // Vector3.up 축을 기준으로 플레이어를 바라보게 만듭니다.
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        // 2. 스포너가 플레이어의 이동에 맞춰 느리게 따라가도록 함
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + hoverHeight, target.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // 3. 기존의 발사 로직은 그대로 유지
        timeAfterSpawn += Time.deltaTime;
        if (timeAfterSpawn >= spawnRate)
        {
            timeAfterSpawn = 0f;
            GameObject fireball = Instantiate(fireballPrefab, transform.position, transform.rotation);

            // 기존 LookAt()은 필요 없으므로 제거
            // fireball.transform.LookAt(target); 

            spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        }
    }
}