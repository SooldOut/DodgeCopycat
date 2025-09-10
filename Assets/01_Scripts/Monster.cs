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
            Debug.LogError("PlayerController�� ���� �÷��̾� ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        if (target != null)
        {
            // 1. �÷��̾ ���� Y�����θ� ȸ���մϴ�.
            // Y ȸ���� LookAt�� ����ϰ�, X ȸ���� �����ϴ� ���� �ƴ϶� �����մϴ�.
            Vector3 targetPositionOnPlane = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.LookAt(targetPositionOnPlane);

            // 2. ����(X-Z) �������θ� �̵��մϴ�.
            // transform.forward�� ������� �ʰ�, �÷��̾ ���ϴ� ���͸� ���� ����մϴ�.
            Vector3 moveDirection = (targetPositionOnPlane - transform.position).normalized;

            // Rigidbody�� ����� �̵��մϴ�.
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