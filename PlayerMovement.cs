using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool active;

    void Update()
    {
        if(active)
        {   float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 movement = new Vector3(horizontal, 0f, vertical);

            transform.position += movement.normalized * moveSpeed * Time.deltaTime;
        }
    }
}
