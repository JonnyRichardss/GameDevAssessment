using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private Transform modelTransform;
    private Rigidbody rb;
    private float movementX, movementY;
    public float speedConst = 0;
    private Vector2 mousePosition;
    public Camera mainCamera;
    public GameObject lookTarget;
    public GameObject playerModel;
    public GameObject gunEmitter;
    public GameObject bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        modelTransform = playerModel.GetComponent<Transform>();
        //mainCamera = GetComponentInChildren<Camera>();
    }
    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speedConst);
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(mousePosition);
        RotationRayCast();
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
        modelTransform.LookAt(lookTarget.transform);
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
        GameObject newBullet = Instantiate(bulletPrefab,gunEmitter.transform.position,modelTransform.rotation);
        BulletScript script = newBullet.GetComponent<BulletScript>();
        newBullet.name = "PlayerShot";
        script.speed = 50;
        script.lifetime = 2;
    }
}
