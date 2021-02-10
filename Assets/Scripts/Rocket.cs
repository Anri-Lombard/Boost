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

    [SerializeField] float rcsThrust = 300f, mainThrust = 35f * 100;
    [SerializeField] AudioClip mainEngine, explosion, success;
    [SerializeField] ParticleSystem engineParticles, successParticles, explosionParticles;

    int sceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
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
        Instantiate(explosionParticles, new Vector3(0, 0, 0), Quaternion.identity);
        explosionParticles.Play();
        audioSource.Stop();
        audioClipSound = explosion;
        audioSource.PlayOneShot(audioClipSound);
        StartCoroutine(Restart());
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
        StartCoroutine(LoadNextScene());
        StartCoroutine(WaitAndStopSuccessParticles());
    }

    IEnumerator WaitAndStopSuccessParticles()
    {
        yield return new WaitForSeconds(1f);
        explosionParticles.Stop();
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
            rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                audioClipSound = mainEngine;
                audioSource.PlayOneShot(audioClipSound);
            }
            engineParticles.Play();
        }
        else
        {
            if (audioClipSound == mainEngine)
            {
                audioSource.Stop();
            }
            engineParticles.Stop();
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f);
        if (sceneIndex == 4)
        {
            sceneIndex = -1;
        }
        SceneManager.LoadScene(sceneIndex + 1);
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneIndex);
    }
}
