using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;
    public int attackDamage = 10;


    Animator anim;
    GameObject player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;
    bool playerInRange;
    float timer;
	GameController gameController;

    void Awake ()
    {
        player = GameObject.FindGameObjectWithTag ("Player");
        playerHealth = player.GetComponent <PlayerHealth> ();
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent <Animator> ();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
    }


    void OnTriggerEnter (Collider other)
    {
//		Debug.Log("Enter collid");
        if(other.gameObject == player)
        {
            playerInRange = true;
        }
		if (other.tag == "cake") {
			JSONObject updateJson = new JSONObject ();

			updateJson.AddField ("type", "itemGet");
			updateJson.AddField ("id", other.gameObject.GetComponent<CakeController>().id);

			gameController.SocketSend (updateJson.ToString ());
			Destroy (other.gameObject);
		}
    }


    void OnTriggerExit (Collider other)
    {
        if(other.gameObject == player)
        {
            playerInRange = false;
        }
    }


    void Update ()
    {
        timer += Time.deltaTime;

        if(timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
        {
//			Debug.Log ("Attack");
            Attack ();
        }

        if(playerHealth.currentHealth <= 0)
        {
            anim.SetTrigger ("PlayerDead");
        }
    }


    void Attack ()
    {
        timer = 0f;

		if(playerHealth.currentHealth > 0 && !player.GetComponent<PlayerHealth>().isDead)
        {
            playerHealth.TakeDamage (attackDamage);
        }
    }
}
