using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
    public float speed = 2f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(transform.forward * speed * Time.deltaTime);
	}
}
