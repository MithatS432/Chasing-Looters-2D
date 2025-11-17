using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D prb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Player Settings")]
    [SerializeField] private float speed;
    private float attackIndex = 0f;

    void Start()
    {
        prb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (prb.linearVelocity.x > 0)
            spriteRenderer.flipX = false;
        else if (prb.linearVelocity.x < 0)
            spriteRenderer.flipX = true;

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetFloat("attackIndex", attackIndex);
            anim.SetTrigger("Attack");

            attackIndex++;
            if (attackIndex > 2f) attackIndex = 0f;
        }
    }
    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(x, y);
        prb.linearVelocity = move * speed;
        anim.SetFloat("Speed", Mathf.Abs(x) + Mathf.Abs(y));
    }
    private void OnCollisionEnter2D(Collision2D other)
    {

    }
}
