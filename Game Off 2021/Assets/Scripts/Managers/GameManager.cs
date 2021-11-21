using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    private Animator anim;

    [Header("Scene Loading Settings")]
    [SerializeField] float sceneTransitionTime;
    public bool loadingScene;
    public bool paused;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;

        // Set up references
        anim = GetComponent<Animator>();
    }

    public void Load(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    public IEnumerator LoadScene(string sceneName)
    {
        loadingScene = true;
        anim.Play("TransitionOut", 0); // Start transitioning scene out
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition

        // Start loading scene
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        load.allowSceneActivation = false;
        while (!load.isDone) yield return null;

        anim.SetTrigger("FinishedLoadingScene"); // Start transitioning scene back

        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        loadingScene = false;
    }

    private void Update()
    {
        Time.timeScale = paused ? 0 : 1;
    }

    public void TogglePause()
    {
        paused = !paused;
    }
}
