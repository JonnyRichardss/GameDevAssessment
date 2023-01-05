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
    private float weaponCharge = 0;
    private bool godMode = false;
    
    public float speedConst = 0;
    public float fireDelay = 0;
    public Camera mainCamera;
    public GameObject lookTarget;
    public GameObject playerModel;
    public GameObject gunEmitter;
    public GameObject bulletPrefab;
    public TextMeshProUGUI basicInfoText;

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
        float sphereRadius = 1.0f;
        float rayLength = 100.0f;
        Ray hitscan = new Ray(gunEmitter.transform.position,  gunEmitter.transform.position-lookTarget.transform.position);
        Vector3[] rayOrigins = { hitscan.origin, hitscan.origin + transform.InverseTransformVector(Vector3.right) * sphereRadius, hitscan.origin + transform.InverseTransformVector(Vector3.left) * sphereRadius};

        foreach (Vector3 origin in rayOrigins)
        {
            Debug.DrawRay(origin, hitscan.direction * rayLength, Color.red, 0.5f);
        }

        if(Physics.SphereCast(hitscan,sphereRadius, out RaycastHit hit, rayLength))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.SendMessage("OnScannedHit",5f);
            }
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
    }
}
