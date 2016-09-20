using UnityEngine;
using System.Collections;

public class Action {

	/// <summary>
	/// type of move
	/// target
	/// ability
	/// user
	/// result dmg ?? result generic
	/// S	/// </summary>

	Tile targetTile;
	Unit targetUnit;
	Unit user;
	Type type;

	public enum Type {
		Rush,
		Move,
		Orient,
		//Strike,
		Ability,
		None,
	};

	public Action (Type type) {
		this.type = type;
	}

}
	

