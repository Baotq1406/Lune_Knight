using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public void LoadSceneLV_1()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadSceneVitory()
    {
        SceneManager.LoadScene(5);
    }

    public void GameOver()
    {
        StartCoroutine(ShowGameOverScreen());
    }

    IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(4);
    }
}
