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
    public float damage;
    public GameObject targetObject;
    public Transform targetTransform;
    public EnemyType attackType;

    private float attackCD = 0;
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
        nav.SetDestination(targetTransform.position);
        TryAttack();
    }
    void FixedUpdate()
    {

    }
    void Move()
    {
        
       
    }
    bool TryAttack()
    {
        if (attackCD <= 0)
        {
            switch (attackType)
            {
                case EnemyType.Melee:
                    if ((transform.position - targetTransform.position).magnitude < 1.2)
                    {
                        targetObject.SendMessage("OnDamageTaken", damage);
                        attackCD = 1f;
                        return (true);
                    }
                    break;
                case EnemyType.Ranged:
                    break;
                case EnemyType.Boss:
                    break;
                    //actually might make a sep boss script as they will prob have mult attks
            }
            return (false);
        }
        else
        {
            attackCD -= Time.deltaTime;
            return (false);
        }
    }
    void UpdateTarget(GameObject newTarget)
    {
        targetObject = newTarget;
        targetTransform = newTarget.transform;
    }
    void OnDamageTaken(float damage)
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
        OnDamageTaken(damage);
    }
    void OnProjHit(float damage)
    {
        Debug.Log("PROJ");
        OnDamageTaken(damage);
    }
    void OnMeleeHit(float damage)
    {
        Debug.Log("Ow");
        OnDamageTaken(damage);
    }
    #endregion
}

