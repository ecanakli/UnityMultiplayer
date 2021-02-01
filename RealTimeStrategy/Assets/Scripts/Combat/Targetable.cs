using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    //To Aiming Target Pos
    [SerializeField] private Transform aimAtPoint = null;

    //To Reach AimPoint
    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
