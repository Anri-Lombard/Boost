using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    int sceneIndex;

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(1f);
        if (sceneIndex == 2)
        {
            sceneIndex = -1;
        }
        SceneManager.LoadScene(sceneIndex + 1);
    }

    public IEnumerator Restart()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneIndex);
    }
}
