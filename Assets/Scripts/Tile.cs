using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile {

	public Grid grid;

	public GameObject gameObject;
	public int x, y;

	public int height = 0;
	public int weight = 1;

	public bool occupied = false;
	public Unit occupier = null;

	// should not be serialized i suppose!
	public List<Effect> effects;

	public Tile(Grid grid, int x, int y) {
		this.grid = grid;
		this.x = x;
		this.y = y;
		effects = new List<Effect>();
	}
	public void AddEffect(Effect effect) {
		//if (!effects.Contains(effect)) {
			effects.Add(effect);
		Debug.DrawRay(gameObject.transform.position, Vector3.up*2, Color.green,10f);
		//}
	}
	public void RemoveEffect(Effect effect) {
		for (int i = 0; i < effects.Count; i++) {
			if (effects[i].Equals(effect)) effects.RemoveAt(i);
			Debug.DrawRay(gameObject.transform.position+Vector3.right*0.2f, Vector3.up*2, Color.red,10f);
		}
		//effects.Remove(effect);
		//effects.remo

	}

}
