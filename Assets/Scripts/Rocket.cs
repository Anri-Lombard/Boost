using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    SceneLoader sceneLoader;
    AudioClip audioClipSound;

    [SerializeField] float rcsThrust = 300f, mainThrust = 600f;
    [SerializeField] AudioClip mainEngine, explosion, success;
    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem explosionParticles;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        sceneLoader = new SceneLoader();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        RespondToThrustInput();
        RespondToRotateInput();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        audioSource.Stop();
        audioClipSound = explosion;
        audioSource.PlayOneShot(audioClipSound);
        StartCoroutine(sceneLoader.Restart());
    }

    private void StartSuccessSequence()
    {
        audioSource.Stop();
        audioClipSound = success;
        audioSource.PlayOneShot(audioClipSound);
        StartCoroutine(sceneLoader.LoadNextScene());
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

    private void RespondToThrustInput()
    {
        MainEngineSound();
    }

    private void MainEngineSound()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            engineParticles.Play();
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying)
            {
                audioClipSound = mainEngine;
                audioSource.PlayOneShot(audioClipSound);
            }
        }
        else
        {
            if (audioClipSound == mainEngine)
            {
                audioSource.Stop();
            }
        }
    }
}
