using UnityEngine;
using System.Collections;

// Lets you fade the renderers in this GO's children 
// by tweening the alpha of the material
public class Fade : MonoBehaviour 
{
    public float alphaLerpSpeed;

    private Material[] materials;
    private float targetAlpha = 1;

	// Use this for initialization
	void Start () 
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length];
        for(int i=0; i<renderers.Length; i++)
            materials[i] = renderers[i].material;
	}

    void Update()
    {
	    for(int i=0; i<materials.Length; i++)
        {
            Color c = materials[i].color;
            c.a = Mathf.Lerp(c.a, targetAlpha, alphaLerpSpeed);
            materials[i].color = c;
        }
	}

    public void FadeIn()
    {
        targetAlpha = 1;
    }

    public void FadeOut()
    {
        targetAlpha = 0;
    }
}
