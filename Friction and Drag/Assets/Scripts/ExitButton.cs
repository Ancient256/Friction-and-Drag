using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExitButton : MonoBehaviour
{
    public Button tmpButton;  // Use the standard Unity Button component for TMP buttons

    private void Start()
    {
        if (tmpButton != null)
        {
            tmpButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("Button is not assigned. Please assign a button in the Unity Editor.");
        }
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
