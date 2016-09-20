using UnityEngine;
using System.Collections;

public class ActionButton : MonoBehaviour {

	public Unit user;
	public Ability ability;
	bool activated = false;

	public void Activate () {
		switch (ability.abilityType) {
		case Ability.AbilityType.NoTarget:
			ability.Use();
			break;
		default:
			MouseInput.instance.ActivateAbility(this);
			break;
		}
		
	}
	public void Use (Unit target) {
		ability.Use(user,target);
	}

}
