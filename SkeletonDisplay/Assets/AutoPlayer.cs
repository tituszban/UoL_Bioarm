using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayer : MonoBehaviour {

    Player p;

    void Awake()
    {
        p = GetComponent<Player>();
    }

    public float time;
    public float speed = 0.1f;

    public bool on = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (on)
        {
            time = (time + Time.deltaTime * speed) % 1;
        }
        p.time = time;
    }
}
