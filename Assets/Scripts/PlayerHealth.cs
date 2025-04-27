using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");

        GameOverUI ui = FindObjectOfType<GameOverUI>();
        if (ui != null)
        {
            ui.ShowDefeat(); // <-- Исправлено
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверка попадания от пули автомата
        RifleBullet bullet = other.GetComponent<RifleBullet>();
        if (bullet != null)
        {
            TakeDamage(bullet.damage);
            Destroy(other.gameObject);
            return;
        }

        // Проверка попадания от дроби
        ShotgunBullet pellet = other.GetComponent<ShotgunBullet>();
        if (pellet != null)
        {
            TakeDamage(pellet.damage);
            Destroy(other.gameObject);
            return;
        }
    }
}
