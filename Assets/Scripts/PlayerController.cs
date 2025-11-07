using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float movespeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    Animator animator;
    Vector2 movementInput;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    SpriteRenderer spriteRenderer;
    bool canMove = true;
    public SwordAttack swordAttack;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            bool success = false;

            if (movementInput != Vector2.zero)
            {
                success = TryMove(movementInput);
                if (!success)
                {
                    if (movementInput.x != 0)
                    {
                        success = TryMove(new Vector2(movementInput.x, 0));
                    }

                    if (!success && movementInput.y != 0)
                    {
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }
            }

            animator.SetBool("isMoving", success);

            if (movementInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movementInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }
    
    private bool TryMove(Vector2 direction)
    {
        
        int count = rb.Cast(
            direction,
            movementFilter,
            castCollisions,
            movespeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * movespeed * Time.fixedDeltaTime);
            return true;
        }
        else
        {
            return false;
        }

    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnAttack()
    {
        animator.SetTrigger("swordAttack");
    }
    public void SwordAttack()
    {
        Lockmovement();
        if (spriteRenderer.flipX)
        {
            swordAttack.AttackLeft();
        }
        else
        {
            swordAttack.AttackRight();
        }

    }
    public void EndSwordAttack()
    {
        swordAttack.StopAttack();
        Unlockmovement();
    }
    public void Lockmovement()
    {
        canMove = false;
    }

    public void Unlockmovement()
    {
        canMove = true;
    }
}
