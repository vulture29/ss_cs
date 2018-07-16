using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public int pie = 0;
	public int apple = 0;
	public int cake = 0;
	public GameController gameController;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "HealthPack") {
			GetComponent<PlayerHealth> ().currentHealth += 20;
			if (GetComponent<PlayerHealth> ().currentHealth > 100) {
				GetComponent<PlayerHealth> ().currentHealth = 100;
			}

			JSONObject updateJson = new JSONObject ();

			updateJson.AddField ("type", "itemGet");
			updateJson.AddField ("id", other.gameObject.GetComponent<HealthPackController>().id);

			gameController.SocketSend (updateJson.ToString ());
			Destroy (other.gameObject);
		}
		else if (other.tag == "pie") {
			pie++;

			JSONObject updateJson = new JSONObject ();

			updateJson.AddField ("type", "itemGet");
			updateJson.AddField ("id", other.gameObject.GetComponent<PieController>().id);

			gameController.SocketSend (updateJson.ToString ());
			Destroy (other.gameObject);
		}
		else if (other.tag == "apple") {
			apple++;

			JSONObject updateJson = new JSONObject ();

			updateJson.AddField ("type", "itemGet");
			updateJson.AddField ("id", other.gameObject.GetComponent<AppleController>().id);

			gameController.SocketSend (updateJson.ToString ());
			Destroy (other.gameObject);
		}
		if (pie > 0 && apple > 0) {
			JSONObject updateJson = new JSONObject ();
			updateJson.AddField ("type", "cakeGen");

			gameController.SocketSend (updateJson.ToString ());
			pie = 0;
			apple = 0;
		}
	}
}
