using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ResetScene : MonoBehaviour
{
    //credit to answers.unity.com/questions/1261937/creating-a-restart-button.html
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
