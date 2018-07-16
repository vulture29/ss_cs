using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
	public bool isDead;
	public float restartDelay = 4.0f;

	GameController gameController;
    Animator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    //PlayerShooting playerShooting;
    bool damaged;
	float restartDelayTimer = 0;

    void Awake ()
    {
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        playerMovement = GetComponent <PlayerMovement> ();
        //playerShooting = GetComponentInChildren <PlayerShooting> ();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController>();
        currentHealth = startingHealth;
    }


    void Update ()
    {
//		print ((new JSONObject (gameController.data)).ToString ());
		currentHealth = int.Parse((new JSONObject(gameController.data))["health"].ToString());

		healthSlider.value = currentHealth;


        if(damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        damaged = true;

        playerAudio.Play ();
		currentHealth = currentHealth - 10;

		JSONObject currHealthJson = new JSONObject ();

		currHealthJson.AddField ("type", "playerHealth");
		currHealthJson.AddField ("health", currentHealth.ToString());

		gameController.SocketSend (currHealthJson.ToString ());

        if(currentHealth <= 0 && !isDead)
        {
			gameController.SocketQuit ();
            Death ();
        }
    }


    void Death ()
    {
        isDead = true;
		anim.SetTrigger ("Die");
        //playerShooting.DisableEffects ();

        playerAudio.clip = deathClip;
        playerAudio.Play ();
		playerMovement.enabled = false;

    }


	public void ReturnToMain ()
    {
        SceneManager.LoadScene (0);
    }
}
