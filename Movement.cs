using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    
    // RigidBody Properties
    public int flySpeed = 20;
    public int fallSpeed = 20;
    Vector3 targetPos = new Vector3(-4,-7,0); //Start Position
    private Rigidbody rb;

    // Particle System (Explosions)
    public ParticleSystem particle;

    // Audio Sources
    public AudioSource[] sounds;
    public AudioSource upSound;
    public AudioSource downSound;
    public AudioSource explosion;
    public AudioSource goldSound;

    private void Awake()
    {
        // Because Particle System Always Triggers on Awake. Stop it.
        particle.Stop();
    }

    private void Start()
    {
        // RigidBody
        rb = gameObject.GetComponent<Rigidbody>();
        this.transform.position = targetPos;

        // Audio Sources
        sounds = GetComponents<AudioSource>();
        upSound = sounds[0];
        downSound = sounds[1];
        explosion = sounds[2];
        goldSound = sounds[3];

    }

    private void FixedUpdate()
    {

        if (MasterManager.Instance.gameOver == false)
        {
            //Add Scores per frame second
            MasterManager.Instance.AddScore();

            //Unfreeze Copter
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezePositionX;
            rb.constraints = RigidbodyConstraints.FreezePositionZ;

            if (Input.GetMouseButton(0))
            {
                rb.AddForce(new Vector3(0.0f, 1.5f, 0.0f) * flySpeed);

                //Play Sound
                upSound.Play();
            }
            else
            {
                rb.AddForce(new Vector3(0.0f, 1.0f, 0.0f) * -flySpeed);
                downSound.Play();
            }
        }

        else
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
            

        if (other.tag == "Gold")
        {
            // Play Sound Before Destroy
			goldSound.Play();

            // Destroy Gold GameObject
            MasterManager.Instance.AddGold(1);
            Destroy(other.gameObject);
        }

        else 
        {
            // Play Explosion Particle
            particle.Play();

            // Play death sound
            explosion.Play();

            //Stop All other Sounds
            upSound.Stop();
            downSound.Stop();
            MasterManager.Instance.GameOver();
        }
    }
    

}
