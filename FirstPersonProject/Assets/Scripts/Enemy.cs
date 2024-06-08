using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int health = 200;

    public void TakeDamage(int value)
    {
        health -= value;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
   
}
