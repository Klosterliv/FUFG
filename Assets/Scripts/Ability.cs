using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ability {

	// UI data
	public Texture2D icon;
	public string name;

	// // // //
	public float strength;
	public AbilityType abilityType;

	public float range = 5;
	public float delay = 1;

	/*
	 * viable target tags ..?
	 * spawned effects
	 * 
	 * 
	 * */

	public enum AbilityType {
		TargetDamage,
		NoTarget,
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {		

	}
	public virtual void Use (Unit user, Unit target) {
		target.Strike(user.tile, 1, 1);
		user.delay+=delay;
		
	}
	public virtual void Use () {
		
	}

	public bool IsValidTarget(Unit user, Unit target) {
		if (user != target)
			return true;
		else return false;
	}
	public bool IsValidTarget(Unit user, Tile target) {
		return true;
	}
}
