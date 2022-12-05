using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX, movementY;
    private Vector2 mousePosition;

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
        rb.AddForce(movement * speedConst);
    }

    void Update()
    {
        //Debug.Log(mousePosition);
        //RotationRayCast();
        RotatePlayer();
    }
    void RotationRayCast()
    {
        Ray ray = new Ray(playerModel.transform.position, lookTarget.transform.position-playerModel.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * 1000);
    }
    void RotatePlayer()
    {
        lookTarget.transform.localPosition = new Vector3(mousePosition.normalized.x,0.0f,mousePosition.normalized.y);
        playerModel.transform.LookAt(lookTarget.transform);
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }
    void OnMousePos(InputValue mousePos)
    {
        mousePosition = mousePos.Get<Vector2>();
        mousePosition -= new Vector2(Screen.width / 2, Screen.height / 2);
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
        Ray hitscan = new Ray(gunEmitter.transform.position,  gunEmitter.transform.position-lookTarget.transform.position );
        Debug.DrawRay(hitscan.origin, hitscan.direction * 1000,Color.red,0.5f);
    }
}
