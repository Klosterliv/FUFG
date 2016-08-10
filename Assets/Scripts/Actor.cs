using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour
{
    public float delay;
    public float actionDelay;
    public Tile position;

    public virtual void Act() {
        Debug.Log("Actor doing Actor things");
        delay = actionDelay;
    }
}
