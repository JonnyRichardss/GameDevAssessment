using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region vars
    //CONSTS
    private float meleeRadius = 2f;
    private float speedConst = 75f;
    private float fireDelay = 0.1f;
    private float sphereRadius = 0.005f;
    private float rayLength = 100.0f;
    private float baseBulletDamage = 2;
    private float baseKickDamage = 1;
    private float maxHealth = 100;

    //Own components
    private Rigidbody rb;
    private Animator anim;
    private Camera mainCamera;
    private Transform lookTarget;
    private Transform playerModel;
    private Transform gunEmitter;

    //Outside instanced
    public GameObject bulletPrefab;
    public GameObject hitscanPrefab;
    public Slider healthBar;
    public Slider chargeBar;

    //Dynamic vars
    private float movementX, movementY;
    private float mouseX, mouseY;
    private float bulletDamage;
    private float kickDamage;
    private float healthPowerTimer=0;
    private float chargePowerTimer=0;
    private float damagePowerTimer=0;
    private float lastFire = 0;
    private float kickCD;
    private float railCD;
    private bool firing;
    private bool godMode = false;

    public bool animating = false;
    public float health=100;
    public float weaponCharge = 1;
    public int lastTrap;
    #endregion
    #region builtins
    void Start()
    {
        Physics.IgnoreLayerCollision(9, 9);
        Physics.IgnoreLayerCollision(8, 8);
        Physics.IgnoreLayerCollision(8, 9);
        Physics.IgnoreLayerCollision(8, 10);
        GetOwnComponents();
        mainCamera.transform.LookAt(rb.transform);
        bulletDamage = baseBulletDamage;
        kickDamage = baseKickDamage;
        UpdateBars();
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
        lookTarget.localPosition = mouse;
        }
        playerModel.LookAt(lookTarget);

        if (firing)
        {
            AutoFire();
           
        }

        UpdateBars();
        ApplyPowerups();
        kickCD -= Time.deltaTime;
        railCD -= Time.deltaTime;
        
    }
    #endregion

    #region methods
    void RespawnPlayer(float missingHealth)
    {
        PlayerSpawnPoint spawnPoint = GetComponentInParent<PlayerSpawnPoint>();
        spawnPoint.SpawnPlayer(health - missingHealth, weaponCharge);
        Destroy(gameObject);
    }
    void AutoFire()
    {
        if (Time.time - lastFire >= fireDelay)
        {
            lastFire = Time.time;
            GameObject newBullet = Instantiate(bulletPrefab, gunEmitter.position, playerModel.rotation);
            BulletScript script = newBullet.GetComponent<BulletScript>();
            newBullet.name = "PlayerShot";
            script.impulse = .5f;
            script.lifetime = 2f;
            script.damage = bulletDamage;
            script.bulletParent = gameObject;

        }
    }
    void ApplyPowerups()
    {
        if (healthPowerTimer >= 0)
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
    #endregion
    #region inputs
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
        if (railCD > 0)
        {
            Ray hitscan = new Ray(playerModel.position, gunEmitter.position - playerModel.position);
            Debug.DrawRay(hitscan.origin, hitscan.direction * 100f, Color.red, 1f);

            foreach (RaycastHit hit in Physics.SphereCastAll(hitscan, sphereRadius * weaponCharge, rayLength))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.SendMessage("OnScannedHit", bulletDamage / 10f * weaponCharge);
                }
            }
            for (float i = 2f; i > .2f; i -= 1.8f)
            {
                GameObject newBullet = Instantiate(hitscanPrefab, gunEmitter.position, playerModel.rotation);
                ScanEffectScript script = newBullet.GetComponent<ScanEffectScript>();
                newBullet.name = "HitscanVisual";
                script.impulse = i;
                script.lifetime = 1f;
                script.charge = weaponCharge;
            }
            weaponCharge = 1;
            railCD = 1f;
        }
    }
    void OnAbilityUse()
    {
        if (kickCD > 0)
        {
            anim.SetTrigger("Spin"); //ngl wish i knew about async when i hacked this one up
            Collider[] RangeHits = Physics.OverlapSphere(playerModel.position, meleeRadius);
            Debug.Log(RangeHits.Length);
            foreach (Collider c in RangeHits)
            {
                if (c.CompareTag("Enemy"))
                {
                    c.SendMessage("OnMeleeHit", kickDamage);
                    c.attachedRigidbody.AddForce((c.transform.position - transform.position) * 5f, ForceMode.Impulse);
                }
            }
            kickCD = 1.5f;
        }
    }
    void OnGodToggle()
    {
        godMode = !godMode;
    }
    #endregion
    
    #region messages  
    void OnDamageTaken(float damage)
    {
        if (godMode) {return;}
        health -= damage;
        if (health <= 0.0f)
        {
            RespawnPlayer(-100f);
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
       if (collision.collider.CompareTag("Deadly"))
        {
            RespawnPlayer(10f);
        }
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
    void OnDealtDamage(float damage)
    {
        weaponCharge += damage;
    }
    #endregion
    #region helpers
    void GetOwnComponents()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        mainCamera = GetComponentInChildren<Camera>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            switch (child.name)
            {
                case "LookTarget":
                    lookTarget = child;
                    break;
                case "PlayerModel":
                    playerModel = child;
                    gunEmitter = child.GetChild(0);
                    break;
                default:
                    break;
            }
        }
    }
    void UpdateBars()
    {
        Color healthColour;
        Color chargeColour;

        if (health >= maxHealth) {health = maxHealth;}
        if (weaponCharge >= 100f) {weaponCharge = 100f; chargeColour = Color.green;} else {chargeColour = Color.yellow;}
        if (godMode) {healthColour = Color.cyan;} else{healthColour = Color.red;}

        healthBar.SendMessage("OnUpdateValue", health);
        chargeBar.SendMessage("OnUpdateValue", weaponCharge);
        healthBar.SendMessage("OnSetColour", healthColour);
        chargeBar.SendMessage("OnSetColour", chargeColour);

    }
    #endregion
    }
