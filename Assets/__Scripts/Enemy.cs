using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] directions =
        { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    [Header("Set in inspector: Enemy")]
    public float maxHealth = 1;

    [Header("Set dynamically: Enemy")]
    public float health;

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
}
