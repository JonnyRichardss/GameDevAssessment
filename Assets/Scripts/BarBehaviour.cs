using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class BarBehaviour : MonoBehaviour
{
    private Slider bar;
    public VisualEffect effect;
    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<Slider>();
    }
    public void OnUpdateValue(float value)
    {
        bar.value = value;
    }
    public void OnSetColour(Color colour)
    {
        foreach (Image child in GetComponentsInChildren<Image>())
        {
            if (child.name != "Border")
            {
                child.color = colour;
            }
        }
        if (colour == Color.green || colour == Color.cyan)
        {
            effect.enabled = true;
        }
        else
        {
            effect.enabled = false;
        }
    }
}
