using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float aimSpeedMultiplier = 0.5f;
    public float rotationSpeed = 10f;

    private Animator animator;
    private Rigidbody2D rb;

    private Vector2 moveInput;
    private bool wasRunning = false;
    private int lastWeaponType = -1;
    private bool lastIsAiming = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isAiming = Input.GetMouseButton(1);
        animator.SetBool("isAiming", isAiming);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical).normalized;
        bool isRunning = moveInput.magnitude > 0.01f;

        int weaponType = animator.GetInteger("weaponType");

        // Обновляем направление взгляда на курсор
        RotateToMouse();

        // Управление анимацией
        if (isRunning != wasRunning || weaponType != lastWeaponType || isAiming != lastIsAiming)
        {
            PlayCorrectAnimation(isRunning, weaponType, isAiming);
            wasRunning = isRunning;
            lastWeaponType = weaponType;
            lastIsAiming = isAiming;
        }

        animator.SetBool("isRunning", isRunning);
    }

    void FixedUpdate()
    {
        float moveSpeed = animator.GetBool("isAiming") ? speed * aimSpeedMultiplier : speed;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void PlayCorrectAnimation(bool isRunning, int weaponType, bool isAiming)
    {
        if (isAiming && isRunning)
        {
            switch (weaponType)
            {
                case 1: animator.Play("Buckshot_aim_run"); break;
                case 2: animator.Play("Gun_aim_run"); break;
            }
        }
        else if (isAiming)
        {
            switch (weaponType)
            {
                case 1: animator.Play("Buckshot_aim_hold"); break;
                case 2: animator.Play("Gun_aim_hold"); break;
            }
        }
        else if (isRunning)
        {
            switch (weaponType)
            {
                case 0: animator.Play("Run"); break;
                case 1: animator.Play("Buckshot_run"); break;
                case 2: animator.Play("Gun_running"); break;
            }
        }
        else
        {
            switch (weaponType)
            {
                case 0: animator.Play("idle"); break;
                case 1: animator.Play("Buckshot_idle"); break;
                case 2: animator.Play("Gun_idle"); break;
            }
        }
    }

    void RotateToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        direction.z = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void ForceAnimationUpdate()
    {
        wasRunning = !wasRunning;
        lastWeaponType = -1;
        lastIsAiming = !lastIsAiming;
    }
}

