using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {
	
	public GameController gameController;

	public GameObject healthPackGroup;
	public GameObject cakeGroup;
	public GameObject pieGroup;
	public GameObject appleGroup;
	public GameObject healthPackPrefab;
	public GameObject cakePrefab;
	public GameObject piePrefab;
	public GameObject applePrefab;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
	}

	void FixedUpdate () {
		if ((gameController.getData () == null || gameController.getData ().Equals ("")
			|| gameController.getData ().Equals ("init"))) {
			return;
		}

		JSONObject itemData = (new JSONObject(gameController.data))["Item"];
//		print ("Item: " + itemData.ToString ());

		foreach(JSONObject item in itemData["HealthPack"].list) {
			Vector3 updatePos = new Vector3 (float.Parse(item["x"].ToString()),
				float.Parse(item["y"].ToString()), float.Parse(item["z"].ToString()));
			bool found = false;
			foreach (Transform child in healthPackGroup.transform) {
				if (child.GetComponent<HealthPackController> ().id.Equals (item ["id"].ToString())) {
					found = true;
					child.GetComponent<Rigidbody> ().MovePosition (updatePos);
					break;
				}
			}
			if (!found) {
				GameObject newObj = Instantiate (healthPackPrefab, updatePos, new Quaternion(), healthPackGroup.transform);
				newObj.GetComponent<HealthPackController> ().id = item ["id"].ToString();
			}
		}
		foreach(JSONObject item in itemData["Cake"].list) {
			Vector3 updatePos = new Vector3 (float.Parse(item["x"].ToString()),
				float.Parse(item["y"].ToString()), float.Parse(item["z"].ToString()));
			bool found = false;
			foreach (Transform child in cakeGroup.transform) {
				if (child.GetComponent<CakeController> ().id.Equals (item ["id"].ToString())) {
					found = true;
					child.GetComponent<Rigidbody> ().MovePosition (updatePos);
					break;
				}
			}
			if (!found) {
				GameObject newObj = Instantiate (cakePrefab, updatePos, new Quaternion(), cakeGroup.transform);
				newObj.GetComponent<CakeController> ().id = item ["id"].ToString();
			}
		}
		foreach(JSONObject item in itemData["Pie"].list) {
			Vector3 updatePos = new Vector3 (float.Parse(item["x"].ToString()),
				float.Parse(item["y"].ToString()), float.Parse(item["z"].ToString()));
			bool found = false;
			foreach (Transform child in pieGroup.transform) {
				if (child.GetComponent<PieController> ().id.Equals (item ["id"].ToString())) {
					found = true;
					child.GetComponent<Rigidbody> ().MovePosition (updatePos);
					break;
				}
			}
			if (!found) {
				GameObject newObj = Instantiate (piePrefab, updatePos, new Quaternion(), pieGroup.transform);
				newObj.GetComponent<PieController> ().id = item ["id"].ToString();
			}
		}
		foreach(JSONObject item in itemData["Apple"].list) {
			Vector3 updatePos = new Vector3 (float.Parse(item["x"].ToString()),
				float.Parse(item["y"].ToString()), float.Parse(item["z"].ToString()));
			bool found = false;
			foreach (Transform child in appleGroup.transform) {
				if (child.GetComponent<AppleController> ().id.Equals (item ["id"].ToString())) {
					found = true;
					child.GetComponent<Rigidbody> ().MovePosition (updatePos);
					break;
				}
			}
			if (!found) {
				GameObject newObj = Instantiate (applePrefab, updatePos, new Quaternion(), appleGroup.transform);
				newObj.GetComponent<AppleController> ().id = item ["id"].ToString();
			}
		}
	}
}
