using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class LoginController : MonoBehaviour {

	public InputField inputName;


	public void login()
	{
//		print ("Login: " + inputName.text.Trim().ToString());

		string name = inputName.text.Trim ().ToString ();
		if (name != null && name.Length > 0) {
			GameController.username = name;
		}

		JSONObject loginJson = new JSONObject ();

		loginJson.AddField ("type", "login");
		loginJson.AddField ("value", name);

		StartCoroutine(PutRequest(loginJson.ToString()));
	} 

	public void register()
	{
		print ("Register: " + inputName.text.Trim());
		string name = inputName.text.Trim ().ToString ();

		JSONObject registerJson = new JSONObject ();

		registerJson.AddField ("type", "register");
		registerJson.AddField ("value", name);

		StartCoroutine(PutRequest(registerJson.ToString()));
	}

	IEnumerator PutRequest(string json) {
		JSONObject reqJson = new JSONObject (json);
//		print ("To put: " + json);

		byte[] myData = System.Text.Encoding.UTF8.GetBytes(json);
		UnityWebRequest www = UnityWebRequest.Put("http://localhost:5567", myData);
		yield return www.SendWebRequest();

		if(www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		}

		print (www.responseCode.ToString ());

		if(www.responseCode == 200) {
			SceneManager.LoadScene (1);
		}
	}
}
