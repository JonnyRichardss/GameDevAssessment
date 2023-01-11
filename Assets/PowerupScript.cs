using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class PowerupScript : MonoBehaviour
{
    public enum PowerUpType
    {
        Health,
        Charge,
        Damage
    }
    private Vector3 basePos;
    public float periodMult = 5f;
    public float amplitudeMult = .25f;
    private VisualEffect vfx;
    private Transform childTransform;
    public PowerUpType type;
    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponentInChildren<VisualEffect>();
        childTransform = GetComponentsInChildren<Transform>()[1];
        Color colour;
        switch (type)
        {
            case PowerUpType.Health:
                colour = Color.green;
                break;
            case PowerUpType.Charge:
                colour = Color.yellow;
                break;
            case PowerUpType.Damage:
                colour = Color.red;
                break;
            default:
                colour = Color.white;
                Debug.Log("TYPE NOT SET");
                break;
        }
        vfx.SetVector4("Color", (Vector4)MakeHDRColour(colour, 6f));
    }

    // Update is called once per frame
    void Update()
    {
  
        childTransform.localPosition = new Vector3(0f,amplitudeMult*Mathf.Sin(Time.time*periodMult),0f);
    }
    Color MakeHDRColour(Color colour,float intensity)
    {
        float factor = Mathf.Pow(2f, intensity);
        return new Color(colour.r*factor, colour.g*factor, colour.b*factor);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
        
    }
}
