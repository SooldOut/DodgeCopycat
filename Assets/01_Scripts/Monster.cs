using UnityEngine;

public class Monster : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Transform target;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        target = FindObjectOfType<PlayerController>()?.transform;
        if (target == null)
        {
            Debug.LogError("PlayerController를 가진 플레이어 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (target != null)
        {
            // 1. 플레이어를 향해 Y축으로만 회전합니다.
            // Y 회전은 LookAt을 사용하고, X 회전은 고정하는 것이 아니라 무시합니다.
            Vector3 targetPositionOnPlane = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.LookAt(targetPositionOnPlane);

            // 2. 수평(X-Z) 방향으로만 이동합니다.
            // transform.forward를 사용하지 않고, 플레이어를 향하는 벡터를 직접 계산합니다.
            Vector3 moveDirection = (targetPositionOnPlane - transform.position).normalized;

            // Rigidbody를 사용해 이동합니다.
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Die();
            }
        }
    }
}