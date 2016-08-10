using UnityEngine;
using System.Collections;

public class Actor
{
    float delay=0;

    public virtual void Act() {
        
    }
}  

public class Effect : Actor
{
    public override void Act()
    {
        
    }
}
public class Character : Actor
{

    public override void Act()
    {
        base.Act();
    }
}