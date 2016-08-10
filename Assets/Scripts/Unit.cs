using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : Actor
{
    public int moveSpeed;
    public int minorActions;
    public int majorActions;
    public override void Act()
    {
        //DoAction (minorAction, majorAction etc.)
        Debug.Log("Unit -> Do Action");
        delay = actionDelay; //SetNewDelay

        //base.Act();
    }
}
