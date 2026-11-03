
using UnityEngine;

public class SeekFleeSteering : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Distance")]
    public float safetyDistance = 5f;
    public float stopDistance = 0.5f;

    [Header("Steering")]
    public float acceleration = 8f;
    public float maxSpeed = 5f;
    public float turnSpeed = 5f;

    private float currentSpeed;
    private Vector3 movementDirection;

    void Update()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance < safetyDistance)
        {
            Flee(direction);
        }
        else
        {
            Seek(direction, distance);
        }

        UpdateMovement();
    }

    void Seek(Vector3 direction, float distance)
    {
        if (distance <= stopDistance)
        {
            movementDirection = Vector3.zero;
            currentSpeed = 0f;
            return;
        }

        movementDirection = direction.normalized;
    }

    void Flee(Vector3 direction)
    {
        movementDirection = -direction.normalized;
    }

    void UpdateMovement()
    {
        if (movementDirection.sqrMagnitude > 0.001f)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            Quaternion targetRotation =
                Quaternion.LookRotation(movementDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );

            transform.position += transform.forward
                * currentSpeed * Time.deltaTime;
        }
        else
        {
            currentSpeed = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, safetyDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
