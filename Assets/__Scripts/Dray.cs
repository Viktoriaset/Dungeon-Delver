using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class Dray : MonoBehaviour, IFacindMover
{
    public enum eMode { idle, move, attack, transition }

    [Header("Set in inspector")]
    public float speed = 5;
    public float attackDuration = 0.25f;
    public float attackDelay = 0.5f;

    [Header("Set dynamically")]
    public int dirHeld = -1;
    public int facing = 1;
    public eMode mode = eMode.idle;

    private float timeAtkDone = 0;
    private float timeAtkNext = 0;

    private Rigidbody rb;
    private Animator animator;
    private InRoom inRm;
    private Vector3[] directions = { Vector3.right, Vector3.up, Vector3.left, Vector3.down };
    private KeyCode[] keys = { KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, };


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        inRm = GetComponent<InRoom>();
    }

    private void Update()
    {
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

}
