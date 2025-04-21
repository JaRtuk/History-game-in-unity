using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    private float lastShotTime;
    private bool isAiming = false;

    public enum MovementType { Patrol, Wander }
    public MovementType movementType = MovementType.Patrol;

    [Header("Патрулирование по точкам")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    [Header("Случайное движение")]
    public float wanderRadius = 5f;
    public float wanderDelay = 2f;
    private float wanderTimer;

    [Header("Общие параметры")]
    public float moveSpeed = 2f;
    public float visionRange = 8f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    private Rigidbody2D rb;
    private Transform player;
    private Vector2 targetPosition;
    private bool playerVisible = false;

    private EnemyShooter shooter;
    private Animator animator;

    private bool isSearching = false;
    private Vector2 lastKnownPlayerPosition;
    public float searchRadius = 3f;
    private bool hasSearchTarget = false;

    private bool isWanderingAfterSearch = false;
    private float wanderAfterSearchTimer = 0f;
    private float wanderAfterSearchDuration = 3f;

    // Залипание в точке поиска
    private Vector2 searchStartPosition;
    private float timeAtSearchPosition = 0f;
    private float stuckSearchTimeout = 2f;
    private bool waitingNearLostTarget = false;

    // Защита от общего застревания
    private Vector2 lastPosition;
    private float stuckTimer = 0f;
    private float stuckCheckInterval = 0.5f;
    private float nextStuckCheckTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        shooter = GetComponent<EnemyShooter>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (movementType == MovementType.Wander)
            SetNewWanderTarget();
    }

    void Update()
    {
        DetectPlayer();

        if (playerVisible)
        {
            lastKnownPlayerPosition = player.position;
            isSearching = false;
            hasSearchTarget = false;
            isWanderingAfterSearch = false;
            waitingNearLostTarget = false;

            FaceTarget(player.position);
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);

            if (!isAiming)
            {
                isAiming = true;
                animator.SetBool("isAiming", true);
            }

            if (CanShoot())
            {
                animator.SetTrigger("Shoot");
                shooter.ShootAt(player.position);
                lastShotTime = Time.time;
            }

            return;
        }
        else if (isAiming)
        {
            isAiming = false;
            animator.SetBool("isAiming", false);
        }

        if (!playerVisible && !isSearching && lastKnownPlayerPosition != Vector2.zero)
        {
            isSearching = true;
            hasSearchTarget = false;
            waitingNearLostTarget = false;
            searchStartPosition = transform.position;
            timeAtSearchPosition = 0f;
        }

        if (isSearching)
        {
            float distanceToTarget = Vector2.Distance(transform.position, lastKnownPlayerPosition);

            if (distanceToTarget > 0.5f)
            {
                MoveTowards(lastKnownPlayerPosition);
                waitingNearLostTarget = false;
                timeAtSearchPosition = 0f;
                searchStartPosition = transform.position;
            }
            else
            {
                MoveTowards(lastKnownPlayerPosition);

                if (!waitingNearLostTarget)
                {
                    waitingNearLostTarget = true;
                    searchStartPosition = transform.position;
                    timeAtSearchPosition = 0f;
                }

                float distanceMoved = Vector2.Distance(transform.position, searchStartPosition);

                if (distanceMoved < 0.1f)
                {
                    timeAtSearchPosition += Time.deltaTime;

                    if (timeAtSearchPosition >= stuckSearchTimeout)
                    {
                        isSearching = false;
                        lastKnownPlayerPosition = Vector2.zero;
                        hasSearchTarget = false;
                        isWanderingAfterSearch = true;
                        wanderAfterSearchTimer = 0f;
                        SetNewWanderTarget();

                        Vector2 offsetTarget = (Vector2)transform.position + Random.insideUnitCircle.normalized * 0.5f;
                        StartCoroutine(SmoothOffset(offsetTarget, 0.3f));

                        waitingNearLostTarget = false;
                    }
                }
                else
                {
                    searchStartPosition = transform.position;
                    timeAtSearchPosition = 0f;
                }
            }

            return; 
        }

        if (isWanderingAfterSearch)
        {
            wanderAfterSearchTimer += Time.deltaTime;

            if (wanderAfterSearchTimer > wanderAfterSearchDuration)
            {
                isWanderingAfterSearch = false;
                if (movementType == MovementType.Wander)
                    SetNewWanderTarget();
            }
            else
            {
                Wander();
            }

            CheckIfStuck();
            return;
        }

        if (movementType == MovementType.Patrol)
        {
            Patrol();
        }
        else if (movementType == MovementType.Wander)
        {
            Wander();
        }

        CheckIfStuck();
    }

    void MoveTowards(Vector2 destination)
    {
        Vector2 dir = (destination - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.6f, obstacleLayer);
        if (hit.collider != null)
        {
            Vector2 sideStep = Vector2.Perpendicular(dir) * (Random.value > 0.5f ? 1 : -1);
            dir = (dir + sideStep).normalized;

            RaycastHit2D rehit = Physics2D.Raycast(transform.position, dir, 0.6f, obstacleLayer);
            if (rehit.collider != null)
            {
                dir = -dir;
            }
        }

        rb.velocity = dir * moveSpeed;
        animator.SetBool("isMoving", true);
        FaceTarget((Vector2)transform.position + dir);
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector2 dir = (patrolPoints[currentPatrolIndex].position - transform.position).normalized;
        rb.velocity = dir * moveSpeed;

        animator.SetBool("isMoving", true);
        FaceTarget(patrolPoints[currentPatrolIndex].position);

        if (Vector2.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.3f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;
        rb.velocity = (targetPosition - (Vector2)transform.position).normalized * moveSpeed;

        animator.SetBool("isMoving", true);
        FaceTarget(targetPosition);

        if (Vector2.Distance(transform.position, targetPosition) < 0.5f || wanderTimer > wanderDelay)
        {
            SetNewWanderTarget();
        }
    }

    void SetNewWanderTarget()
    {
        wanderTimer = 0f;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        targetPosition = (Vector2)transform.position + randomDirection * wanderRadius;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDirection, wanderRadius, obstacleLayer);
        if (hit.collider != null)
        {
            targetPosition = hit.point - randomDirection * 0.5f;
        }
    }

    void DetectPlayer()
    {
        playerVisible = false;

        if (player == null)
            return;

        Vector2 dirToPlayer = player.position - transform.position;
        float distance = dirToPlayer.magnitude;

        if (distance > visionRange)
            return;

        float angleToPlayer = Vector2.Angle(transform.up, dirToPlayer);
        float fieldOfView = 180f;

        if (angleToPlayer > fieldOfView / 2f)
            return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer.normalized, distance, obstacleLayer | playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            playerVisible = true;
        }
    }

    void FaceTarget(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position);
        if (direction.sqrMagnitude < 0.01f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    bool CanShoot()
    {
        return Time.time - lastShotTime >= shooter.GetEffectiveFireRate();
    }

    IEnumerator SmoothOffset(Vector2 target, float duration)
    {
        Vector2 start = transform.position;
        float time = 0f;

        while (time < duration)
        {
            transform.position = Vector2.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }

    void CheckIfStuck()
    {
        if (Time.time >= nextStuckCheckTime)
        {
            nextStuckCheckTime = Time.time + stuckCheckInterval;

            float movedDistance = Vector2.Distance(transform.position, lastPosition);

            if (movedDistance < 0.1f)
            {
                stuckTimer += stuckCheckInterval;

                if (stuckTimer >= 3f)
                {
                    isSearching = false;
                    hasSearchTarget = false;
                    isWanderingAfterSearch = true;
                    wanderAfterSearchTimer = 0f;
                    SetNewWanderTarget();
                    stuckTimer = 0f;
                }
            }
            else
            {
                stuckTimer = 0f;
            }

            lastPosition = transform.position;
        }
    }
}
