using UnityEngine;
using System.Collections;

[System.Serializable]
public class Ability {

	public Texture2D icon;
	public string name;

	public float strength;

	public enum AbilityType {
		TargetDamage,
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {		
	
	}
	public virtual void Use (Tile target) {
		
	}
}
