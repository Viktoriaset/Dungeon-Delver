using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMove : MonoBehaviour
{
    private IFacindMover mover;

    private void Awake()
    {
        mover = GetComponent<IFacindMover>();
    }

    private void FixedUpdate()
    {
        if (!mover.moving) return;

        int facing = mover.GetFacing();

        Vector2 rPos = mover.roomPos;
        Vector2 rPosGrid = mover.GetRoomPosOnGrid();

        float delta = 0;
        if (facing == 0 || facing == 2)
        {
            delta = rPosGrid.y - rPos.y;
        } else
        {
            delta = rPosGrid.x - rPos.x;
        }
        if (delta == 0) return;

        float move = mover.GetSpeed() * Time.fixedDeltaTime;
        move = Mathf.Min(move, Mathf.Abs(delta));
        if (delta < 0) move = -move;

        if (facing == 0 || facing == 2)
        {
            rPos.y += move;
        } else
        {
            rPos.x += move;
        }

        mover.roomPos = rPos;
    }
}
