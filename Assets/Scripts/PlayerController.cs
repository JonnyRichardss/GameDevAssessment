using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    public Camera mainCamera;
    public GameObject lookTarget;
    public GameObject playerModel;
    public GameObject gunEmitter;
    public GameObject bulletPrefab;
    public TextMeshProUGUI basicInfoText;
    public Slider healthBar;
    public Slider chargeBar;
    public float maxHealth;

    private float hitDebugCharge = 1;
    private Vector3 hitDebugPos;
    private Vector3 hitDebugRay;
    private float hitDebugTimer = 0;
    private bool hitDebug = false;
    
   
    void Start()
    {
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
        basicInfoText.text = string.Format("Health: {0} \nCharge: {1}", health, weaponCharge);
        healthBar.SendMessage("OnUpdateValue",health);
        chargeBar.SendMessage("OnUpdateValue", weaponCharge);
        if (hitDebugTimer <= 0)
        {
            hitDebug = false;
        }
        else
        {
            hitDebugTimer -= Time.deltaTime;
        }

        
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
            script.damage = 1f;
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
       
        //add effect at some point
        hitDebug = true;
        hitDebugTimer = 1;
        hitDebugCharge = weaponCharge;
        Ray hitscan = new Ray(playerModel.transform.position,  gunEmitter.transform.position-playerModel.transform.position);
        hitDebugPos = hitscan.origin;
        hitDebugRay = hitscan.direction;
        Debug.DrawRay(hitscan.origin, hitscan.direction*100f, Color.red, 1f);
        
        if (Physics.SphereCast(hitscan,sphereRadius * weaponCharge, out RaycastHit hit, rayLength))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.SendMessage("OnScannedHit", .2f * weaponCharge);
            }
        }
        weaponCharge = 1;
        chargeBar.SendMessage("OnSetColour", Color.yellow);
    }
    void AddCharge(float charge)
    { 
        weaponCharge += charge;
        if (weaponCharge >= 100)
        {
            weaponCharge = 100;
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
                c.SendMessage("OnMeleeHit",10f);
                AddCharge(10f);
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerModel.transform.position, meleeRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lookTarget.transform.position, 0.1f);
        if (hitDebug)
        {
            for(int i = 0; i < 40; i+=4)
            {
                Gizmos.DrawWireSphere(hitDebugPos + hitDebugRay * i, sphereRadius*hitDebugCharge);
            }
        }
    }
    void OnDealtDamage(float damage)
    {
        AddCharge(damage);
    }
}
