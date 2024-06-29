using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touch : MonoBehaviour
{
    [SerializeField]
    private AIPathFinder pathfinder;

    private void OnTriggerEnter(Collider col)
    {
        switch (col.tag)
        {
            case "Player":
                pathfinder.Explode();
                break;
            case "Block":
                pathfinder.Jump();
                break;    
        }
    }
}
