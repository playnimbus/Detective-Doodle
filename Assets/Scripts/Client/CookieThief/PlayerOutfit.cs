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
        photonView.RPC("setShirt", PhotonTargets.All, (byte)Random.Range(0, 3));
        photonView.RPC("setHat", PhotonTargets.All, (byte)Random.Range(0, 3));
        photonView.RPC("setGlove", PhotonTargets.All, (byte)Random.Range(0, 3));
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
    void setHat(byte hat) 
    {
        currentHat = (HatColors)hat;
        switch (hat)
        {
            case (byte)HatColors.blue: hatIcon.sprite = blueHat; break;
            case (byte)HatColors.orange: hatIcon.sprite = orangeHat; break;
            case (byte)HatColors.red: hatIcon.sprite = redHat; break;
        }
    }
    [RPC]
    void setShirt(byte shirt)
    {
        currentShirt = (ShirtColors)shirt;
        switch (shirt)
        {
            case (byte)ShirtColors.blue: shirtIcon.sprite = blueShirt; break;
            case (byte)ShirtColors.green: shirtIcon.sprite = greenShirt; break;
            case (byte)ShirtColors.red: shirtIcon.sprite = redShirt; break;
        }
    }
    [RPC]
    void setGlove(byte glove) 
    {
        currentGlove = (GloveColors)glove;
        switch (glove)
        {
            case (byte)GloveColors.blue: gloveIcon.sprite = blueGlove; break;
            case (byte)GloveColors.green: gloveIcon.sprite = greenGlove; break;
            case (byte)GloveColors.red: gloveIcon.sprite = redGlove; break;
        }
    }
}
