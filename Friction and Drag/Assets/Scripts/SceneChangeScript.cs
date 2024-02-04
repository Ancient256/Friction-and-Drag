using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public Button tmpButton;  // Drag and drop your TMP button onto this field in the Unity Editor
    public string sceneToLoad;  // Name of the scene you want to load

    private void Start()
    {
        // Ensure TMP button is assigned
        if (tmpButton != null)
        {
            // Add a listener to the TMP button's onClick event
            tmpButton.onClick.AddListener(ChangeScene);
        }
        else
        {
            Debug.LogError("TMP Button is not assigned. Please assign a TMP button in the Unity Editor.");
        }
    }

    private void ChangeScene()
    {
        // Check if the scene name is not empty
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Scene name is not set. Please assign a scene name in the Unity Editor.");
        }
    }
}
