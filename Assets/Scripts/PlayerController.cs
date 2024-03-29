using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region vars
    //CONSTS
    private float meleeRadius = 2f;
    private float speedConst = 75f;
    private float fireDelay = 0.1f;
    private float sphereRadius = 0.02f;
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
    public AudioSource playerHurt;
    public AudioSource playerFire;
    public AudioSource shotgunFire;
    public AudioSource loseSound;
    public AudioSource hitscanSound;
    public AudioSource spinSpound;

    //Outside instanced
    public GameObject bulletPrefab;
    public GameObject hitscanPrefab;
    public Slider healthBar;
    public Slider chargeBar;
    public Image healthIcon;
    public Image chargeIcon;
    public Image damageIcon;

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

    public bool animating = false;
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
        spawnPoint.SpawnPlayer();
        VariableHolder.playerHealth -= missingHealth;
        Destroy(gameObject);
    }
    void AutoFire()
    {
        if (!VariableHolder.shotgun)
        {
            if (Time.time - lastFire >= fireDelay)
            {
                playerFire.Play();
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
        else
        {
            if (Time.time - lastFire >= .5f)
            {
                shotgunFire.Play();
                lastFire = Time.time;
                for (float angle = -90f * (1f / 4f); angle <= 90f * (1f / 4f); angle += 90f / 8f)
                {
                    GameObject newBullet = Instantiate(bulletPrefab, gunEmitter.position, playerModel.rotation);
                    newBullet.transform.Rotate(Vector3.up, angle);
                    BulletScript script = newBullet.GetComponent<BulletScript>();
                    newBullet.name = "PlayerShot";
                    script.impulse = .5f;
                    script.lifetime = 2f;
                    script.damage = bulletDamage;
                    script.bulletParent = gameObject;
                }

            }
        }
    }
    void ApplyPowerups()
    {
        if (healthPowerTimer >= 0)
        {
            healthPowerTimer -= Time.deltaTime;
            VariableHolder.playerHealth += Time.deltaTime * 4f;
            healthIcon.fillAmount = healthPowerTimer/10f;
        }
        if (chargePowerTimer >= 0)
        {
            chargePowerTimer -= Time.deltaTime;
            VariableHolder.playerCharge += Time.deltaTime * 4f;
            chargeIcon.fillAmount = chargePowerTimer / 10f;
        }
        if (damagePowerTimer >= 0)
        {
            damagePowerTimer -= Time.deltaTime;
            bulletDamage = baseBulletDamage * 2f;
            kickDamage = baseKickDamage * 5f;
            damageIcon.fillAmount = damagePowerTimer / 10f;
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
        if (railCD <= 0)
        {
            hitscanSound.Play();
            Ray hitscan = new Ray(playerModel.position, gunEmitter.position - playerModel.position);
            Debug.DrawRay(hitscan.origin, hitscan.direction * 100f, Color.red, 1f);

            foreach (RaycastHit hit in Physics.SphereCastAll(hitscan, sphereRadius * VariableHolder.playerCharge, rayLength))
            {
                if (hit.collider.CompareTag("Enemy")||hit.collider.CompareTag("Boss"))
                {
                    VariableHolder.playerScore += 10;
                    hit.collider.SendMessage("OnScannedHit", bulletDamage / 10f * VariableHolder.playerCharge);
                }
            }
            for (float i = 2f; i > .2f; i -= 1.8f)
            {
                GameObject newBullet = Instantiate(hitscanPrefab, gunEmitter.position, playerModel.rotation);
                ScanEffectScript script = newBullet.GetComponent<ScanEffectScript>();
                newBullet.name = "HitscanVisual";
                script.impulse = i;
                script.lifetime = 1f;
                script.charge = VariableHolder.playerCharge;
            }
            VariableHolder.playerCharge = 1;
            railCD = 1f;
        }
    }
    void OnAbilityUse()
    {
        if (kickCD <= 0)
        {
            spinSpound.Play();
            anim.SetTrigger("Spin"); //ngl wish i knew about async when i hacked this one up
            Collider[] RangeHits = Physics.OverlapSphere(playerModel.position, meleeRadius);
            Debug.Log(RangeHits.Length);
            foreach (Collider c in RangeHits)
            {
                if (c.CompareTag("Enemy"))
                {
                    c.SendMessage("OnMeleeHit", kickDamage);
                    VariableHolder.playerScore += 10;
                    c.attachedRigidbody.AddForce((c.transform.position - transform.position) * 7f, ForceMode.Impulse);
                }
                if (c.CompareTag("Shotgun"))
                {
                    VariableHolder.shotgun = true;
                }
            }
            kickCD = 1.5f;
        }
    }
    void OnGodToggle()
    {
        VariableHolder.godMode = !VariableHolder.godMode;
    }
    #endregion
    
    #region messages  
    void OnDamageTaken(float damage)
    {
        if (VariableHolder.godMode) {return;}
        VariableHolder.playerHealth -= damage;
        playerHurt.Play();
        if (VariableHolder.playerHealth <= 0.0f)
        {
            loseSound.Play();
            SceneManager.LoadScene("TitleScreen");
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
                    VariableHolder.playerScore += 20;
                    break;
                case PowerupScript.PowerUpType.Charge:
                    chargePowerTimer = 10f;
                    VariableHolder.playerScore += 20;
                    break;
                case PowerupScript.PowerUpType.Damage:
                    damagePowerTimer = 10f;
                    VariableHolder.playerScore += 20;
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
        VariableHolder.playerCharge += damage;
        VariableHolder.playerScore += damage;
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

        if (VariableHolder.playerHealth >= maxHealth) { VariableHolder.playerHealth = maxHealth;}
        if (VariableHolder.playerCharge >= 100f) { VariableHolder.playerCharge = 100f; chargeColour = Color.green;} else {chargeColour = Color.yellow;}
        if (VariableHolder.godMode) {healthColour = Color.cyan;} else{healthColour = Color.red;}

        healthBar.SendMessage("OnUpdateValue", VariableHolder.playerHealth);
        chargeBar.SendMessage("OnUpdateValue", VariableHolder.playerCharge);
        healthBar.SendMessage("OnSetColour", healthColour);
        chargeBar.SendMessage("OnSetColour", chargeColour);

    }
    #endregion
    }
