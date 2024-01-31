using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlaySimulator() {
        // Load the selection scene, check order in build manager
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitSimulator() {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
