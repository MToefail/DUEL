using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode meleeAttackKey = KeyCode.Mouse0;
    public KeyCode jumpKey = KeyCode.Space;
    public string xMoveAxis = "Horizontal";

    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 12f;
    public float groundedLeeway = 0.6f;
    public LayerMask groundLayer;

    [Header("Combat")]
    public Transform meleeAttackOrigin;
    public float meleeAttackRadius = 0.6f;
    public float meleeDamage = 2f;
    public float meleeAttackDelay = 0.5f;
    public LayerMask enemyLayer;

    [Header("Audio")]
    public AudioSource attackSound;

    private Rigidbody2D rb2D;
    private Animator anim;
    private float moveIntentionX = 0f;
    private bool attemptJump = false;
    private bool attemptMeleeAttack = false;
    private float timeUntilMeleeReadied = 0;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (attackSound == null) attackSound = GetComponent<AudioSource>();

        rb2D.freezeRotation = true;
    }

    void Update()
    {
        moveIntentionX = Input.GetAxisRaw(xMoveAxis);

        if (Input.GetKeyDown(jumpKey)) attemptJump = true;
        if (Input.GetKeyDown(meleeAttackKey)) attemptMeleeAttack = true;

        if (timeUntilMeleeReadied > 0) timeUntilMeleeReadied -= Time.deltaTime;

        anim.SetFloat("Speed", Mathf.Abs(moveIntentionX));

        bool isGrounded = CheckGrounded();
        anim.SetBool("isGrounded", isGrounded);

        anim.SetFloat("AirVelocity", rb2D.linearVelocity.y);

        HandleAttack();
    }

    void FixedUpdate()
    {
        HandleRun();
        HandleJump();
    }

    private void HandleRun()
    {
        if (moveIntentionX > 0.1f) transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (moveIntentionX < -0.1f) transform.rotation = Quaternion.Euler(0, 180f, 0);

        rb2D.linearVelocity = new Vector2(moveIntentionX * speed, rb2D.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (attemptJump)
        {
            if (CheckGrounded())
            {
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);
            }
            attemptJump = false;
        }
    }

    private void HandleAttack()
    {
        if (attemptMeleeAttack && timeUntilMeleeReadied <= 0)
        {

            if (attackSound != null)
            {
                attackSound.Play();
            }


            anim.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackOrigin.position, meleeAttackRadius, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                IDamageable damageable = enemy.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(meleeDamage);
                    Debug.Log("Hit " + enemy.name);
                }
            }

            timeUntilMeleeReadied = meleeAttackDelay;
            attemptMeleeAttack = false;
        }
    }

    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundedLeeway, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * groundedLeeway, Color.red);
        return hit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundedLeeway);

        if (meleeAttackOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meleeAttackOrigin.position, meleeAttackRadius);
        }
    }
}