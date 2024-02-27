using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] directions =
        { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    [Header("Set in inspector: Enemy")]
    public float maxHealth = 1;
    public float knockBackSpeed = 10;
    public float knockBackDuration = 0.25f;
    public float invincibleDuration = 0.5f;
    public GameObject[] randomItemDrops;
    public GameObject guaranteedItemDrop = null;

    [Header("Set dynamically: Enemy")]
    public float health;
    public bool invincible = false;
    public bool knockBack = false;

    private float invincibleDone = 0;
    private float knockBackDone = 0;
    private Vector3 knockBackVel;

    protected Animator animator;
    protected Rigidbody rb;
    protected SpriteRenderer sRend;

    protected virtual void Awake()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        sRend = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (invincible && Time.time > invincibleDone) invincible = false;
        sRend.color = invincible ? Color.red : Color.white;
        if (knockBack)
        {
            rb.velocity = knockBackVel;
            if (Time.time < knockBackDone) return;
        }

        animator.speed = 1;
        knockBack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (invincible) return;

        DamageEffect dEf = other.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return;

        health -= dEf.damage;
        if (health <= 0) Die();

        invincible = true;
        invincibleDone = Time.time + invincibleDuration;

        if (dEf.knockBack)
        {
            Vector3 delta = transform.position - other.transform.root.position;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            } else
            {
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }

            knockBackVel = delta * knockBackSpeed;
            rb.velocity = knockBackVel;

            knockBack = true;
            knockBackDone = Time.time + knockBackDuration;
            animator.speed = 0;
        }
    }

    private void Die()
    {
        GameObject go;
        if (guaranteedItemDrop != null)
        {
            go = Instantiate(guaranteedItemDrop);
            go.transform.position = transform.position;
        } else if (randomItemDrops.Length > 0)
        {
            int n = Random.Range(0, randomItemDrops.Length);
            GameObject prefab = randomItemDrops[n];
            if (prefab != null)
            {
                go = Instantiate(prefab);
                go.transform.position = transform.position;
            }
        }
        Destroy(gameObject);

    }
}
