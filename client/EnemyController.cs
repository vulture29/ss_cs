using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour {

	public GameController gameController;
	public GameObject zomBunnyPrefab;
	public GameObject zomBearPrefab;
	public GameObject hellephantPrefab;
	public GameObject zomBunnyGroup;
	public GameObject zomBearGroup;
	public GameObject hellephantGroup;
	public GameObject player;

	Dictionary<string, GameObject> zomBunnyDict = new Dictionary <string, GameObject> ();
	Dictionary<string, GameObject> zomBearDict = new Dictionary <string, GameObject> ();
	Dictionary<string, GameObject> hellephantDict = new Dictionary <string, GameObject> ();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update() {
		if ((gameController.getData () == null || gameController.getData ().Equals ("")
		   || gameController.getData ().Equals ("init"))) {
			return;
		}

		JSONObject enemyData = (new JSONObject(gameController.data))["Enemy"];
//		print ("data: " + gameController.data);

		foreach(JSONObject zomBunny in enemyData["Zombunny"].list) {
			Vector3 updatePos = new Vector3 (float.Parse(zomBunny["x"].ToString()),
				float.Parse(zomBunny["y"].ToString()), float.Parse(zomBunny["z"].ToString()));
			
			if (zomBunnyDict.ContainsKey(zomBunny["id"].ToString())) {
				GameObject zomBunnyObj = zomBunnyDict[zomBunny ["id"].ToString()];
				if (zomBunnyObj != null) {
					Vector3 velocity = player.transform.position - zomBunnyObj.transform.position;
					zomBunnyObj.GetComponent<EnemyHealth> ().SetDestination (updatePos, (float)0.2);
					zomBunnyObj.transform.LookAt (player.transform);
				}
			}
			else {
				GameObject newObj = Instantiate (zomBunnyPrefab, updatePos, new Quaternion(), zomBunnyGroup.transform);
				zomBunnyDict.Add(zomBunny["id"].ToString(), newObj);
				newObj.GetComponent<EnemyHealth> ().id = zomBunny ["id"].ToString();
			}
			
		}
		foreach(JSONObject zomBear in enemyData["ZomBear"].list) {
			Vector3 updatePos = new Vector3 (float.Parse(zomBear["x"].ToString()),
				float.Parse(zomBear["y"].ToString()), float.Parse(zomBear["z"].ToString()));

			if (zomBearDict.ContainsKey(zomBear["id"].ToString())) {
				GameObject zomBearObj = zomBearDict[zomBear ["id"].ToString()];
				if (zomBearObj != null) {
					Vector3 velocity = player.transform.position - zomBearObj.transform.position;
					zomBearObj.GetComponent<EnemyHealth> ().SetDestination (updatePos, (float)0.2);
					zomBearObj.transform.LookAt (player.transform);
				}
			}
			else {
				GameObject newObj = Instantiate (zomBearPrefab, updatePos, new Quaternion(), zomBearGroup.transform);
				zomBearDict.Add(zomBear["id"].ToString(), newObj);
				newObj.GetComponent<EnemyHealth> ().id = zomBear ["id"].ToString();
			}
		}
		foreach(JSONObject hellephant in enemyData["Hellephant"].list) {
			Vector3 updatePos = new Vector3 (float.Parse(hellephant["x"].ToString()),
				float.Parse(hellephant["y"].ToString()), float.Parse(hellephant["z"].ToString()));

			if (hellephantDict.ContainsKey(hellephant["id"].ToString())) {
				GameObject hellephantObj = hellephantDict[hellephant ["id"].ToString()];
				if (hellephantObj != null) {
					Vector3 velocity = player.transform.position - hellephantObj.transform.position;
					hellephantObj.GetComponent<EnemyHealth> ().SetDestination (updatePos, (float)0.2);
					hellephantObj.transform.LookAt (player.transform);
				}
			}
			else {
				GameObject newObj = Instantiate (hellephantPrefab, updatePos, new Quaternion(), hellephantGroup.transform);
				hellephantDict.Add(hellephant["id"].ToString(), newObj);
				newObj.GetComponent<EnemyHealth> ().id = hellephant ["id"].ToString();
			}
		}

		for(int i = 0; i < zomBunnyGroup.transform.childCount; i++)
		{
			GameObject child = zomBunnyGroup.transform.GetChild(i).gameObject;
			if (!zomBunnyDict.ContainsKey (child.GetComponent<EnemyHealth>().id.ToString ())) {
				Destroy (child);
			}
		}
		for(int i = 0; i < zomBearGroup.transform.childCount; i++)
		{
			GameObject child = zomBearGroup.transform.GetChild(i).gameObject;
			if (!zomBearDict.ContainsKey (child.GetComponent<EnemyHealth>().id.ToString ())) {
				Destroy (child);
			}
		}
		for(int i = 0; i < hellephantGroup.transform.childCount; i++)
		{
			GameObject child = hellephantGroup.transform.GetChild(i).gameObject;
			if (!hellephantDict.ContainsKey (child.GetComponent<EnemyHealth>().id.ToString ())) {
				Destroy (child);
			}
		}
	}
}
