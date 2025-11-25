using UnityEngine;

public class Allies : MonoBehaviour
{
    private Rigidbody2D arb;
    private Animator aa;
    private SpriteRenderer[] allSprites;
    private AudioSource deathSound;

    public float health;
    public float speed;
    void Start()
    {
        arb = GetComponent<Rigidbody2D>();
        aa = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>();
        allSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {

    }
}
