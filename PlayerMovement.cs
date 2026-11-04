using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 120f;
    public bool active;

    private void Update()
    {
        if(active)
        {
            float turn = Input.GetAxis("Horizontal");
            float move = Input.GetAxis("Vertical");

            transform.Rotate(0f, turn * turnSpeed * Time.deltaTime, 0f);

            transform.position += transform.forward * move * moveSpeed * Time.deltaTime;
        }
    }
}
