using System.Collections;
using UnityEngine;

public class MoveForwardProjectile : MonoBehaviour
{
    public float speed; // Скорость движения
    public float maxDistance; // Максимальное расстояние, которое может пройти снаряд

    private Vector2 targetDirection; // Направление движения снаряда
    private float distanceTraveled; // Пройденное расстояние

    void Update()
    {
        MoveProjectile();
        TrackDistance();
    }

    public void SetTargetDirection(Vector2 direction)
    {
        targetDirection = direction.normalized;
    }

    void MoveProjectile()
    {
        transform.Translate(targetDirection * speed * Time.deltaTime);
    }

    void TrackDistance()
    {
        distanceTraveled += speed * Time.deltaTime;
        if (distanceTraveled >= maxDistance)
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != other.CompareTag("Enemy"))
        {
            DestroyProjectile();
        }
    }
}
