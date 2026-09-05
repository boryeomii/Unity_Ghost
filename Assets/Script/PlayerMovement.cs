using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    [SerializeField]
    private float defaultSpeed = 4f;

    private float currentSpeed;

    private Rigidbody2D rigidbody;
    private Vector2 moveInput;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        currentSpeed = defaultSpeed;
    }

    private void Update()
    {
        if (!photonView.IsMine || GameManager.instance.isGameover)
        {
            moveInput = Vector2.zero;
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(x, y).normalized;
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Vector2 nextPosition = rigidbody.position + moveInput * currentSpeed * Time.fixedDeltaTime;

        rigidbody.MovePosition(nextPosition);
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }

    public void ResetSpeed()
    {
        currentSpeed = defaultSpeed;
    }
}
