using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	[Header("Config")]
	public GameObject prefab;
	public static float difficulty = 0f;
	public Vector2Int size;
	public TMPro.TMP_Text scoreText;
	public TMPro.TMP_Text gameOverText;
	public Rigidbody wreckingBall;
	public Animator congratsText;
	
	[Header("State")]
	public CubeController[,] cubes;
	public List<CubeController> mines;
	public int totalMines;
	public int totalSafe;
	public int totalRevealed;
	public int totalCubes;
	public bool gameOver;

    void Start() {
		Time.timeScale = 1;
		Time.fixedDeltaTime = 0.02f;
		Random.InitState((int)System.DateTime.Now.Ticks);

		transform.position = -(new Vector3(size.x-1, size.y-1, 0)/2);
		cubes = new CubeController[size.x, size.y];
		cubes.ForEach2D( (v,_) => {
			var go = Instantiate(prefab, transform.position + new Vector3(v.x, v.y, 0), Quaternion.identity);
			go.transform.parent = transform;

			var cc = go.GetComponent<CubeController>();
			cubes[v.x, v.y] = cc;
			cc.index = new Vector2Int(v.x, v.y);
			cc.gm = this;

			cc.audio.panStereo = (( (float)v.x - size.x/2f)/size.x)*2;

			if(cc.isMine = Random.value < difficulty) {
				totalMines++;
				mines.Add(cc);
			}
			else totalSafe++;
			totalCubes++;
		});
		mines.Shuffle();

		cubes.ForEach2D( (v, cc) => {
			// not actually necessary, just an easy optimisation
			if(cc.isMine) return;
			var counter = 0;
			cubes.ForEachNeighbours(v.x, v.y, (_, n) => { if(n.isMine) counter++; });
			cc.nearbyMines = counter;
		});

		scoreText.text = totalSafe.ToString();
    }

	void Update(){
		if(Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("Menu");
		if(Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		var mb0 = Input.GetMouseButtonDown(0);
		var mb1 = Input.GetMouseButtonDown(1);

		if((mb0 || mb1) && !gameOver){
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100);
			var cc = hit.collider?.gameObject.GetComponent<CubeController>();
			if(cc != null){
				if(mb0) {
					if(cc.isFlagged) return;
					cc.Reveal();
					if(cc.isMine){
						SetGameOver(false);
						float totTime = 0;
						foreach(var m in mines){
							var t = Random.value;
							if(totTime > 7) t *= 0.075f;
							StartCoroutine(Delay(totTime+=t, ()=>m.Reveal()));
						}
					}
				}
				else if(mb1) cc.Flag();
			}
		}

	}

	public void RevealedUp(){
		totalRevealed++;
		var progress = (totalSafe - totalRevealed);
		scoreText.text = progress.ToString();
		if(progress == 0) SetGameOver(true);
	}

	void SetGameOver(bool win){
		gameOver = true;
		if(win){
			StartCoroutine(Delay(1.25f, ()=> {
				congratsText.SetTrigger("FadeIn");
				Time.timeScale = 0.025f;
				Time.fixedDeltaTime *= 0.025f;
			}));
			// Time.timeScale = 0.1f;
			GetComponent<AudioSource>().Play();
			cubes.ForEach2D((v, cc) => cc.GetComponent<Rigidbody>().isKinematic = false);
			wreckingBall.isKinematic = false;
			wreckingBall.AddForce(Vector3.right*50, ForceMode.VelocityChange);
		} else {
			scoreText.text = "Game Over!";
		}
	}

	public IEnumerator Delay(float s, System.Action cb){
		yield return new WaitForSecondsRealtime(s);
		cb();
	}
}
