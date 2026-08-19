using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    public float moveSpeed = 2f;
    private Vector3 moveDir;
    private float xInput, zInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        CheckInput();

        Rotation();
    }

    private void FixedUpdate()
    {     
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);
        anim.SetBool("Walk", moveDir != Vector3.zero);
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(xInput, 0, zInput).normalized;
    }

    private void Rotation()
    {
        if(moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }
}
