using UnityEngine;

public class RifleBullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 2f;
    public int damage = 5;
    private Vector2 direction;

    [HideInInspector] public GameObject owner;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        transform.right = direction;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon") || collision.gameObject == owner)
            return;

        if (collision.CompareTag("Enemy") && (owner == null || owner.tag != "Enemy"))
        {
            var enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damage);
        }

        if (collision.CompareTag("Player") && (owner == null || owner.tag != "Player"))
        {
            var playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
