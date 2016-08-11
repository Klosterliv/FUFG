using UnityEngine;
using System.Collections;

public class Effect : Actor
{
    float lifeTime;
    //float damage;
    public override void Act()
    {
        //ApplyDamage
        Debug.Log("Apply Damage Effect");
        lifeTime = lifeTime - delay; //update lifetime
        delay = actionDelay; //SetNewDelay
    }
}
