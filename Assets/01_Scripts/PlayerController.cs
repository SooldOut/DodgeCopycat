using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float mouseSensitivity = 150f;
    public Transform playerCamera;

    private Rigidbody rb;
    private float verticalRotation = 0f;

    // === GameManager 변수 추가 ===
    private GameManager gameManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Start()
    {
        // === GameManager 오브젝트 찾아서 연결 ===
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager를 씬에서 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // 게임이 시작되지 않았거나 멈춰있으면 움직이지 않습니다.
        if (Time.timeScale == 0) return;

        // 키보드 입력 받기
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // 플레이어 회전 (좌우)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // 카메라 회전 (상하)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // 이동 (키보드)
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        Vector3 finalMovement = transform.TransformDirection(moveDirection) * moveSpeed;
        rb.linearVelocity = finalMovement;
    }

    // === 충돌 처리 함수 수정 ===
    void OnTriggerEnter(Collider other)
    {
        // 다이아몬드와 접촉하면 게임 승리 함수 호출
        if (other.CompareTag("Diamond"))
        {
            gameManager.WinGame();
            Destroy(other.gameObject); // 다이아몬드 파괴
        }

        // 총알(몬스터)과 접촉하면 게임 패배 함수 호출
        if (other.CompareTag("Bullet"))
        {
            gameManager.LoseGame();
        }
    }
}