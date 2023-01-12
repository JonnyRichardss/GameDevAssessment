using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public enum EnemyType
    {
        Melee,
        Ranged
    }

    public int lastTrap;
    public float health;
    public float damage;
    public GameObject targetObject;
    public Transform targetTransform;
    public EnemyType attackType;
    public GameObject powerupPrefab;
    public GameObject bulletPrefab;


    private AudioSource soundPlayer;

    private float attackCD = 0;
    private NavMeshAgent nav;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreLayerCollision(6, 9);
        Physics.IgnoreLayerCollision(6, 7);
        soundPlayer = GetComponent<AudioSource>();
        health = 10f;
        nav = GetComponent<NavMeshAgent>();
        UpdateTarget(targetObject);
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(targetTransform.position);
        if (attackType == EnemyType.Ranged)
        {
            nav.stoppingDistance = 7f;
        }
        TryAttack();
    }
    void SpawnPowerup()
    {
         if( Random.value <= 0.1f)
        {
            float rand1 = Random.Range(.1f,.2f);
            float rand2 = Random.Range(.1f,.2f);
            if (Random.value > 0.5) { rand1 = 0 - rand1; }
            if (Random.value > 0.5) { rand2 = 0 - rand1; }
            GameObject powerup = Instantiate(powerupPrefab,transform.position+(new Vector3(0f,1f,0f)),transform.rotation);
            PowerupScript script = powerup.GetComponent<PowerupScript>();
            Rigidbody rb = powerup.GetComponent<Rigidbody>();
            Vector3 escapeVector = Vector3.Lerp(Vector3.up, Vector3.right, rand1);
            rb.AddForce(Vector3.Lerp(escapeVector,Vector3.forward,rand2)*10f,ForceMode.Impulse);
            PowerupScript.PowerUpType type;
            switch (Mathf.FloorToInt(Random.value*3))
            {
                case 0:
                    type = PowerupScript.PowerUpType.Health;
                    break;
                case 1:
                    type = PowerupScript.PowerUpType.Charge;
                    break;
                case 2:
                    type = PowerupScript.PowerUpType.Damage;
                    break;
                default:
                    type = PowerupScript.PowerUpType.Health;
                    Debug.Log("Rounding problem with powerup spawn");
                    break;

            }
            script.type = type;
        }

    }
    bool TryAttack()
    {
        if (attackCD <= 0f)
        {
            anim.SetTrigger("Attack");
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
                    if ((transform.position - targetTransform.position).magnitude < 8f)
                    {
                        FireBullet();
                        attackCD = 1f;
                        return (true);
                    }
                    break;
            }
            return (false);
        }
        else
        {
            attackCD -= Time.deltaTime;
            return (false);
        }
    }
    void FireBullet()
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        EnemyBulletScript script = newBullet.GetComponent<EnemyBulletScript>();
        newBullet.name = "EnemyShot";
        script.impulse = .1f;
        script.lifetime = 4f;
        script.damage = 1f;
        script.bulletParent = gameObject;
    }
    void UpdateTarget(GameObject newTarget)
    {
        targetObject = newTarget;
        targetTransform = newTarget.transform;
    }

    #region Messages
    void OnDamageTaken(float damage)
    {
        health -= damage;
        soundPlayer.Play();
        if (health <= 0.0f)
        {
            SpawnPowerup();
            Destroy(gameObject);
        }
    }
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
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            if (lastTrap != other.GetComponentInParent<SpikeTrapScript>().trapCounter && other.GetComponentInParent<SpikeTrapScript>().isDangerous)
            {
                lastTrap = other.GetComponentInParent<SpikeTrapScript>().trapCounter;
                OnDamageTaken(5f);
            }
        }
    }
    #endregion
}

