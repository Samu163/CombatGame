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
    private float previousMovementSpeed;

    [Header("Combat Settings")]
    public float attackRange = 1.0f;
    public Transform lowQuickPoint;
    public Transform lowMassivePoint;
    public Transform highMassivePoint;
    public Transform highQuickPoint;
    public Transform lowAttackPoint;
    public LayerMask opponentLayer;

    private Animator animator;
    private bool isDefeated = false;
    public string hitTag = "KongHit";

    private bool blockForward = false;
    private bool blockBackward = false;
    private bool inmuneToHighAttacks = false;
    private bool inmuneToLowAttacks = false;
    private bool isJumping = false;
    private bool isCrouching = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        previousMovementSpeed = movementSpeed;
    }

    private void Update()
    {
        if (isDefeated || inputsBlocked) return;

        HandleMovement();
        HandleAttacks();
    }

    private void HandleMovement()
    {
        // Movimiento basado en las teclas
        bool isMovingForward = Input.GetKey(moveForwardKey) && !blockForward;
        bool isMovingBackward = Input.GetKey(moveBackwardKey) && !blockBackward;
        float movementDirection = 0;

        if (isMovingForward)
        {
            movementDirection = 1; // Mover adelante
            animator.SetBool("WalkForward", true);
            animator.SetBool("WalkBackward", false);
        }
        else if (isMovingBackward)
        {
            movementDirection = -1; // Mover atr�s
            animator.SetBool("WalkForward", false);
            animator.SetBool("WalkBackward", true);
        }
        else
        {
            animator.SetBool("WalkForward", false);
            animator.SetBool("WalkBackward", false);
        }

        // Actualiza la posici�n solo si no est� bloqueada
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
        }
        else if (Input.GetKeyDown(slowLowKey)) // Slow Low Attack
        {
            animator.SetTrigger("SlowLow");
        }
        else if (Input.GetKeyDown(quickHighKey)) // Quick High Attack
        {
            animator.SetTrigger("QuickHigh");
        }
        else if (Input.GetKeyDown(slowHighKey)) // Slow High Attack
        {
            animator.SetTrigger("SlowHigh");
        }
        else if (Input.GetKeyDown(dodgeHighKey)) // Dodge High Attack
        {
            animator.SetTrigger("EvadeHigh");
            StartCoroutine(PerformCrouch());

        }
        else if (Input.GetKeyDown(evadeLowKey)) // Evade Low Attack
        {
            animator.SetTrigger("EvadeLow");
            StartCoroutine(PerformJump());
        }
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

        //animator.SetTrigger("EvadeHigh");
        // Esperar hasta que la animaci�n de agacharse comience
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("EvadeLow"));

        // Obtener la duraci�n de la animaci�n de agacharse
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length /2.0f);

        isJumping = false;
    }

    public void TakeDamage()
    {
        animator.SetTrigger("Lose");
        GameManager.instance.PlayerDefeated(this);
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
        if (other.CompareTag(hitTag + "High") )
        {
            if(isCrouching)
            {
                return;
            }
            else
            {
                TakeDamage();

            }
        }
        else if(other.CompareTag(hitTag + "Low"))
        {
            if (isJumping)
            {
                return;
            }
            else
            {
                TakeDamage();
            }
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
