using UnityEngine;
using System.Collections;

public class Effect : Actor
{
	int priority = 0;
    float lifeTime;

	public bool activateWhenSteppedOn = false;

    //float damage;
    public override void Act()
    {
        //ApplyDamage
        Debug.Log("Apply Damage Effect");
        lifeTime = lifeTime - delay; //update lifetime
        delay = actionDelay; //SetNewDelay
		if (delay <= 0) {delay = 9999; Debug.LogError("delay loop exception; disqualified :"+this.name);}
		TurnHandler.instance.TimeStep();
    }

	public virtual void Activate() {
				
	}
}
