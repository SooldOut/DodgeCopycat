using UnityEngine;
using System.Collections;

public class BulletSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float spawnInterval = 2f;
    public float bulletSpeed = 10f;
    public float rotationSpeed = 5f; // Inspector���� �����Ͽ� ������ �ӵ��� ã������.

    // === �ʱ� ��ġ ���� ���� (Inspector���� �������� �������ּ���!) ===
    public Vector3 forcedInitialPosition;
    public Quaternion forcedInitialRotation;

    // === ���� ��ġ: �ּ� Y ���� ===
    public float minimumYPosition = 0f; // <-- ���Ϸ� ���� �ʵ��� �ּ� Y�� ���� (�ʿ信 ���� ����)

    private GameObject playerObject;
    private Coroutine spawnCoroutine;

    // ���� GameManager�� ���� ���¸� �����ϹǷ�, GameManager�� ���� ���� ���� �����ʰ� ȸ��/�߻��ϵ���
    private GameManager gameManager;

    void Awake()
    {
        // 0. Awake ���� ������ ��ġ �α� (������ ��ġ�� ��ġ)
        Debug.Log($"[{gameObject.name}] Awake Start: Initial Transform.position = {transform.position}");

        // 1. Inspector���� forcedInitialPosition�� �����Ǿ� �ִٸ� ���� (�̰��� �ֿ켱)
        Vector3 finalInitialPosition = forcedInitialPosition;
        if (finalInitialPosition.y < minimumYPosition)
        {
            finalInitialPosition.y = minimumYPosition;
            Debug.LogWarning($"[{gameObject.name}] Awake: forcedInitialPosition.y�� minimumYPosition({minimumYPosition})���� ���Ƽ� {finalInitialPosition.y}�� �����Ǿ����ϴ�.");
        }

        if (forcedInitialPosition != Vector3.zero || forcedInitialRotation != Quaternion.identity)
        {
            transform.position = finalInitialPosition;
            transform.rotation = forcedInitialRotation;
            Debug.Log($"[{gameObject.name}] Awake: Inspector�� ForcedInitialPosition���� ���� ����. New position = {transform.position}");
        }
        else if (transform.position == Vector3.zero && transform.rotation == Quaternion.identity)
        {
            Debug.LogWarning($"[{gameObject.name}] Awake: transform.position�� forcedInitialPosition ��� (0,0,0)�Դϴ�. Inspector ������ Ȯ���ϼ���! ���� {gameObject.name}�� Transform�� (0,0,0)�� ���ɼ��� �����ϴ�.");
        }

        // 3. Awake ���� �Ϸ� �� ���� ��ġ
        Debug.Log($"[{gameObject.name}] Awake End: Final Transform.position = {transform.position}");

        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError($"[{gameObject.name}]: ������ 'Player' �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�.");
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError($"[{gameObject.name}]: GameManager�� ã�� �� �����ϴ�. ���� ���� Ȯ�� �Ұ�.");
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
            Debug.LogWarning($"[{gameObject.name}] Start: Transform.position.y�� minimumYPosition({minimumYPosition})���� ���Ƽ� {transform.position.y}�� �����Ǿ����ϴ�.");
        }

        Debug.Log($"[{gameObject.name}] Start End: Final Transform.position = {transform.position}");
    }

    void OnEnable()
    {
        // OnEnable ������ �ڷ�ƾ ���� (�Ѿ� �߻�)
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnBulletRoutine());
    }

    void OnDisable()
    {
        // OnDisable ������ �ڷ�ƾ ����
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
    }

    void Update() // <-- ȸ�� ������ Update�� �̵�!
    {
        // ������ ���۵Ǿ��� �Ͻ����� ���°� �ƴ� ���� ȸ��
        if (gameManager != null && gameManager.IsGameStarted() && !gameManager.isPaused && playerObject != null)
        {
            Vector3 directionToPlayer = (playerObject.transform.position - transform.position).normalized;
            // �����ʰ� �÷��̾�� ���� ��ġ�� �־� directionToPlayer�� (0,0,0)�� �Ǵ� ��� ����
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
            // ������ ���۵Ǿ��� �Ͻ����� ���°� �ƴ� ���� �Ѿ� �߻�
            if (gameManager != null && gameManager.IsGameStarted() && !gameManager.isPaused && playerObject != null)
            {
                // �Ѿ� ���� �� �߻� (ȸ���� Update���� ó���ϹǷ� ���⼭ �� �ʿ� ����)
                GameObject bullet = Instantiate(fireballPrefab, transform.position, transform.rotation);

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.speed = bulletSpeed;
                }
                // Debug.Log($"[{gameObject.name}]: �Ѿ� �߻�� at {transform.position}");
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
            Debug.LogWarning($"[{gameObject.name}] ResetPosition: forcedInitialPosition.y�� minimumYPosition({minimumYPosition})���� ���Ƽ� {resetToPosition.y}�� �����Ǿ����ϴ�.");
        }

        if (forcedInitialPosition == Vector3.zero && forcedInitialRotation == Quaternion.identity)
        {
            Debug.LogWarning($"[{gameObject.name}]: ResetPosition ȣ���. ������ forcedInitialPosition�� (0,0,0)�Դϴ�. Inspector ������ Ȯ���ϼ���!");
        }

        transform.position = resetToPosition;
        transform.rotation = forcedInitialRotation;
        Debug.Log($"[{gameObject.name}] ResetPosition End: Initial position restored to: {transform.position}");
    }
}