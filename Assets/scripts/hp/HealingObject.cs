using UnityEngine;

public class HealingObject : MonoBehaviour
{
    public int healAmount = 10;
    public float healInterval = 1f;
    public float triggerDistance = 2f;

    private Transform player;
    private bool canHeal = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("HealPlayer", 0f, healInterval);
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= triggerDistance && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(gameObject);
        }
    }

    private void HealPlayer()
    {
        if (canHeal)
        {
         player.GetComponent<HealthManager>().Heal(healAmount);
        }
    }
}
