using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Set dynamically")]
    public int x;
    public int y;
    public int tileNum;

    private BoxCollider bColl;

    private void Awake()
    {
        bColl = GetComponent<BoxCollider>();
    }

    public void SetTile(int eX, int eY, int eTileNum = -1)
    {
        x = eX;
        y = eY;
        transform.localPosition = new Vector3(x, y, 0);
        gameObject.name = x.ToString("D3") + "x" + y.ToString("D3");

        if (eTileNum == -1)
        {
            eTileNum = TileCamera.GET_MAP(x, y);
        } else
        {
            TileCamera.SET_MAP(x, y, eTileNum);
        }
        tileNum = eTileNum;
        if (tileNum == -1)
        {
            print("X = " + x);
            print("Y = y" + y);
        }
        GetComponent<SpriteRenderer>().sprite = TileCamera.SPRITES[tileNum];

        SetCollider();
    }

    private void SetCollider()
    {
        bColl.enabled = true;
        char c = TileCamera.COLLISIONS[tileNum];
        switch(c)
        {
            case 'S':
                bColl.center = Vector3.zero;
                bColl.size = Vector3.one;
                break;

            case 'W':
                bColl.center = new Vector3(0, 0.25f, 0);
                bColl.size = new Vector3(1, 0.5f, 1);
                break;

            case 'X':
                bColl.center = new Vector3(0, -0.25f, 0);
                bColl.size = new Vector3(1, 0.5f, 1);
                break;

            case 'A':
                bColl.center = new Vector3(-0.25f, 0, 0);
                bColl.size = new Vector3(0.5f, 1, 1);
                break;

            case 'D':
                bColl.center = new Vector3(0.25f, 0, 0);
                bColl.size = new Vector3(0.5f, 1, 1);
                break;

            case 'Q':
                bColl.center = new Vector3(-0.25f, 0.25f, 0);
                bColl.size = new Vector3(0.5f, 0.5f, 1);
                break;

            case 'E':
                bColl.center = new Vector3(0.25f, 0.25f, 0);
                bColl.size = new Vector3(0.5f, 0.5f, 1);
                break;

            case 'Z':
                bColl.center = new Vector3(-0.25f, -0.25f, 0);
                bColl.size = new Vector3(0.5f, 0.5f, 1);
                break;

            case 'C':
                bColl.center = new Vector3(0.25f, -0.25f, 0);
                bColl.size = new Vector3(0.5f, 0.5f, 1);
                break;

            default:
                bColl.enabled = false;
                break;

        }
    }
}
