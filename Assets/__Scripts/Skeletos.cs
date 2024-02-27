using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Skeletos : Enemy, IFacindMover
{
    [Header("Set in inspector: Skeletos")]
    public int speed = 2;
    public float timeThinkMin = 1f;
    public float timeThinkMax = 4f;

    [Header("Set dynamically: Skeletos")]
    public int facing = 0;
    public float timeNextDecision = 0;

    private InRoom inRm;

    public bool moving => true;

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

    protected override void Awake()
    {
        base.Awake();
        inRm = GetComponent<InRoom>();
    }

    override protected void Update()
    {
        base.Update();
        if (knockBack) return;

        if (Time.time >= timeNextDecision)
        {
            DecideDirection();
        }

        rb.velocity = directions[facing] * speed;
    }

    private void DecideDirection()
    {
        facing = Random.Range(0, 4);
        timeNextDecision = Time.time + Random.Range(timeThinkMin, timeThinkMax);
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
}
