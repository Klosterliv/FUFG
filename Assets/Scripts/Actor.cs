using UnityEngine;
using System.Collections;

public class Actor
{
    float delay=0;
    Tile position;
    public virtual void Act() {
    }
}  

public class Effect : Actor
{
    float lifeTime;
    public override void Act()
    {
        
    }
}
public class Unit : Actor
{
    int moveSpeed;
    int minorActions;
    int majorActions;
    public override void Act()
    {
        //base.Act();

    }
}