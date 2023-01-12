using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    public float impulse;
    public float lifetime;
    public float damage;
    private Rigidbody rb;
    public GameObject bulletParent;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(impulse * transform.forward, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.SendMessage("OnDamageTaken", damage);
            Destroy(gameObject);
        }

    }
}
