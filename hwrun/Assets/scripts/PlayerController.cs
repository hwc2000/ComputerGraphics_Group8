using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 속성 설정
    public float laneDistance = 2f; 
    public float laneChangeSpeed = 10f; 
    public float forwardSpeed = 5f; 
    public float jumpForce = 7f; 

    // 상태 관리
    public bool isJumping = false; 
    public bool isSliding = false; 
    public Animator playerAnim;

    // 컴포넌트 및 참조
    [SerializeField] Transform Foot;
    [SerializeField] LayerMask ground;
    private Rigidbody rb; 

    // 레인 이동 관련
    public enum MoveState { Idle, Left, Right }
    public MoveState moveState = MoveState.Idle;
    private int currentLane = 1;         // 0=왼쪽, 1=중앙, 2=오른쪽
    private float targetX = 0f;
    private float initX = 0f;
    private CapsuleCollider capsuleCollider;
    private float originalHeight;
    private Vector3 originalCenter;

    void Start()
    {
        playerAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); 
        if (rb != null)
        {
            rb.freezeRotation = true; 
        }

        initX = transform.position.x;
        targetX = initX;
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            originalHeight = capsuleCollider.height;
            originalCenter = capsuleCollider.center;
        }
    }

    // 1. 물리 업데이트 로직 (FixedUpdate)
    void FixedUpdate()
    {
        // 앞으로 자동 이동
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, forwardSpeed);

        // X축 레인 이동 처리
        Vector3 targetPosition = new Vector3(targetX, rb.position.y, rb.position.z);
        Vector3 newXPosition = Vector3.Lerp(rb.position, targetPosition, laneChangeSpeed * Time.fixedDeltaTime);
        rb.position = new Vector3(newXPosition.x, rb.position.y, rb.position.z);

        // 레인 이동 완료 시 상태 초기화
        if (Mathf.Abs(rb.position.x - targetX) < 0.05f)
        {
             moveState = MoveState.Idle;
        }

        if (IsGrounded())
        {
            isJumping = false;
        }
    }

    // 2. 입력 및 애니메이션 처리 로직 (Update)
    void Update()
    {
        // 입력 감지 및 상태 변경
        HandleLaneInput();
        HandleJumpInput();
        HandleSlideInput();
        
        // 애니메이터 업데이트
        playerAnim.SetBool("IsJump", isJumping);
        playerAnim.SetBool("IsSlide", isSliding);
    }
    
    // 지면 체크 함수
    bool IsGrounded()
    {
        return Physics.CheckSphere(Foot.position, 0.25f, ground);
    }

    // 점프 입력 처리 및 물리 적용
    void HandleJumpInput()
    {
        // 점프는 지면에 닿아있고, 슬라이딩 상태가 아닐 때만 가능
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isSliding) 
        {
            isJumping = true;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
        }
    }

    // 슬라이드 입력 처리 및 물리 적용
    void HandleSlideInput()
    {
        // 슬라이딩 시작
        if (Input.GetKeyDown(KeyCode.DownArrow) && IsGrounded() && !isSliding)
        {
            isSliding = true;
            StartCoroutine(SlideDuration(1f)); 
        }
    }
    
    // 레인 이동 입력 처리
    void HandleLaneInput()
    {
        // 슬라이딩 중에는 레인 이동 방지
        if (isSliding) return; 
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0)
            {
                currentLane--;
                moveState = MoveState.Left;
                targetX = initX + (currentLane - 1) * laneDistance; 
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < 2)
            {
                currentLane++;
                moveState = MoveState.Right;
                targetX = initX + (currentLane - 1) * laneDistance;
            }
        }
    }

    // 슬라이드 지속 시간 코루틴
    IEnumerator SlideDuration(float duration)
{ 
    if (capsuleCollider != null)
    {
        capsuleCollider.height = originalHeight / 2f;
        capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y / 2f, originalCenter.z);
    }

    yield return new WaitForSeconds(duration);
    isSliding = false;

    // 슬라이드 종료: 콜라이더 높이 원상 복구
    if (capsuleCollider != null)
    {
        capsuleCollider.height = originalHeight;
        capsuleCollider.center = originalCenter;
    }
}
}
