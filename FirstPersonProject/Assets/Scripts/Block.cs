using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int health {get; set; }
    [SerializeField]
    private BlockTypes type;
    

    public void DestroyBehavior() 
    {
        GameObject miniBlock = Resources.Load<GameObject>("mini" + type.ToString());
        Instantiate(miniBlock, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    void Start()
    {
        health = (int)type;
    }

    
    void Update()
    {
        
    }
}
