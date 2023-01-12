using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapScript : MonoBehaviour
{
    private float triggerTimer;
    private MeshRenderer planeMesh;
    private Transform spikes;
    private Color safeColour = Color.green;
    private Color dangerColour = Color.red;
    private bool isDangerous;
    // Start is called before the first frame update
    void Start()
    {
        planeMesh = GetComponent<MeshRenderer>();
        spikes = gameObject.transform.GetChild(0);
        spikes.localPosition = new Vector3(0f,-1f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerTimer <= 0)
        {
            StartCoroutine(TriggerSpikes());
        }
        else
        {
            triggerTimer -= Time.deltaTime;
        }
        
    }
    IEnumerator TriggerSpikes()
    {
        //flash spikes
        bool dangerColour = false;
        for (int i = 0; i < 7; i++)
        {
            if (dangerColour) 
            {
                planeMesh.material.color = safeColour;
                dangerColour = false;
            }
            else
            {
                planeMesh.material.color = this.dangerColour;
                dangerColour = true;
            }
            yield return new WaitForSeconds(.2f);
        }
        //trigger spikes
        isDangerous = true;
        for (int i = 0; i < 10; i++)
        {
            spikes.localPosition += Vector3.up * 0.2f;
            yield return new WaitForSeconds(.05f);
        }

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < 40; i++)
        {
            spikes.localPosition += Vector3.up * -0.05f;
            yield return new WaitForSeconds(.1f);
        }
        planeMesh.material.color = safeColour;
        isDangerous = false;
        triggerTimer = Random.Range(2, 10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDangerous)
        {
            if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Enemy"))
            {
                collision.collider.SendMessage("OnDamageTaken", 5f);
            }
        }
    }
}
