using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class ChargeBarBehaviour : MonoBehaviour
{
    private Slider bar;
    public VisualEffect effect;
    private Image barImage;
    private Image icon;
    private Color targetColour;
    private void Awake()
    {
        bar = GetComponent<Slider>();
        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if (image.name == "Bar")
            {
                barImage = image;
            }
            if (image.name == "Icon")
            {
                icon = image;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

      
    }
    public void OnUpdateValue(float value)
    {
        bar.value = value;
        barImage.color = Color.Lerp(Color.black, targetColour, bar.normalizedValue);
        effect.enabled = value == 100f;

    }
    public void OnSetColour(Color colour)
    {
        targetColour = colour;
        icon.color = colour;
    }
}
