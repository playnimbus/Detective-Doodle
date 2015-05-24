using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerOutfit : Photon.MonoBehaviour{

    public Image shirtIcon;
    public Image hatIcon;
    public Image gloveIcon;

    public Sprite blueShirt;
    public Sprite greenShirt;
    public Sprite redShirt;

    public Sprite blueHat;
    public Sprite orangeHat;
    public Sprite redHat;

    public Sprite blueGlove;
    public Sprite greenGlove;
    public Sprite redGlove;

    public ShirtColors currentShirt;
    public HatColors currentHat;
    public GloveColors currentGlove;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Sets a players outfit to random colors
    /// </summary>
    public void setOutfit()
    {
        photonView.RPC("setShirt", PhotonTargets.All, Random.Range(0, 2));
        photonView.RPC("setHat", PhotonTargets.All, Random.Range(0, 2));
        photonView.RPC("setGlove", PhotonTargets.All, Random.Range(0, 2));
    }

    /// <summary>
    /// Sets Players outfit to specific colors
    /// </summary>
    public void setOutfit(ShirtColors shirt, HatColors hat, GloveColors glove)
    {
        photonView.RPC("setShirt", PhotonTargets.All, (byte)shirt);
        photonView.RPC("setHat", PhotonTargets.All, (byte)hat);
        photonView.RPC("setGlove", PhotonTargets.All, (byte)glove);
    }

    [RPC]
    void setHat(HatColors hat) 
    {
        currentHat = hat;
        switch (hat)
        {
            case HatColors.blue: hatIcon.sprite = blueHat; break;
            case HatColors.orange: hatIcon.sprite = orangeHat; break;
            case HatColors.red: hatIcon.sprite = redHat; break;
        }
    }
    [RPC]
    void setShirt(ShirtColors shirt)
    {
        currentShirt = shirt;
        switch (shirt)
        {
            case ShirtColors.blue: shirtIcon.sprite = blueShirt; break;
            case ShirtColors.green: shirtIcon.sprite = greenShirt; break;
            case ShirtColors.red: shirtIcon.sprite = redShirt; break;
        }
    }
    [RPC]
    void setGlove(GloveColors glove) 
    {
        currentGlove = glove;
        switch (glove)
        {
            case GloveColors.blue: gloveIcon.sprite = blueGlove; break;
            case GloveColors.green: gloveIcon.sprite = greenGlove; break;
            case GloveColors.red: gloveIcon.sprite = redGlove; break;
        }
    }
}
