using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public KeyCode moveForwardKey = KeyCode.W; // Tecla para avanzar
    public KeyCode moveBackwardKey = KeyCode.S; // Tecla para retroceder
    public KeyCode quickLowKey = KeyCode.Z;
    public KeyCode slowLowKey = KeyCode.X;
    public KeyCode quickHighKey = KeyCode.C;
    public KeyCode slowHighKey = KeyCode.V;
    public KeyCode dodgeHighKey = KeyCode.Q;
    public KeyCode evadeLowKey = KeyCode.E;

    [Header("Movement Settings")]
    public float movementSpeed = 2.0f;
    public Transform stageBounds;

    [Header("Combat Settings")]
    public float attackRange = 1.0f;
    public Transform lowQuickPoint;
    public Transform lowMassivePoint;
    public Transform highMassivePoint;
    public Transform highQuickPoint;
    public Transform lowAttackPoint;
    public LayerMask opponentLayer;

    [Header("Game Settings")]
    public GameObject winUI; // UI displayed when the player wins

    private Animator animator;
    private bool isDefeated = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (winUI != null)
        {
            winUI.SetActive(false); // Ensure win UI is hidden at the start
        }
    }

    private void Update()
    {
        if (isDefeated) return;

        HandleMovement();
        HandleAttacks();
    }

    private void HandleMovement()
    {
        // Movement logic with specific keys
        bool isMovingForward = Input.GetKey(moveForwardKey);
        bool isMovingBackward = Input.GetKey(moveBackwardKey);
        float movementDirection = 0;

        if (isMovingForward)
        {
            movementDirection = 1; // Forward
            animator.SetBool("WalkForward", true);
            animator.SetBool("WalkBackward", false);
        }
        else if (isMovingBackward)
        {
            movementDirection = -1; // Backward
            animator.SetBool("WalkForward", false);
            animator.SetBool("WalkBackward", true);
        }
        else
        {
            animator.SetBool("WalkForward", false);
            animator.SetBool("WalkBackward", false);
        }

        // Move character along the Z-axis
        Vector3 position = transform.position;
        position.z = Mathf.Clamp(position.z + movementDirection * movementSpeed * Time.deltaTime,
                                 stageBounds.position.z - stageBounds.localScale.z / 2,
                                 stageBounds.position.z + stageBounds.localScale.z / 2);
        transform.position = position;
    }

    private void HandleAttacks()
    {
        if (Input.GetKeyDown(quickLowKey)) // Quick Low Attack
        {
            animator.SetTrigger("QuickLow");
            PerformAttack(5, lowAttackPoint);
        }
        else if (Input.GetKeyDown(slowLowKey)) // Slow Low Attack
        {
            animator.SetTrigger("SlowLow");
            PerformAttack(7, lowMassivePoint);
        }
        else if (Input.GetKeyDown(quickHighKey)) // Quick High Attack
        {
            animator.SetTrigger("QuickHigh");
            //PerformAttack(attackRange * 1.5f, highQuickPoint);
        }
        else if (Input.GetKeyDown(slowHighKey)) // Slow High Attack
        {
            animator.SetTrigger("SlowHigh");
            PerformAttack(attackRange * 1.5f, highMassivePoint);
        }
        else if (Input.GetKeyDown(dodgeHighKey)) // Dodge High Attack
        {
            animator.SetTrigger("EvadeHigh");
        }
        else if (Input.GetKeyDown(evadeLowKey)) // Evade Low Attack
        {
            animator.SetTrigger("EvadeLow");
        }
    }

    private void PerformAttack(float range, Transform attackPosition)
    {
        Collider2D[] hitOpponents = Physics2D.OverlapCircleAll(attackPosition.position,range, opponentLayer);
        foreach (var opponent in hitOpponents)
        {
            opponent.GetComponent<PlayerController>()?.TakeDamage();
        }
    }

    public void TakeDamage()
    {
        animator.SetTrigger("GetHit");
        TriggerWin();
    }

    private void TriggerWin()
    {
        // Display the win UI and stop all actions
        if (winUI != null)
        {
            winUI.SetActive(true);
        }
        isDefeated = true;
        animator.SetTrigger("Defeat");
    }

    private void OnDrawGizmosSelected()
    {
        if (lowQuickPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lowQuickPoint.position, 5);
        }
        if (lowMassivePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lowMassivePoint.position, 7);
        }
        if (highQuickPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(highQuickPoint.position, attackRange * 1.5f);
        }
        if (highMassivePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(highMassivePoint.position, attackRange * 1.5f);
        }
        if (lowAttackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lowAttackPoint.position, attackRange);
        }
    }

    [Header("Attack Settings")]
    public GameObject attackColliderPrefab; // Prefab del collider del ataque
    public Transform attackSpawnPoint; // Punto de spawn del collider
    public float colliderDuration = 0.5f; // Duración del collider en segundos

    private void SpawnCollider()
    {
        // Instancia el prefab del collider en el punto de spawn
        GameObject collider = Instantiate(attackColliderPrefab, attackSpawnPoint.position, Quaternion.identity);

        // Configura su destrucción automática después de un tiempo
        Destroy(collider, colliderDuration);
    }
    
}
