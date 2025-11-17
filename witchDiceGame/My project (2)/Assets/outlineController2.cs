using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class outlineController2 : MonoBehaviour
{
    public Color color2 = Color.white;

    [Range(0, 16)]
    public int outlineSize2 = 1;

    private SpriteRenderer spriteRenderer2;
    private Material material;
    [SerializeField]
    public int outlineWidth = 1;
    void OnEnable()
    {
        spriteRenderer2 = GetComponent<SpriteRenderer>();
        material = GetComponent<SpriteRenderer>().sharedMaterial;
        UpdateOutline2(true);
    }

    void OnDisable()
    {
        UpdateOutline2(false);
    }

    void Update()
    {
        UpdateOutline2(true);
    }

    void UpdateOutline2(bool outline)
    {
        material.SetInt("_Radius", outlineWidth);
        /*MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer2.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color2);
        mpb.SetFloat("_OutlineSize", outlineSize2);
        spriteRenderer2.SetPropertyBlock(mpb);
        */
    }
}