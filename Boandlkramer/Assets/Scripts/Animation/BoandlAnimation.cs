using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoandlAnimation : MonoBehaviour {

    public Animator anim;
    NavMeshAgent nav;

	// Use this for initialization
	void Start () {
        nav = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        float move = nav.velocity.magnitude/nav.speed;
        anim.SetFloat("Speed", move);
	}

    public void Trigger(string trig)
    {
        anim.SetTrigger(trig);
    }
}
