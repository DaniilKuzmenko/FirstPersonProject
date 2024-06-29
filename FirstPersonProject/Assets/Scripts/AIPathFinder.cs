using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathFinder : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed, turnSpeed;
    private int currentPoint;

    [SerializeField]
    private Transform[] patrolPoints;
    private Collider[] destCollider;
    [SerializeField]
    private Rigidbody rb;
    private Transform targetT;
    [SerializeField]
    private Transform entityT;

    private bool isAttack = false;

    void Start()
    {
        InvokeRepeating("FindRepeating", 0f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttack)
        {
            Move(targetT);
        }
        else
        {
            Move(patrolPoints[currentPoint]);
        }
    }

    private void Move(Transform target)
    {
        Vector3 targetDir = target.position - entityT.position;
        targetDir = new Vector3(targetDir.x, 0f, targetDir.z);
        float singleStep = turnSpeed * Time.deltaTime;
        Vector3 look = Vector3.RotateTowards(entityT.forward, targetDir, singleStep, 0f);
        entityT.rotation = Quaternion.LookRotation(look);

        entityT.Translate(Vector3.forward * moveSpeed);
    }

    private void FindTarget()
    {
        if (Vector3.Distance(entityT.position, targetT.position) < 10)
        {
            isAttack = true;
        }
        else
        {
            Vector3 entityPosNoY = new Vector3(entityT.position.x, 0f, entityT.position.z);
            Vector3 patrolPointPosNoY = new Vector3(patrolPoints[currentPoint].position.x, 0f, patrolPoints[currentPoint].position.z);
            isAttack = false;
            if (Vector3.Distance(entityT.position, patrolPointPosNoY) < 2f)
            {
                currentPoint++;
                if (currentPoint >= patrolPoints.Length)
                {
                    currentPoint = 0;
                }
            }
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector3(0f, 6f);
    }

    public void Explode()
    {
        destCollider = Physics.OverlapSphere(entityT.position, 2f);
        foreach (Collider col in destCollider)
        {
            if (!col.CompareTag("Player"))
            {
                Destroy(col.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
