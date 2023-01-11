using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;

public class HealthBarBehaviour : MonoBehaviour
{
    private Slider bar;
    public VisualEffect effect;
    public Sprite barNormal;
    public Sprite barGod;
    public Sprite iconNormal;
    public Sprite iconGod;
    private Image barImage;
    private Image icon;
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
    }
    public void OnSetColour(Color colour)
    {
        barImage.color = colour;
        icon.color = colour;
        if (colour == Color.cyan)
        {
            barImage.sprite = barGod;
            icon.sprite = iconGod;
            effect.enabled = true;
        }
        else
        {
            barImage.sprite = barNormal;
            icon.sprite = iconNormal;
            effect.enabled = false;
        }

    }
}
