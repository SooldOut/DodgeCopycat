using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    private Rigidbody bulletRigidbody;

    private GameManager gameManager;
    private Transform playerTarget;

    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("Bullet: GameManager를 찾을 수 없습니다! 게임 패배 로직이 작동하지 않을 수 있습니다.");
        }

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerTarget = playerController.transform;
        }
        else
        {
            Debug.LogError("Bullet: PlayerController를 가진 오브젝트를 찾을 수 없어 총알이 플레이어를 추적할 수 없습니다!");
        }


        if (bulletRigidbody != null)
        {
            if (playerTarget != null)
            {
                Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
                bulletRigidbody.linearVelocity = directionToPlayer * speed;
                Debug.Log(gameObject.name + ": 총알이 플레이어를 향해 발사됨! 속도: " + bulletRigidbody.linearVelocity);
            }
            else
            {
                bulletRigidbody.linearVelocity = transform.forward * speed;
                Debug.Log(gameObject.name + ": 플레이어를 찾을 수 없어 기본 방향으로 발사. 속도: " + bulletRigidbody.linearVelocity);
            }
        }
        else
        {
            Debug.LogError(gameObject.name + ": Rigidbody를 찾을 수 없어 총알 속도 설정 불가!");
        }

        Destroy(gameObject, 3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // === 플레이어와 충돌 ===
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Bullet: 플레이어와 충돌! 게임 패배 처리.");
            if (gameManager != null)
            {
                gameManager.LoseGame();
            }
            Destroy(gameObject);
        }
        // === 몬스터와 충돌 === (새로 추가된 부분)
        else if (collision.collider.CompareTag("Monster"))
        {
            Debug.Log("Bullet: 몬스터와 충돌! 총알 파괴.");
            // 몬스터에게 대미지를 주는 로직이 있다면 여기에 추가
            // 예: collision.collider.GetComponent<Monster>().TakeDamage(10);
            Destroy(gameObject); // 총알 파괴
        }
        // === 벽과 충돌 ===
        else if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("Bullet: 벽과 충돌! 총알 파괴.");
            Destroy(gameObject);
        }
    }
}