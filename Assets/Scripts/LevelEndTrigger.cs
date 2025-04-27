using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (AreAllEnemiesDead())
            {
                FindObjectOfType<GameOverUI>().ShowVictory();
            }
            else
            {
                Debug.Log("Еще остались враги!");
            }
        }
    }

    private bool AreAllEnemiesDead()
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        return enemies.Length == 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

