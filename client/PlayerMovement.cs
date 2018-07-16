using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 6.0f;
	public GameController gameController;
	public bool isSkating = false;

	Vector3 movements;
	Animator animator;
	Rigidbody playerRigidbody;
	Transform initTransform;
	int floorMask;
	float camRayLength = 100.0f;

	void Awake() {
		floorMask = LayerMask.GetMask ("Floor");
		animator = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody> ();
	}

	void Start () {
		StartCoroutine (initTrans ());
		while (gameController.getData () == null || gameController.getData ().Equals ("")
		      || gameController.getData ().Equals ("init")) {
			System.Threading.Thread.Sleep(100);
		};

		InvokeRepeating("updateTrans", 0.0f, 0.5f);
	}

	void FixedUpdate() {
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");

		Move (horizontal, vertical);
		Turning ();
		Animating(horizontal, vertical);
	}

	IEnumerator initTrans () {
		Debug.Log("Waiting for data");
		yield return new WaitUntil(() => (gameController.getData () != null && !gameController.getData ().Equals ("")
			&& !gameController.getData ().Equals ("init")));

		JSONObject pos = (new JSONObject(gameController.data))["Position"];

		Vector3 initPos = new Vector3(float.Parse(pos["x"].ToString()),
			float.Parse(pos["y"].ToString()), float.Parse(pos["z"].ToString()));
		initTransform = GetComponent<Transform> ();
		initTransform.SetPositionAndRotation(initPos, new Quaternion(0, 0, 0, 1));
		 
	}

	void updateTrans() {
		Transform currTransform = GetComponent<Transform> ();

		if (!GetComponent<PlayerHealth> ().isDead) {
			JSONObject currTransJson = new JSONObject ();
			string currTransStr = "{\"x\": " + currTransform.position.x +
			                     ",\"y\": " + currTransform.position.y + ",\"z\": " + currTransform.position.z + "}";

			currTransJson.AddField ("type", "playerPos");
			currTransJson.AddField ("value", new JSONObject (currTransStr));

			gameController.SocketSend (currTransJson.ToString ());
		}
	}

	void Move(float horizontal, float vertical) {
		movements.Set (horizontal, 0f, vertical);
		movements = movements.normalized * speed * Time.deltaTime;

		playerRigidbody.MovePosition (transform.position + movements);
	}

	void Turning() {
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit floorHit;

		if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
			Vector3 playerToMouse = floorHit.point - transform.position;

			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
			playerRigidbody.MoveRotation (newRotation);
		}
	}

	void Animating(float horizontal, float vertical) {
		bool walking = ((horizontal != 0f || vertical != 0f) && !isSkating);
		animator.SetBool ("IsWalking", walking);
	}
}
