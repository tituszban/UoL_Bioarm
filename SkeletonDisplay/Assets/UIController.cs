using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Player p;
    public AutoPlayer ap;

    public InputField loadField;
    public Slider timeSlider;

    public Image buttonImage;

    public Sprite[] onOff;

	// Use this for initialization
	void Start () {
        
	}
    
	
	// Update is called once per frame
	void Update () {
        timeSlider.interactable = !ap.on;
        if (ap.on)
        {
            timeSlider.value = ap.time;
        }
        else
        {
            ap.time = timeSlider.value;
        }

        buttonImage.sprite = onOff[(ap.on ? 0 : 1)];
	}

    public void Load()
    {
        p.Load(loadField.text + ".txt");
    }
    public void Step(int s)
    {
        if(!ap.on)
            timeSlider.value = (timeSlider.value + 0.01f * s) % 1;
    }
    public void Play()
    {
        ap.on = !ap.on;
    }
}
