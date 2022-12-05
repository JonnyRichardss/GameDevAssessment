using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX, movementY;
    private float mouseX, mouseY;
    private bool godMode = false;

    public float speedConst = 0;
    public Camera mainCamera;
    public GameObject lookTarget;
    public GameObject playerModel;
    public GameObject gunEmitter;
    public GameObject bulletPrefab;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
    void OnPrimaryFire()
    {
        GameObject newBullet = Instantiate(bulletPrefab,gunEmitter.transform.position,playerModel.transform.rotation);
        BulletScript script = newBullet.GetComponent<BulletScript>();
        newBullet.name = "PlayerShot";
        script.impulse = .5f;
        script.lifetime = 2;
    }
    void OnSecondaryFire()
    {
        Ray hitscan = new Ray(gunEmitter.transform.position,  gunEmitter.transform.position-lookTarget.transform.position);
        Debug.DrawRay(hitscan.origin, hitscan.direction * 10f,Color.red,0.5f);
        if(Physics.Raycast(hitscan, out RaycastHit hit, 10f))
        {
            if (hit.collider.CompareTag("Target"))
            {
                hit.collider.SendMessage("OnScannedHit");
            }
        }
    }
    void OnAbilityUse()
    {

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
}
