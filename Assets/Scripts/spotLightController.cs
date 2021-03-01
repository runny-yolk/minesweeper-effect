using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spotLightController : MonoBehaviour {
	public float speed = 20;

	void Start(){
		speed += (Random.value - 0.5f);
        transform.RotateAround(Vector3.zero, transform.right, 360*Random.value);
	}
    void Update() {
        transform.RotateAround(Vector3.zero, transform.right, speed*Time.deltaTime);
    }
}