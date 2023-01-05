using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Ranged,
        Boss
    }

    public float health;
    public GameObject targetObject;
    public Transform targetTransform;
    public EnemyType attackType;


    private NavMeshAgent nav;
    // Start is called before the first frame update
    void Start()
    {
        health = 10f;
        nav = GetComponent<NavMeshAgent>();
        UpdateTarget(targetObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        nav.destination = targetTransform.position;
    }
    void Move()
    {
        
       
    }
    void Attack()
    {
        switch (attackType)
        {
            case EnemyType.Melee:
                return;
            case EnemyType.Ranged:
                return;
            case EnemyType.Boss:
                return;
                //actually might make a sep boss script as they will prob have mult attks
        }
    }
    void UpdateTarget(GameObject newTarget)
    {
        targetObject = newTarget;
        targetTransform = newTarget.transform;
    }
    void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
    #region
    //messages
    void OnScannedHit(float damage)
    {
        Debug.Log("SCNA");
        TakeDamage(damage);
    }
    void OnProjHit(float damage)
    {
        Debug.Log("PROJ");
        TakeDamage(damage);
    }
    void OnMeleeHit(float damage)
    {
        Debug.Log("Ow");
        TakeDamage(damage);
    }
    #endregion
}

