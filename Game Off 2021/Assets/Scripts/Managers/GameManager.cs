using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public Animator transition;
    public Animator pauseMenu;

    [Header("Scene Loading Settings")]
    [SerializeField] float sceneTransitionTime;
    public bool loadingScene;
    public bool paused;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
    }

    public void Load(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    public IEnumerator LoadScene(string sceneName)
    {
        loadingScene = true;
        transition.SetTrigger("Transition"); // Start transitioning scene out
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition

        // Start loading scene
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;
        while (!load.isDone) yield return null;

        transition.SetTrigger("Transition"); // Start transitioning scene back

        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        loadingScene = false;
    }

    private void Update()
    {
        Time.timeScale = paused ? 0 : 1;

        if (Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }

    public void TogglePause()
    {
        paused = !paused;
        pauseMenu.SetBool("Paused", paused);
    }
}
