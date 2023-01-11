using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float meleeRadius = 1.3f;

    private Rigidbody rb;
    private Animator anim;
    private float movementX, movementY;
    private float mouseX, mouseY;
    private bool firing;
    private float lastFire = 0;
    private float weaponCharge = 1;
    private bool godMode = false;
    private float health;

    public bool animating;
    public float speedConst = 0;
    public float fireDelay = 0;
    private float sphereRadius = 0.005f;
    public float rayLength = 100.0f;
    public float baseBulletDamage;
    public float baseKickDamage;
    public Camera mainCamera;
    public GameObject lookTarget;
    public GameObject playerModel;
    public GameObject gunEmitter;
    public GameObject bulletPrefab;
    public GameObject hitscanPrefab;
    public Slider healthBar;
    public Slider chargeBar;
    public float maxHealth;

    private float healthPowerTimer;
    private float chargePowerTimer;
    private float damagePowerTimer;
    private float bulletDamage;
    private float kickDamage;

    public float hitspeed;
    void Start()
    {
        bulletDamage = baseBulletDamage;
        kickDamage = baseKickDamage;
        healthBar.SendMessage("OnSetColour", Color.red);
        chargeBar.SendMessage("OnSetColour", Color.yellow);
        rb = GetComponent<Rigidbody>();
        mainCamera.transform.LookAt(rb.transform);
        anim = GetComponent<Animator>();
        health = maxHealth;
    }
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);


        rb.AddForce(movement * speedConst);

        
        
    }

    void Update()
    {
        if (!animating)
        {
        Vector3 mouse = new Vector3(mouseX, 0.0f, mouseY);
        lookTarget.transform.localPosition = mouse;
        }
        playerModel.transform.LookAt(lookTarget.transform);

        if (firing)
        {
            AutoFire();
           
        }

        healthBar.SendMessage("OnUpdateValue",health);
        chargeBar.SendMessage("OnUpdateValue", weaponCharge);
        ApplyPowerups();

        
    }
    void AutoFire()
    {
        if (Time.time - lastFire >= fireDelay)
        {
            lastFire = Time.time;
            GameObject newBullet = Instantiate(bulletPrefab, gunEmitter.transform.position, playerModel.transform.rotation);
            BulletScript script = newBullet.GetComponent<BulletScript>();
            newBullet.name = "PlayerShot";
            script.impulse = .5f;
            script.lifetime = 2f;
            script.damage = bulletDamage;
            script.bulletParent = gameObject;
            
        }
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
    void OnMousePos(InputValue mouseIn)
    {
        Vector2 mousePos = mouseIn.Get<Vector2>() - new Vector2(Screen.width / 2, Screen.height / 2);
        mousePos = mousePos.normalized;
        mouseX = mousePos.x;
        mouseY = mousePos.y;
    }

    void OnPrimaryFire(InputValue value)
    {
        firing = value.isPressed;
    }
    void OnSecondaryFire()
    {
        Ray hitscan = new Ray(playerModel.transform.position,  gunEmitter.transform.position-playerModel.transform.position);
        Debug.DrawRay(hitscan.origin, hitscan.direction*100f, Color.red, 1f);
        
        if (Physics.SphereCast(hitscan,sphereRadius * weaponCharge, out RaycastHit hit, rayLength))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.SendMessage("OnScannedHit", bulletDamage/10f * weaponCharge);
            }
        }
        for (float i = 2f; i > .2f; i -= 1.8f)
        {
            GameObject newBullet = Instantiate(hitscanPrefab, gunEmitter.transform.position, playerModel.transform.rotation);
            ScanEffectScript script = newBullet.GetComponent<ScanEffectScript>();
            newBullet.name = "HitscanVisual";
            script.impulse = i;
            script.lifetime = 1f;
            script.charge = weaponCharge;
        }
        weaponCharge = 1;
        chargeBar.SendMessage("OnSetColour", Color.yellow);
        
    }
    void ClampValues()
    {
        if (health >= maxHealth) {health = maxHealth;}
        if (weaponCharge >= 100f) {weaponCharge = 100f;}
    }
    void AddCharge(float charge)
    { 
        weaponCharge += charge;
        if (weaponCharge >= 100f)
        {
            weaponCharge = 100f;
            chargeBar.SendMessage("OnSetColour", Color.green);
        }
    }
    void OnAbilityUse()
    {
        anim.SetTrigger("Spin");
        Collider[] RangeHits = Physics.OverlapSphere(playerModel.transform.position, meleeRadius);
        Debug.Log(RangeHits.Length);
       foreach (Collider c in RangeHits )
        {
            if (c.CompareTag("Enemy"))
            {
                c.SendMessage("OnMeleeHit",kickDamage);
                AddCharge(kickDamage);
                //ADD KB YOU BUFFON
            }
        }
    }
    void OnDamageTaken(float damage)
    {
        if (godMode) {return;}
        health -= damage;
        if (health <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
    void OnGodToggle()
    {
        if (godMode)
        {
            godMode = false;
            healthBar.SendMessage("OnSetColour",Color.red);
        }
        else 
        {
            godMode = true;
            healthBar.SendMessage("OnSetColour", Color.cyan);

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Powerup"))
        {
            PowerupScript script = collision.collider.GetComponent<PowerupScript>();
            switch (script.type)
            {
                case PowerupScript.PowerUpType.Health:
                    healthPowerTimer = 10f;
                    break;
                case PowerupScript.PowerUpType.Charge:
                    chargePowerTimer = 10f;
                    break;
                case PowerupScript.PowerUpType.Damage:
                    damagePowerTimer = 10f;
                    break;
            }
        }
    }
    private void ApplyPowerups()
    {
        Debug.Log(healthPowerTimer + ","+chargePowerTimer + "," + damagePowerTimer);
        if(healthPowerTimer >= 0)
        {
            healthPowerTimer -= Time.deltaTime;
            health += Time.deltaTime * 4f;
        }
        if (chargePowerTimer > 0)
        {
            chargePowerTimer -= Time.deltaTime;
            weaponCharge += Time.deltaTime * 4f;
        }
        if (damagePowerTimer >= 0)
        {
            damagePowerTimer -= Time.deltaTime;
            bulletDamage = baseBulletDamage * 2f;
            kickDamage = baseKickDamage * 5f;
        }
        else
        {
            bulletDamage = baseBulletDamage;
            kickDamage = baseKickDamage;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerModel.transform.position, meleeRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lookTarget.transform.position, 0.1f);
    }
    void OnDealtDamage(float damage)
    {
        AddCharge(damage);
    }
}
