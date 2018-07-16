using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;
    public float sinkSpeed = 2.5f;
    public int scoreValue = 10;
    public AudioClip deathClip;
	public string id;
	public bool isDead;
	public GameController gameController;

	public float t;
	public Vector3 startPosition;
	public Vector3 target;
	public float timeToReachTarget;

    Animator anim;
    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;


    bool isSinking;


    void Awake ()
    {
        anim = GetComponent <Animator> ();
        enemyAudio = GetComponent <AudioSource> ();
        hitParticles = GetComponentInChildren <ParticleSystem> ();
        capsuleCollider = GetComponent <CapsuleCollider> ();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();

        currentHealth = startingHealth;
    }


	void Start() {
		startPosition = target = transform.position;
	}

	public void SetDestination(Vector3 destination, float time)
	{
		t = 0;
		startPosition = transform.position;
		timeToReachTarget = time;
		target = destination; 
	}

    void Update ()
    {
		if (isSinking) {
			transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
		} else {
			t += Time.deltaTime / timeToReachTarget;
			transform.position = Vector3.Lerp (startPosition, target, t);
		}
    }


    public void TakeDamage (int amount, Vector3 hitPoint)
    {
        if(isDead)
            return;

        enemyAudio.Play ();

        currentHealth -= amount;
            
        hitParticles.transform.position = hitPoint;
        hitParticles.Play();

		JSONObject currHealthJson = new JSONObject ();

		currHealthJson.AddField ("type", "enemyHealth");
		currHealthJson.AddField ("id", id);
		currHealthJson.AddField ("health", currentHealth.ToString());

		gameController.SocketSend (currHealthJson.ToString ());

        if(currentHealth <= 0)
        {
            Death ();
        }
    }


    void Death ()
    {
        isDead = true;

        capsuleCollider.isTrigger = true;

        anim.SetTrigger ("Dead");

        enemyAudio.clip = deathClip;
        enemyAudio.Play ();
    }


    public void StartSinking ()
    {
//        GetComponent <UnityEngine.AI.NavMeshAgent> ().enabled = false;
        GetComponent <Rigidbody> ().isKinematic = true;
        isSinking = true;
        ScoreManager.score += scoreValue;
        Destroy (gameObject, 2f);
    }
}
