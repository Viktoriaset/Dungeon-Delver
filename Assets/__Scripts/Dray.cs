using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class Dray : MonoBehaviour, IFacindMover, IKeyMaster
{
    public enum eMode { idle, move, attack, transition, knockBack }

    [Header("Set in inspector")]
    public float speed = 5;
    public float attackDuration = 0.25f;
    public float attackDelay = 0.5f;
    public float transitionDelay = 0.5f;

    public int maxHealth = 10;
    public float knockBackSpeed = 10;
    public float knockBackDuration = 0.25f;
    public float invincibleDuration = 0.5f;

    [Header("Set dynamically")]
    public int dirHeld = -1;
    public int facing = 1;
    public eMode mode = eMode.idle;
    public bool invincible = false;
    public bool hasGrappler = false;
    public Vector3 lastSafeLoc;
    public int lastSafeFacing;

    [SerializeField]
    private int numKeys = 0;
    [SerializeField]
    private int _health;

    public int health
    {
        get => _health;
        set => _health = value;
    }

    private float timeAtkDone = 0;
    private float timeAtkNext = 0;

    private float transitionDone = 0;
    private Vector2 transitionPos;

    private float knockBackDone = 0;
    private float invincibleDone = 0;
    private Vector3 knockBackVel;

    private SpriteRenderer sRend;
    private Rigidbody rb;
    private Animator animator;
    private InRoom inRm;
    private Vector3[] directions = { Vector3.right, Vector3.up, Vector3.left, Vector3.down };
    private KeyCode[] keys = { KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, };


    private void Awake()
    {
        sRend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        inRm = GetComponent<InRoom>();
        health = maxHealth;
        lastSafeLoc = transform.position;
        lastSafeFacing = facing;
    }

    private void Update()
    {
        if (invincible && Time.time > invincibleDone) invincible = false;
        sRend.color = invincible ? Color.red : Color.white;
        if (mode == eMode.knockBack)
        {
            rb.velocity = knockBackVel;
            if (Time.time < knockBackDone) return;
        }

        if (mode == eMode.transition)
        {
            rb.velocity = Vector3.zero;
            animator.speed = 0;
            roomPos = transitionPos;
            if (Time.time < transitionDone) return;

            mode = eMode.idle;
        }

        dirHeld = -1;
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKey(keys[i])) dirHeld = i;
        }

        if (Input.GetKey(KeyCode.Z) && Time.time >= timeAtkNext)
        {
            mode = eMode.attack;
            timeAtkDone = Time.time + attackDuration;
            timeAtkNext = Time.time + attackDelay;
        }

        if (Time.time >= timeAtkDone) mode = eMode.idle;

        if (mode != eMode.attack)
        {
            if (dirHeld == -1)
            {
                mode = eMode.idle;
            } else
            {
                facing = dirHeld;
                mode = eMode.move;
            }
        }

        Vector3 vel = Vector3.zero;
        switch (mode)
        {
            case eMode.attack:
                animator.CrossFade("Dray_Attack_" + facing, 0);
                animator.speed = 0;
                break;

            case eMode.idle:
                animator.CrossFade("Dray_Walk_" + facing, 0);
                animator.speed = 0;
                break;

            case eMode.move:
                vel = directions[dirHeld];
                animator.CrossFade("Dray_Walk_" + facing, 0);
                animator.speed = 1;
                break;
        }

        rb.velocity = vel * speed;
    }

    private void LateUpdate()
    {
        Vector2 rPos = GetRoomPosOnGrid(0.5f);

        int doorNum;
        for (doorNum = 0; doorNum < 4; doorNum++)
        {
            if (rPos == InRoom.DOORS[doorNum])
            {
                break;
            }
        }

        if (doorNum > 3 || doorNum != facing) return;

        Vector2 rm = roomNum;
        switch(doorNum)
        {
            case 0:
                rm.x += 1;
                break;

            case 1:
                rm.y += 1;
                break;

            case 2:
                rm.x -= 1;
                break;

            case 3:
                rm.y -= 1;
                break;
        }

        if (rm.x >= 0 && rm.x <= InRoom.MAX_RM_X)
        {
            if (rm.y >= 0 && rm.y <= InRoom.MAX_RM_Y)
            {
                roomNum = rm;
                transitionPos = InRoom.DOORS[(doorNum + 2) % 4];
                roomPos = transitionPos;
                lastSafeLoc = transform.position;
                lastSafeFacing = facing;
                mode = eMode.transition;
                transitionDone = Time.time + transitionDelay;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (invincible) return;

        DamageEffect dEf = collision.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return;

        health -= dEf.damage;
        invincible = true;
        invincibleDone = Time.time + invincibleDuration;

        if (dEf.knockBack)
        {
            Vector3 delta = transform.position - collision.transform.position;
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

            mode = eMode.knockBack;
            knockBackDone = Time.time + knockBackDuration;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PickUp pup = other.GetComponent<PickUp>();
        if (pup == null) return;

        switch(pup.itemType)
        {
            case PickUp.eType.health:
                health = Mathf.Min(health + 2, maxHealth);
                break;

            case PickUp.eType.key:
                KeyCount++;
                break;

            case PickUp.eType.grappler:
                hasGrappler = true;
                break;
        }

        Destroy(other.gameObject);
    }

    public void ResetInRoom(int healthLoss = 0)
    {
        transform.position = lastSafeLoc;
        facing = lastSafeFacing;
        health -= healthLoss;

        invincible = true;
        invincibleDone = Time.time + invincibleDuration;
    }

    public int GetFacing()
    {
        return facing;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public Vector2 GetRoomPosOnGrid(float mult = -1)
    {
        return inRm.GetRoomPosOnGrid(mult);
    }

    public bool moving 
    { 
        get 
        {
            return (mode == eMode.move);
        }
    }

    public float gridMult => inRm.gridMult;

    public Vector2 roomPos 
    { 
        get => inRm.roomPos; 
        set => inRm.roomPos = value; 
    }
    public Vector2 roomNum 
    { 
        get => inRm.roomNum; 
        set => inRm.roomNum = value; 
    }
    public int KeyCount 
    {
        get => numKeys;
        set => numKeys = value; 
    }
}
