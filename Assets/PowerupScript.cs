using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class PowerupScript : MonoBehaviour
{
    private Vector3 basePos;
    public float periodMult = 5f;
    public float amplitudeMult = .5f;
    private VisualEffect vfx;
    // Start is called before the first frame update
    void Start()
    {
        basePos = transform.position;
        vfx = GetComponent<VisualEffect>();
        vfx.SetVector4("Color", (Vector4)MakeHDRColour(Color.green, 6f));
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = basePos+new Vector3(0f,amplitudeMult*Mathf.Sin(Time.time*periodMult),0f);
    }
    Color MakeHDRColour(Color colour,float intensity)
    {
        float factor = Mathf.Pow(2f, intensity);
        return new Color(colour.r*factor, colour.g*factor, colour.b*factor);
    }
}
