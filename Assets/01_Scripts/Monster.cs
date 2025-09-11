using UnityEngine;

public class Monster : MonoBehaviour
{
    public float moveSpeed = 5f; // 몬스터의 이동 속도
    public float rotationSpeed = 5f; // 몬스터의 회전 속도

    private Rigidbody monsterRigidbody; // <-- Rigidbody 변수 선언
    private Transform playerTarget;
    private GameManager gameManager;

    void Start()
    {
        monsterRigidbody = GetComponent<Rigidbody>();
        if (monsterRigidbody == null)
        {
            Debug.LogError("Monster: Rigidbody 컴포넌트를 찾을 수 없습니다! 물리적 충돌이 발생하지 않을 수 있습니다.");
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("Monster: GameManager를 씬에서 찾을 수 없습니다.");
        }

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerTarget = playerController.transform;
        }
        else
        {
            Debug.LogError("Monster: PlayerController를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void FixedUpdate() // <-- 물리 계산은 FixedUpdate에서 처리하는 것이 좋습니다.
    {
        // 플레이어 타겟이 없거나, 게임이 일시정지 상태면 이동하지 않음
        if (playerTarget == null || Time.timeScale == 0)
        {
            // Rigidbody가 움직이지 않도록 속도를 0으로 설정
            if (monsterRigidbody != null)
            {
                monsterRigidbody.linearVelocity = Vector3.zero;
                monsterRigidbody.angularVelocity = Vector3.zero;
            }
            return;
        }

        // 플레이어를 향하는 방향 계산
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;

        // 몬스터의 앞 방향을 플레이어를 향하게 부드럽게 회전
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        // Rigidbody를 사용할 경우 transform.rotation 대신 Rigidbody.MoveRotation을 사용하는 것이 좋습니다.
        monsterRigidbody.MoveRotation(Quaternion.Slerp(monsterRigidbody.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed));

        // 계산된 방향으로 몬스터 이동 (Rigidbody.velocity 사용)
        // transform.position += ... 대신 Rigidbody.velocity를 사용합니다.
        monsterRigidbody.linearVelocity = directionToPlayer * moveSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Monster: 플레이어와 충돌! 게임 패배 처리.");
            if (gameManager != null)
            {
                gameManager.LoseGame();
            }
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Wall")) // <-- 벽과 충돌 시 로그 추가 (디버깅용)
        {
            Debug.Log("Monster: 벽과 충돌했습니다. 뚫지 않습니다.");
            // 몬스터가 벽을 뚫지 않도록 Rigidbody 설정과 Collider 설정이 올바르면
            // 특별히 여기서 추가 액션을 취할 필요는 없습니다. 물리 엔진이 처리할 것입니다.
        }
    }
}