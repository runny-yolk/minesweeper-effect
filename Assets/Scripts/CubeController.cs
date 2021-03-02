using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeController : MonoBehaviour {
	public Material standard;
	public Material flag;
	public Material inactive;
	public Material mine;
	public TMP_Text textComp;
	public Gradient textColour;
	public GameObject flagLight;
	public GameObject revealLight;
	public GameObject quad;

	public Vector2Int index;
	public GameManager gm;
	public bool isMine;
	public int nearbyMines;
	public bool isRevealed;
	public bool isFlagged;

	MeshRenderer mr => GetComponent<MeshRenderer>();
	new public AudioSource audio => GetComponent<AudioSource>();

	void Start(){
		textComp.text = nearbyMines.ToString();
		// divided by 8, because 8 is the max number of possible nearby mines
		// and is a float, otherwise it'll be done as an integer calculation, and the necessary float data will be lost
		textComp.color = textColour.Evaluate(nearbyMines/8f);
	}
	
	public void ParticleBurst(){
		revealLight.GetComponent<Animation>().Play();
		GetComponent<ParticleSystem>().Emit(7);
	}

	public void Reveal(int delay = 0){
		float d = delay;

		var t = d * 0.125f;
		if(delay > 15) t = d * (0.125f/2);
		if(delay > 36) t = d * (0.125f/3);
		if(delay > 54) t = d * (0.125f/4);

		if(isRevealed) {
			if(nearbyMines > 0) quad.SetActive(!quad.activeSelf);
			return;
		}

		if(isFlagged) Flag();

		if(isMine) {
			mr.sharedMaterial = mine;
			var failsfx = Resources.LoadAll<AudioClip>("failSFX");
			flagLight.SetActive(true);
			flagLight.GetComponent<Light>().color = new Color(1, 0.255f, 0);
			audio.pitch += (Random.value*0.5f) - 0.25f;
			audio.PlayOneShot(failsfx[Mathf.FloorToInt(failsfx.Length*Random.value)]);
		}
		else {
			isRevealed = true;
			gm.RevealedUp();
			if(nearbyMines == 0) {
				StartCoroutine(gm.Delay(t, () => {
					ParticleBurst();
					if(delay % 3 == 0) PlayNote();
					mr.sharedMaterial = inactive;
				} ));
				gm.cubes.ForEachNeighbours(index.x, index.y, (v, cc) => {if(!cc.isRevealed) {
					delay++;
					cc.Reveal(delay);
				}});
			}
			else {
				StartCoroutine(gm.Delay(t, () => {
					ParticleBurst();
					if(delay % 3 == 0) PlayNote();
					textComp.gameObject.SetActive(true);
					mr.enabled = false;
				} ));
			}
		}
	}

	static int[] majorscale = {
		0, 2, 4, 5, 7, 9, 11, 12
	};

	public void PlayNote(){
		var semitones = majorscale[Mathf.FloorToInt(majorscale.Length*Random.value)];
		int oup = Random.value > 0.5f ? 0 : 12;
		audio.pitch = Mathf.Pow(1.05946f, semitones + oup);
		audio.Play();
	}

	public void Flag(){
		if(isRevealed) return;

		isFlagged = !isFlagged;
		mr.sharedMaterial = isFlagged ? flag : standard;
		flagLight.SetActive(isFlagged);
	}
}
