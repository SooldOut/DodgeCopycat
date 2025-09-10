using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float mouseSensitivity = 150f;
    public Transform playerCamera; // 카메라 Transform을 연결할 변수

    private Rigidbody rb;
    private float verticalRotation = 0f; // 카메라의 상하 회전 값을 저장할 변수

    void Awake()
    {
        // Rigidbody 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        // Rigidbody의 회전을 고정하여 캐릭터가 넘어지지 않도록 합니다.
        rb.freezeRotation = true;
    }

    void Start()
    {
        // 마우스 숨기기
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 키보드 입력 받기
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // 플레이어 회전 (좌우)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // 카메라 회전 (상하)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // 상하 회전 각도 제한
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // 이동 (키보드)
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // MovePosition을 사용하면 Rigidbody를 통해 안전하게 이동할 수 있습니다.
        // 이를 통해 물리 충돌을 감지하고 벽을 통과하지 않게 됩니다.
        Vector3 finalMovement = transform.TransformDirection(moveDirection) * moveSpeed;
        rb.linearVelocity = finalMovement;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EXIT"))
        {
            // 게임 종료
            Debug.Log("게임 승리! 탈출에 성공했습니다.");
            gameObject.SetActive(false); // 플레이어 비활성화 (게임 종료 효과)
        }
    }
}