using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D playerRB;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Variables")]
    [SerializeField] private float movementAcceleration = 50f;
    [SerializeField] private float maxMovementSpeed = 12f;
    [SerializeField] private float groundLinearDrag = 10f;
    private float horizontalDirection;
    private bool changingDirection => (playerRB.velocity.x > 0f && horizontalDirection < 0f) || (playerRB.velocity.x < 0f && horizontalDirection > 0f); //takes away sliding when changing directions (used in ApplyLinearDrag function)

    [Header("jump Variables")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float fallMultiplier = 8f; //<--
    [SerializeField] private float lowJumpFallMultiplier = 5f; //<--
    private bool canJump => Input.GetButton("Jump") && onGround;

    [Header("Ground Collision Variables")]
    [SerializeField] private float groundRaycastLenght = 0.7f;
    private bool onGround;

    [Header("Crouching")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite standing;
    [SerializeField] private Sprite crouching;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private Vector2 standingSize;
    [SerializeField] private Vector2 standingOffset;
    [SerializeField] private Vector2 crouchingSize;
    [SerializeField] private Vector2 crouchingOffset;
    private bool canStand;
    [SerializeField] private float airRaycastLenght = 0.7f;


    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = standing;
        standingSize = boxCollider.size;
    }

    private void Update()
    {
        horizontalDirection = GetInput().x;
        if (Input.GetKeyDown(KeyCode.C))
        {
            spriteRenderer.sprite = crouching;
            boxCollider.size = crouchingSize;
            boxCollider.offset = crouchingOffset;
        }
        if (!Input.GetKey(KeyCode.C) && !canStand)
        {
            spriteRenderer.sprite = standing;
            boxCollider.size = standingSize;
            boxCollider.offset = standingOffset;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        MoveCharacter();
        if (canJump) Jump();
        if (onGround)
        {
            ApplyGroundLinearDrag();
            Debug.Log ("je staat op de grond");
        }
        else
        {
            Debug.Log("je bent in de lucht");
            ApplyAirLinearDrag();
            FallMultiplier();
        }
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void MoveCharacter()
    {
        playerRB.AddForce(new Vector2(horizontalDirection, 0f) * movementAcceleration);

        if (Mathf.Abs(playerRB.velocity.x) > maxMovementSpeed)//used to clamp max speed
            playerRB.velocity = new Vector2(Mathf.Sign(playerRB.velocity.x) * maxMovementSpeed, playerRB.velocity.y);
    }

    private void ApplyGroundLinearDrag()
    {
        if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection)
        {
            playerRB.drag = groundLinearDrag;
        }
        else
        {
            playerRB.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag()
    {
        playerRB.drag = airLinearDrag;
    }
    

    private void Jump()
    {
        playerRB.velocity = new Vector2(playerRB.velocity.x, 0f);
        playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void FallMultiplier() //makeing the slow fall go away
    {
        if (playerRB.velocity.y < 0)
        {
            playerRB.gravityScale = fallMultiplier;
        }
        else if (playerRB.velocity.y > 0 && !Input.GetButton("Jump"))
        {
           playerRB.gravityScale = lowJumpFallMultiplier;
        }
        else
        {
            playerRB.gravityScale = 1f;
        }
    }

    private void CheckCollisions()
    {
        onGround = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastLenght, groundLayer);
        canStand = Physics2D.Raycast(transform.position, Vector2.up, airRaycastLenght, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastLenght);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * airRaycastLenght);
    }
}
