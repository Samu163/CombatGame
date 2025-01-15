using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Updated for TextMeshProUGUI

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public KeyCode moveForwardKey = KeyCode.W;
    public KeyCode moveBackwardKey = KeyCode.S;
    public KeyCode quickLowKey = KeyCode.Z;
    public KeyCode slowLowKey = KeyCode.X;
    public KeyCode quickHighKey = KeyCode.C;
    public KeyCode slowHighKey = KeyCode.V;
    public KeyCode dodgeHighKey = KeyCode.Q;
    public KeyCode evadeLowKey = KeyCode.E;

    [Header("Movement Settings")]
    public float movementSpeed = 2.0f;
    public Transform stageBounds;
    private float previousMovementSpeed;

    [Header("Combat Settings")]
    public float attackRange = 1.0f;
    public Transform lowQuickPoint;
    public Transform lowMassivePoint;
    public Transform highMassivePoint;
    public Transform highQuickPoint;
    public Transform lowAttackPoint;
    public LayerMask opponentLayer;

    public float knockbackForce = 5.0f; // New knockback force setting

    private Animator animator;
    private bool isDefeated = false;
    public string hitTag = "KongHit";

    private bool blockForward = false;
    private bool blockBackward = false;
    private bool inmuneToHighAttacks = false;
    private bool inmuneToLowAttacks = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    private bool canMove = false;
    private Rigidbody rb;

    [Header("Life Settings")]
    public int maxLives = 25;
    private int currentLives;

    [Header("UI Settings")]
    public TextMeshProUGUI livesText; // Updated to TextMeshProUGUI

    public bool isplayer2 = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        previousMovementSpeed = movementSpeed;
        currentLives = maxLives;
        UpdateLivesUI();
    }

    private void Update()
    {
        if (isDefeated || inputsBlocked) return;

        HandleMovement();
        HandleAttacks();
    }

    private void HandleMovement()
    {
        bool isMovingForward = Input.GetKey(moveForwardKey) && !blockForward;
        bool isMovingBackward = Input.GetKey(moveBackwardKey) && !blockBackward;
        float movementDirection = 0;

        if (isMovingForward)
        {
            movementDirection = 1.5f;
            animator.SetBool("WalkForward", true);
            animator.SetBool("WalkBackward", false);
        }
        else if (isMovingBackward)
        {
            movementDirection = -1.5f;
            animator.SetBool("WalkForward", false);
            animator.SetBool("WalkBackward", true);
        }
        else
        {
            animator.SetBool("WalkForward", false);
            animator.SetBool("WalkBackward", false);
        }

        Vector3 position = transform.position;
        position.z = Mathf.Clamp(position.z + movementDirection * movementSpeed * Time.deltaTime,
                                 stageBounds.position.z - stageBounds.localScale.z / 2,
                                 stageBounds.position.z + stageBounds.localScale.z / 2);
        transform.position = position;
    }

    private void HandleAttacks()
    {
        if (Input.GetKeyDown(quickLowKey))
        {
            animator.SetTrigger("QuickLow");
            if (isplayer2)
            {
                MoveDuringAttack(3f, 0.3f);
            }
        }
        else if (Input.GetKeyDown(slowLowKey))
        {
            animator.SetTrigger("SlowLow");
        }
        else if (Input.GetKeyDown(quickHighKey))
        {
            animator.SetTrigger("QuickHigh");
        }
        else if (Input.GetKeyDown(slowHighKey))
        {
            animator.SetTrigger("SlowHigh");
            if (isplayer2)
            {
                MoveDuringAttack(8f, 0.7f);
            }
        }
        else if (Input.GetKeyDown(dodgeHighKey))
        {
            animator.SetTrigger("EvadeHigh");
            StartCoroutine(PerformCrouch());
        }
        else if (Input.GetKeyDown(evadeLowKey))
        {
            animator.SetTrigger("EvadeLow");
            StartCoroutine(PerformJump());
        }
    }

    private void MoveDuringAttack(float distance, float delay = 0.1f)
    {
        StartCoroutine(SmoothMove(distance, delay));
    }

    private IEnumerator SmoothMove(float distance, float delay)
    {
        yield return new WaitForSeconds(delay);

        float duration = 0.2f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * distance;

        while (elapsedTime < duration)
        {
            if (!blockForward && !blockBackward)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }

        transform.position = endPosition;
    }

    private IEnumerator PerformCrouch()
    {
        if (isCrouching) yield break;
        isCrouching = true;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("EvadeHigh"));

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length / 1.5f);

        isCrouching = false;
    }

    private IEnumerator PerformJump()
    {
        if (isJumping) yield break;
        isJumping = true;

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("EvadeLow"));

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        yield return new WaitForSeconds(stateInfo.length);

        isJumping = false;
    }

    public void TakeDamage()
    {
        currentLives--;
        UpdateLivesUI();
        ApplyKnockback(); // Apply knockback when damage is taken
        if (currentLives <= 0)
        {
            animator.SetTrigger("Lose");
            GameManager.instance.PlayerDefeated(this);
            isDefeated = true;
        }
    }

    private void ApplyKnockback()
    {
        Vector3 knockbackDirection = -transform.forward;
        rb.AddForce(knockbackDirection * knockbackForce*3, ForceMode.Impulse);
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + currentLives;
        }
    }

    public void TriggerWin()
    {
        animator.SetTrigger("Win");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 contactPoint = collision.contacts[0].point;
            Vector3 directionToCollision = contactPoint - transform.position;

            if (directionToCollision.z > 0)
            {
                blockForward = true;
            }
            else if (directionToCollision.z < 0)
            {
                blockBackward = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(hitTag + "High"))
        {
            if (isCrouching) return;
            TakeDamage();
        }
        else if (other.CompareTag(hitTag + "Low"))
        {
            if (isJumping) return;
            TakeDamage();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            blockForward = false;
            blockBackward = false;
        }
    }

    private bool inputsBlocked = false;

    public void BlockInputs()
    {
        inputsBlocked = true;
    }

    public void UnblockInputs()
    {
        inputsBlocked = false;
    }
}
