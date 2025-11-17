using UnityEngine;
using UnityEngine.InputSystem;

public class QueryChanController : MonoBehaviour
{
    public float speed = 3.0f;
    public float jumpPower = 6.0f;
    private Vector3 velocity;
    private CharacterController controller;
    private Animator anim;
    private Vector2 moveInput;
    private bool jumpInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 수평 이동 처리
        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 moveDirection = Vector3.zero;

        if (inputDirection.magnitude > 0.1f)
        {
            transform.LookAt(transform.position + inputDirection);
            moveDirection = transform.forward * speed;
            anim.SetFloat("Speed", inputDirection.magnitude * speed);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }

        // 점프 처리 (땅에 있을 때만)
        if (controller.isGrounded)
        {
            velocity.y = -2f;

            if (jumpInput)
            {
                anim.SetTrigger("Jump");
                velocity.y = jumpPower;
                jumpInput = false; // 점프 입력 소비
            }
        }

        // 중력 적용
        velocity.y += Physics.gravity.y * Time.deltaTime;

        // 최종 이동
        Vector3 finalMovement = moveDirection * Time.deltaTime + velocity * Time.deltaTime;
        controller.Move(finalMovement);
    }

    // 새로운 Input System 콜백
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput = true;
        }
    }
}