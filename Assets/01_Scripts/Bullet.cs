using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50;
    private Rigidbody bulletRigidbody;


    void Start()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletRigidbody.linearVelocity = transform.forward * speed;

        Destroy(gameObject, 3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var playerController = collision.collider.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Die();
            }
            Destroy(gameObject); // �浹 �� ź ���� ����
        }

        if (collision.collider.CompareTag("Wall"))
        {
            var playerController = collision.collider.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Die();
            }
            Destroy(gameObject); // �浹 �� ź ���� ����
        }
    }
}
