using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    AudioClip audioClipSound;
    MeshCollider meshCollider;

    [SerializeField] float rcsThrust = 300f, mainThrust = 35f * 100;
    [SerializeField] AudioClip mainEngine, explosion, success;
    [SerializeField] ParticleSystem engineParticles, successParticles, explosionParticles;

    int sceneIndex;
    bool collisionsAreEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        meshCollider = GameObject.Find("Body").GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAreEnabled = !collisionsAreEnabled;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }
    }

    private void ProcessInput()
    {
        RespondToThrustInput();
        RespondToRotateInput();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionsAreEnabled == false) { return; }

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
        Instantiate(explosionParticles, new Vector3(0, 0, 0), Quaternion.identity);
        explosionParticles.Play();
        audioSource.Stop();
        audioClipSound = explosion;
        audioSource.PlayOneShot(audioClipSound);
        StartCoroutine(InitiateRestart());
        StartCoroutine(WaitAndStopExplosionParticles());
    }

    IEnumerator WaitAndStopExplosionParticles()
    {
        yield return new WaitForSeconds(1f);
        explosionParticles.Stop();
    }

    private void StartSuccessSequence()
    {
        successParticles.Play();
        audioSource.Stop();
        audioClipSound = success;
        audioSource.PlayOneShot(audioClipSound);
        StartCoroutine(InitiateLoadNextScene());
        StartCoroutine(WaitAndStopSuccessParticles());
    }

    IEnumerator WaitAndStopSuccessParticles()
    {
        yield return new WaitForSeconds(1f);
        explosionParticles.Stop();
    }

    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero; // Remove rotation due to physics.

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }

    private void RespondToThrustInput()
    {
        MainEngineSound();
    }

    private void MainEngineSound()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Thrust();
        }
        else
        {
            DoNotThrust();
        }
    }

    private void DoNotThrust()
    {
        if (audioClipSound == mainEngine)
        {
            audioSource.Stop();
        }
        engineParticles.Stop();
    }

    private void Thrust()
    {
        engineParticles.Play();
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioClipSound = mainEngine;
            audioSource.PlayOneShot(audioClipSound);
        }
        engineParticles.Play();
    }

    IEnumerator InitiateLoadNextScene()
    {
        yield return new WaitForSeconds(1f);
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (sceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            sceneIndex = -1;
        }
        SceneManager.LoadScene(sceneIndex + 1);
    }

    IEnumerator InitiateRestart()
    {
        yield return new WaitForSeconds(1f);
        RestartScene();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
