using UnityEngine;

public class GameOverManager : MonoBehaviour
{
	public PlayerHealth playerHealth;       // Reference to the player's health.
	public float exitDelay = 5f;         // Time to wait before restarting the level


	Animator anim;                          // Reference to the animator component.
	float restartTimer;                     // Timer to count up to restarting the level


	void Awake ()
	{
		// Set up the reference.
		anim = GetComponent <Animator> ();
	}


	void Update ()
	{
		// If the player has run out of health...
//		print(playerHealth.currentHealth.ToString());
		if(playerHealth.isDead)
		{
			// ... tell the animator the game is over.
			print("GameOver");
			anim.SetTrigger ("GameOver");

			// .. increment a timer to count up to restarting.
			restartTimer += Time.deltaTime;

			// .. if it reaches the restart delay...
			if(restartTimer >= exitDelay)
			{
				// .. then reload the currently loaded level.
				print("Load login scence");
			}
		}
	}
}