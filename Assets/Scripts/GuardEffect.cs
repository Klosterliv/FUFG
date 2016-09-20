using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardEffect : Effect
{
	public List<Tile> targets;
	public Unit parent;
	public float strength;

	public GuardEffect (Unit parent) {
		this.parent = parent;
		Initialize();
	}
	public GuardEffect () {
		Initialize();
	}

	public override void Act ()
	{
		base.Act ();
	}

	public override void Activate ()
	{
		Debug.Log("guard triggered");
	}
	public void ClearTargets() {
		foreach (Tile t in targets)
			t.RemoveEffect(this);
	}
	public override void Initialize ()
	{
		timeStep = false;
		targets = new List<Tile>();
	}
}
