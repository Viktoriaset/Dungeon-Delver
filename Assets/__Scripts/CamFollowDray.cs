using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowDray : MonoBehaviour
{
    static public bool TRANSITIONING = false;

    [Header("Set in inspector")]
    public InRoom drayInRoom;
    public float transTime = 0.5f;

    private Vector3 p0, p1;

    private InRoom inRm;
    private float transStart;

    private void Awake()
    {
        inRm = GetComponent<InRoom>();
    }

    private void Update()
    {
        if (TRANSITIONING)
        {
            float u = (Time.time - transStart) / transTime;
            if (u >= 1)
            {
                u = 1;
                TRANSITIONING = false;
            }

            transform.position = (1 - u) * p0 + u * p1;
        } else
        {
            if (drayInRoom.roomNum != inRm.roomNum)
            {
                TransitionTo(drayInRoom.roomNum);
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 tPos = transform.position;
        tPos.z = -10;
        transform.position = tPos;
    }

    private void TransitionTo(Vector2 rm)
    {
        p0 = transform.position;
        inRm.roomNum = rm;
        p1 = transform.position + Vector3.back * 10;
        transform.position = p0;

        transStart = Time.time;
        TRANSITIONING = true;
    }
}
