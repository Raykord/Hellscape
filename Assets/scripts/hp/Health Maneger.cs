using UnityEngine;
using System.Collections.Generic;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public GameObject healPrefab;
    public float spawnHealRadius = 1.5f;
    private List<Vector2> usedPositions = new List<Vector2>();

    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (this.CompareTag("Player"))
        {

        }
        else
        {
            this.gameObject.SetActive(false);

            int numberOfPrefabsToSpawn = Random.Range(2, 7); 

            for (int i = 0; i < numberOfPrefabsToSpawn; i++)
            {
                Vector2 spawnPosition = GetUniqueSpawnPosition();
                Instantiate(healPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
    }

    private Vector2 GetUniqueSpawnPosition()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnHealRadius;
        Vector2 spawnPosition = new Vector2(transform.position.x + randomPoint.x, transform.position.y + randomPoint.y);

        while (usedPositions.Contains(spawnPosition))
        {
            randomPoint = Random.insideUnitCircle * spawnHealRadius;
            spawnPosition = new Vector2(transform.position.x + randomPoint.x, transform.position.y + randomPoint.y);
        }

        usedPositions.Add(spawnPosition);
        return spawnPosition;
    }

    public int GetHealth()
    {
        return currentHealth;
    }
}
