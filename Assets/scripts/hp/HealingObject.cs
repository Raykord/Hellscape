using UnityEngine;

public class HealingObject : MonoBehaviour
{
    public float attractionRadius = 5f;
    public float attractionSpeed = 5f;
    public int healAmount = 20;

    private Transform playerTransform;
    private bool isAttracted = false;
    private TrailRenderer trailRenderer;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        if (isAttracted)
        {
            Vector3 direction = playerTransform.position - transform.position;
            transform.Translate(direction.normalized * attractionSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                playerTransform.GetComponent<HealthManager>().Heal(healAmount);
                HandleObjectDestroy();
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, playerTransform.position) < attractionRadius)
            {
                isAttracted = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthManager>().Heal(healAmount);
            HandleObjectDestroy();
        }
    }

    private void HandleObjectDestroy()
    {
        trailRenderer.emitting = false;
        Destroy(gameObject, trailRenderer.time);
    }
}
