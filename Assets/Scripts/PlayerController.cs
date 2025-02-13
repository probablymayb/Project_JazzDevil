using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private Animator spriteAnimator;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spriteAnimator = transform.Find("Sprite").GetComponent<Animator>();

        // 회전 제한 설정
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationY |
                        RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        ProcessInputs();
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ProcessInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void Move()
    {
        // 현재 y축 속도 유지
        float currentYVelocity = rb.linearVelocity.y;

        // 이동 방향 계산
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);

        // velocity 속성 사용
        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            currentYVelocity,
            moveDirection.z * moveSpeed
        );
    }

    private void UpdateAnimationState()
    {
        if (spriteAnimator != null)
        {
            bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
            spriteAnimator.SetBool("isMove", isMoving);
        }
    }
}