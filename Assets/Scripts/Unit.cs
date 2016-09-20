using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : Actor
{
	public float health = 10;
	public float maxHealth = 10;
	public float guard = 5;
	public float maxGuard = 5;
	public float guardPercent = 1;
    public int moveSpeed;
	public float strength = 3;

	public int alliance = 0;

    public int minorActions;
    public int majorActions;
	public Vector3 facing;

	public Ability attackAbility;
	public Ability rushAbility;

	public List<Ability> abilities;
	public Ability ability;

	// GUARD ZONE
	public float guardRange;
	public float guardAngle;
	//public string guardPatternPath = "Assets/Abilities/Patterns/Guard sets/default.asset";

	public GameObject guardPatternSet;
	public GuardPattern guardPattern;

	public GuardEffect guardEffect;

	public Animator animator;



    public override void Act()
    {
        //DoAction (minorAction, majorAction etc.)
        //Debug.Log("Unit -> Do Action");
		MouseInput.instance.SetControlled(this);
        //delay = actionDelay; //SetNewDelay
    }
	public void Pushed (Tile to) {

		//TODO:: dunno; shit
		if (to==null) to = tile;
		
		tile.occupied = false;
		tile.occupier = null;

		to.occupied = true;
		to.occupier = this;
		tile = to;
		x=tile.x;
		y=tile.y;

	}
	public void Moved (float weight, Tile to) {
		
		delay = actionDelay;
		tile.occupied = false;
		tile.occupier = null;

		to.occupied = true;
		to.occupier = this;
		tile = to;
		x=tile.x;
		y=tile.y;

	}
	public void Orient (Vector3 orientation) {
		facing = orientation;
		Vector3 lookTarget = transform.position + facing;
		transform.LookAt(lookTarget);
		//ApplyGuard();
	}
	public void OrientVisual (Vector3 orientation) {
		Vector3 lookTarget = transform.position + orientation;
		transform.LookAt(lookTarget);
	}

	public void Strike (Tile origin, float strength, float guardBreak) {

		Vector3 dir = new Vector3(origin.x-tile.x,0,origin.y-tile.y);
		float dot = Vector3.Dot(dir, facing);

		//float heightAdvantage = tile.grid.GetHeightDelta(origin, tile);

		if (dot >= 0.5f) {
			//Debug.Log("strike front");
			guardBreak*=1;
			strength*=1;
		}
		else if (dot >= -0.5f) {
			//Debug.Log("strike side");
			guardBreak*=2;
			strength*=1.5f;
		}
		else {
			//Debug.Log("strike back");
			guardBreak*=3;
			strength*=2;
		}
		BreakGuard(guardBreak, strength);

		if (guardPercent < 0.5f) {

			if (guardPercent < 0.2f) {
				Damage(strength);
			}
			Vector3 pushDir = new Vector3(tile.x-origin.x,0,tile.y-origin.y);
			Mover.instance.AddMoving(new Moving(this, pushDir, 1));

		}

		
	}
	void UpdateGuard() {
		if (guard < 0) guard = 0;
		if (guard == 0) guardPercent = 0;
		else guardPercent = guard/maxGuard;
	}
	public void BreakGuard (float guardBreak, float strength) {
		guard -= (guardBreak*(strength*0.5f));
		UpdateGuard();
	}
	public void Damage (float damage) {
		health -= damage;
	}
	public void SetGuardPattern(GuardPattern gp) {
		guardPattern = gp;
	}
	public override void Initialize () {
		SetGuardPattern(guardPatternSet.GetComponent<GuardPattern>());
		guardEffect = gameObject.AddComponent<GuardEffect>();
		//guardEffect = GetComponent<GuardEffect>();
		//guardEffect = new GuardEffect(this);
		guardEffect.parent = this;
		animator = GetComponent<Animator>();

		//tile = FindObjectOfType<Grid>().grid[x, y];
		//Debug.Log(tile.gameObject.transform.position);
		transform.position = tile.gameObject.transform.position;
		//Debug.Log(transform.position);
		UpdateGuard();
	}
	public List<Tile> GetGuardTargets(Vector3 dir) {

		dir.Normalize();

		Vector3 left = (new Vector3(-1,0,0)).normalized;
		Vector3 upLeft = (new Vector3(-1,0,1)).normalized;
		Vector3 up = (new Vector3(0,0,1)).normalized;
		Vector3 upRight = (new Vector3(1,0,1)).normalized;
		Vector3 right = (new Vector3(1,0,0)).normalized;
		Vector3 downRight = (new Vector3(1,0,-1)).normalized;
		Vector3 down = (new Vector3(0,0,-1)).normalized;
		Vector3 downLeft = (new Vector3(-1,0,-1)).normalized;

		// Default
		Pattern guard = guardPattern.up;

		// WHICH IS IT??
		if (dir == left) 
			guard = guardPattern.left;
		else if (dir == upLeft)
			guard = guardPattern.upLeft;
		else if (dir == up)
			guard = guardPattern.up;
		else if (dir == upRight)
			guard = guardPattern.upRight;
		else if (dir == right)
			guard = guardPattern.right;
		else if (dir == downRight)
			guard = guardPattern.downRight;
		else if (dir == down)
			guard = guardPattern.down;
		else if (dir == downLeft)
			guard = guardPattern.downLeft;

		Grid grid = tile.grid;
		List<Tile> targets = new List<Tile>();

		for (int x = 0; x < guard.sizeX; x++) {			
			for (int y = 0; y < guard.sizeY; y++) {

				if (guard.values[x+y*guard.sizeX] > 0) {

					int locX = tile.x + (x - guard.originX);
					int locY = tile.y - (y - guard.originY);

					if ( (locX > 0 && locX < grid.xSize) && (locY > 0 && locY < grid.ySize)) {

						//grid.grid[locX, locY].AddEffect(guardEffect);
						targets.Add(grid.grid[locX, locY]);

					}
				}
			}
		}
		return targets;
	}
	public void ClearGuardTargets() {
		guardEffect.ClearTargets();
	}

	public void ApplyGuard() {

		ClearGuardTargets();

		List<Tile> targets = GetGuardTargets(facing);

		foreach (Tile t in targets) {
			t.AddEffect(guardEffect);
			guardEffect.targets.Add(t);
		}
	}

}
