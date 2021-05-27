using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // PARAMETERS - for tuning, typically set in the UnityEditor.
    // CACHE - e.g. references for eadability or speed.
    // STATE - private instances (member) variables.



    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rotationThrust = 800f;
    [SerializeField] AudioClip mainEngine;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem leftThrusterParticles;
    [SerializeField] ParticleSystem rightThrusterParticles;

    Rigidbody myRb;
    AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        myRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrust();
        ProcessRotation();
    }

    void ProcessRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateRight();
        }
        else
        {
            StopRotating();
        }

    }


    void ProcessThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StartThrusting();

        }
        else
        {
            StopThrusting();
        }

    }



    void StartThrusting()
    {
        myRb.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        Debug.Log("SPACE is pressed - Thrusting");
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    void StopThrusting()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }



    void StopRotating()
    {
        rightThrusterParticles.Stop();
        leftThrusterParticles.Stop();
    }

    void RotateRight()
    {
        ApplyRotation(-rotationThrust);
        Debug.Log("D is pressed - Rotating Right");
        if (!leftThrusterParticles.isPlaying)
        {
            leftThrusterParticles.Play();
        }
    }

    void RotateLeft()
    {
        ApplyRotation(rotationThrust);
        Debug.Log("A is pressed - Rotating left");
        if (!rightThrusterParticles.isPlaying)
        {
            rightThrusterParticles.Play();
        }
    }

    void ApplyRotation(float rotationThisFrame)
    {
        myRb.freezeRotation = true; // freezing rotation so we can manually rotate the rocket.
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.deltaTime);
        myRb.freezeRotation = false; // unfreezing rotation so the physics system takes over.
    }
}
