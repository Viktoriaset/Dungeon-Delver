using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMove : MonoBehaviour
{
    private IFacindMover mover;

    private void Awake()
    {
        mover = GetComponent<IFacindMover>();
    }


}
