using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float meleeRadius = 1.3f;

    private Rigidbody rb;
    private float movementX, movementY;
    private float mouseX, mouseY;
    private bool firing;
    private float lastFire = 0;
    private float weaponCharge = 1;
    private bool godMode = false;
    
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

    private float hitDebugCharge = 1;
    private Vector3 hitDebugPos;
    private Vector3 hitDebugRay;
    private float hitDebugTimer = 0;
    private bool hitDebug = false;
    public float health;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera.transform.LookAt(rb.transform);
    }
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        Vector3 mouse = new Vector3(mouseX, 0.0f, mouseY);

        rb.AddForce(movement * speedConst);

        lookTarget.transform.localPosition = mouse;
        playerModel.transform.LookAt(lookTarget.transform);
    }

    void Update()
    {
        if (firing)
        {
            AutoFire();
        }
        basicInfoText.text = string.Format("Health: {0} \nCharge: {1}", health, weaponCharge);
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
    void OnStickTurn(InputValue inputValue)
    {
        mouseX = inputValue.Get<Vector2>().x;
        mouseY = inputValue.Get<Vector2>().y;

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
        hitDebugPos = gunEmitter.transform.position;
        hitDebugRay = gunEmitter.transform.position - lookTarget.transform.position;
        Ray hitscan = new Ray(gunEmitter.transform.position,  gunEmitter.transform.position-lookTarget.transform.position);
        Debug.DrawRay(hitscan.origin, hitscan.direction*100f, Color.red, 1f);
        
        if (Physics.SphereCast(hitscan,sphereRadius * weaponCharge, out RaycastHit hit, rayLength))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.SendMessage("OnScannedHit",5f);
            }
        }
        weaponCharge = 1;
    }
    void AddCharge(float charge)
    { 
        weaponCharge += charge;
        if (weaponCharge >= 100)
        {
            weaponCharge = 100;
        }
    }
    void OnAbilityUse()
    {
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
    void OnGodToggle()
    {
        if (godMode)
        {
            godMode = false;
        }
        else 
        { 
            godMode = true;
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
