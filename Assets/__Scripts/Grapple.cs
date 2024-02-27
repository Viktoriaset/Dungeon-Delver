using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public enum eMode { none, gOut, gInMiss, gInHit}

    [Header("Set in inspector")]
    public float grappleSpd = 10;
    public float grappleLength = 7;
    public float grappleInLength = 0.5f;
    public int unsafeTileHealthPenalty = 2;
    public TextAsset mapGrappleable;

    [Header("Set dynamically")]
    public eMode mode = eMode.none;
    public List<int> grappleTiles;
    public List<int> unsafeTiles;

    private Dray dray;
    private Rigidbody rb;
    private Animator animator;
    private Collider drayColld;

    private GameObject grapHead;
    private LineRenderer grapLine;
    private Vector3 p0, p1;
    private int facing;

    private Vector3[] directions = 
    { 
        Vector3.right, 
        Vector3.up, 
        Vector3.left, 
        Vector3.down
    };

    private void Awake()
    {
        string gTiles = mapGrappleable.text;
        gTiles = Utils.RemoveLineEndings(gTiles);
        grappleTiles = new List<int>();
        unsafeTiles = new List<int>();
        for (int i = 0; i < gTiles.Length; i++)
        {
            switch(gTiles[i])
            {
                case 'S':
                    grappleTiles.Add(i);
                    break;

                case 'U':
                    unsafeTiles.Add(i);
                    break;
            }
        }

        dray = GetComponent<Dray>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        drayColld = GetComponent<Collider>();

        Transform trans = transform.Find("Grappler");
        grapHead = trans.gameObject;
        grapLine = grapHead.GetComponent<LineRenderer>();
        grapHead.SetActive(false);
    }

    private void Update()
    {
        if (!dray.hasGrappler) return;

        switch(mode)
        {
            case eMode.none:
                if (Input.GetKeyDown(KeyCode.X))
                    StartGrapple();
                break;
        }
    }

    private void StartGrapple()
    {
        facing = dray.GetFacing();
        dray.enabled = false;
        animator.CrossFade("Dray_Attack_" + facing, 0);
        drayColld.enabled = false;
        rb.velocity = Vector3.zero;

        grapHead.SetActive(true);

        p0 = transform.position + (directions[facing] * 0.5f);
        p1 = p0;
        grapHead.transform.position = p1;
        grapHead.transform.rotation = Quaternion.Euler(0, 0, 90 * facing);

        grapLine.positionCount = 2;
        SetPositionGrapLine(0, p0);
        SetPositionGrapLine(1, p1);
        mode = eMode.gOut;
    }

    private void SetPositionGrapLine(int point, Vector3 position)
    {
        position.z = -1;
        grapLine.SetPosition(point, position);
    }

    private void FixedUpdate()
    {
        switch(mode)
        {
            case eMode.gOut:
                p1 += directions[facing] * grappleSpd * Time.fixedDeltaTime;
                grapHead.transform.position = p1;
                SetPositionGrapLine(1, p1);

                int tileNum = TileCamera.GET_MAP(p1.x, p1.y);
                if (grappleTiles.IndexOf(tileNum) != -1)
                {
                    mode = eMode.gInHit;
                    break;
                }
                if ((p1 - p0).magnitude >= grappleLength)
                {
                    mode = eMode.gInMiss;
                }
                break;

            case eMode.gInMiss:
                p1 -= directions[facing] * grappleSpd * Time.fixedDeltaTime;
                if (Vector3.Dot((p1 - p0), directions[facing]) > 0)
                {
                    grapHead.transform.position = p1;
                    SetPositionGrapLine(1, p1);
                } else
                {
                    StopGrapple();
                }
                break;

            case eMode.gInHit:
                float dist = grappleInLength + grappleSpd * Time.fixedDeltaTime;
                if (dist > (p1 - p0).magnitude)
                {
                    p0 = p1 - (directions[facing] * grappleInLength);
                    transform.position = p0;
                    StopGrapple();
                    break;
                }
                p0 += directions[facing] * grappleSpd * Time.fixedDeltaTime;
                transform.position = p0;
                SetPositionGrapLine(0, p0);
                grapHead.transform.position = p1;
                break;
        }
    }

    private void StopGrapple()
    {
        dray.enabled = true;
        drayColld.enabled = true;

        int tileNum = TileCamera.GET_MAP(p0.x, p0.y);
        if (mode == eMode.gInHit && unsafeTiles.IndexOf(tileNum) != -1)
        {
            dray.ResetInRoom(unsafeTileHealthPenalty);
        }

        grapHead.SetActive(false);

        mode = eMode.none;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy e = other.GetComponent<Enemy>();
        if (e == null) return;

        mode = eMode.gInMiss;
    }
}
