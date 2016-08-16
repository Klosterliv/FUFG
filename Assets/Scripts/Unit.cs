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
		MouseInput.instance.SetControlled(this);
        //delay = actionDelay; //SetNewDelay
    }
	public void Moved (float weight, Tile to) {
		delay = actionDelay;
		tile.occupied = false;
		tile.occupier = null;

		to.occupied = true;
		to.occupier = this;
		tile = to;

	}
}
