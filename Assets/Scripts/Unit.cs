using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : Actor
{
	public float health = 10;
	public float maxHealth = 10;
	public float guard = 5;
	public float maxGuard = 5;
    public int moveSpeed;
	public float strength = 3;




    public int minorActions;
    public int majorActions;
	public Vector3 facing;

	public List<Ability> abilities;
	public Ability ability;

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
	public void Orient (Vector3 orientation) {
		facing = orientation;
		Vector3 lookTarget = transform.position + facing;
		transform.LookAt(lookTarget);
	}

	public void Strike (Tile origin, float strength, float guardBreak) {

		Vector3 dir = new Vector3(origin.x-tile.x,0,origin.y-tile.y);
		float dot = Vector3.Dot(dir, facing);

		if (dot >= 0.5f) {
			Debug.Log("strike front");
		}
		else if (dot >= -0.5f) {
			Debug.Log("strike side");
		}
		else {
			Debug.Log("strike back");
		}
		
	}
}
