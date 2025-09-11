using UnityEngine;
using System.Collections;

public class BulletSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float spawnInterval = 2f;
    public float bulletSpeed = 10f;
    public float rotationSpeed = 5f; // Inspector에서 조절하여 적절한 속도를 찾으세요.

    // === 초기 위치 강제 설정 (Inspector에서 수동으로 설정해주세요!) ===
    public Vector3 forcedInitialPosition;
    public Quaternion forcedInitialRotation;

    // === 안전 장치: 최소 Y 높이 ===
    public float minimumYPosition = 0f; // <-- 지하로 가지 않도록 최소 Y값 설정 (필요에 따라 조절)

    private GameObject playerObject;
    private Coroutine spawnCoroutine;

    // 현재 GameManager가 게임 상태를 제어하므로, GameManager가 게임 중일 때만 스포너가 회전/발사하도록
    private GameManager gameManager;

    void Awake()
    {
        // 0. Awake 시작 시점의 위치 로그 (씬에서 배치된 위치)
        Debug.Log($"[{gameObject.name}] Awake Start: Initial Transform.position = {transform.position}");

        // 1. Inspector에서 forcedInitialPosition이 설정되어 있다면 적용 (이것이 최우선)
        Vector3 finalInitialPosition = forcedInitialPosition;
        if (finalInitialPosition.y < minimumYPosition)
        {
            finalInitialPosition.y = minimumYPosition;
            Debug.LogWarning($"[{gameObject.name}] Awake: forcedInitialPosition.y가 minimumYPosition({minimumYPosition})보다 낮아서 {finalInitialPosition.y}로 보정되었습니다.");
        }

        if (forcedInitialPosition != Vector3.zero || forcedInitialRotation != Quaternion.identity)
        {
            transform.position = finalInitialPosition;
            transform.rotation = forcedInitialRotation;
            Debug.Log($"[{gameObject.name}] Awake: Inspector의 ForcedInitialPosition으로 강제 설정. New position = {transform.position}");
        }
        else if (transform.position == Vector3.zero && transform.rotation == Quaternion.identity)
        {
            Debug.LogWarning($"[{gameObject.name}] Awake: transform.position과 forcedInitialPosition 모두 (0,0,0)입니다. Inspector 설정을 확인하세요! 현재 {gameObject.name}의 Transform이 (0,0,0)일 가능성이 높습니다.");
        }

        // 3. Awake 로직 완료 후 최종 위치
        Debug.Log($"[{gameObject.name}] Awake End: Final Transform.position = {transform.position}");

        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError($"[{gameObject.name}]: 씬에서 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError($"[{gameObject.name}]: GameManager를 찾을 수 없습니다. 게임 상태 확인 불가.");
        }
    }

    void Start()
    {
        Debug.Log($"[{gameObject.name}] Start Start: Transform.position = {transform.position}");

        if (transform.position.y < minimumYPosition)
        {
            Vector3 currentPos = transform.position;
            currentPos.y = minimumYPosition;
            transform.position = currentPos;
            Debug.LogWarning($"[{gameObject.name}] Start: Transform.position.y가 minimumYPosition({minimumYPosition})보다 낮아서 {transform.position.y}로 보정되었습니다.");
        }

        Debug.Log($"[{gameObject.name}] Start End: Final Transform.position = {transform.position}");
    }

    void OnEnable()
    {
        // OnEnable 시점에 코루틴 시작 (총알 발사)
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnBulletRoutine());
    }

    void OnDisable()
    {
        // OnDisable 시점에 코루틴 정지
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
    }

    void Update() // <-- 회전 로직을 Update로 이동!
    {
        // 게임이 시작되었고 일시정지 상태가 아닐 때만 회전
        if (gameManager != null && gameManager.IsGameStarted() && !gameManager.isPaused && playerObject != null)
        {
            Vector3 directionToPlayer = (playerObject.transform.position - transform.position).normalized;
            // 스포너가 플레이어와 같은 위치에 있어 directionToPlayer가 (0,0,0)이 되는 경우 방지
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    IEnumerator SpawnBulletRoutine()
    {
        while (true)
        {
            // 게임이 시작되었고 일시정지 상태가 아닐 때만 총알 발사
            if (gameManager != null && gameManager.IsGameStarted() && !gameManager.isPaused && playerObject != null)
            {
                // 총알 생성 및 발사 (회전은 Update에서 처리하므로 여기서 할 필요 없음)
                GameObject bullet = Instantiate(fireballPrefab, transform.position, transform.rotation);

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.speed = bulletSpeed;
                }
                // Debug.Log($"[{gameObject.name}]: 총알 발사됨 at {transform.position}");
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void ResetPosition()
    {
        Debug.Log($"[{gameObject.name}] ResetPosition Start: Current Transform.position = {transform.position}");

        Vector3 resetToPosition = forcedInitialPosition;
        if (resetToPosition.y < minimumYPosition)
        {
            resetToPosition.y = minimumYPosition;
            Debug.LogWarning($"[{gameObject.name}] ResetPosition: forcedInitialPosition.y가 minimumYPosition({minimumYPosition})보다 낮아서 {resetToPosition.y}로 보정되었습니다.");
        }

        if (forcedInitialPosition == Vector3.zero && forcedInitialRotation == Quaternion.identity)
        {
            Debug.LogWarning($"[{gameObject.name}]: ResetPosition 호출됨. 하지만 forcedInitialPosition이 (0,0,0)입니다. Inspector 설정을 확인하세요!");
        }

        transform.position = resetToPosition;
        transform.rotation = forcedInitialRotation;
        Debug.Log($"[{gameObject.name}] ResetPosition End: Initial position restored to: {transform.position}");
    }
}