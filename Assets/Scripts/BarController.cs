using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BarController : MonoBehaviour {
	public TMPro.TMP_Text text;

	static (float, string)[] txtThresh = {
		(0.0022f, "literally impossible to fail"),
		(0.05f, "ez pz"),
		(0.1f, "normal"),
		(0.2f, "hard-ish"),
		(0.3f, "hard"),
		(0.35f, "harder"),
		(0.4f, "very hard"),
		(0.5f, "near impossible"),
		(0.6f, "dude why"),
		(0.65f, "stop"),
		(0.675f, "STOP"),
		(0.7f, "BRO"),
		(0.75f, "BROOO"),
		(0.76f, "BROOOOO"),
		(0.77f, "BROOOOOOO"),
		(0.78f, "BROOOOOOOOO"),
		(0.79f, "BROOOOOOOOOOO"),
		(0.875f, "..."),
		(0.95f, "...fine."),
		(0.9988f, "actually near impossible"),
		(1, "literally impossible to win"),
	};

	void Start(){
		UpdateVisual();
	}

    void Update() {
		if(Input.GetMouseButton(0)){
			var didhit = GetComponent<BoxCollider>().Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100);
			if(didhit){
				var sx = transform.localScale.x;
				var hx = hit.point.x + sx/2;
				GameManager.difficulty = (hx/sx);
				UpdateVisual();
			}
		}

		if(Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene("SampleScene");
    }

	void UpdateVisual(){
		var diff = GameManager.difficulty;
		GetComponent<MeshRenderer>().material.SetFloat("_Threshold", diff-0.5f);
		var display = "";
		foreach(var (thresh, str) in txtThresh){
			if(diff < thresh) {
				display = str;
				break;
			}
		}
		text.text = "Difficulty rating: "+display;
	}
}
