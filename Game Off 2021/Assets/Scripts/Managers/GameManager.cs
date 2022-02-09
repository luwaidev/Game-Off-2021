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
    public bool upgrade;
    public Vector2 spawnPosition;
    public bool effect;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("GameController") != gameObject) Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {

        transition.SetTrigger("Transition"); // Start transitioning scene out
    }
    public void Load(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    public void LoadWithDelay(string sceneName, float delayTime)
    {
        StartCoroutine(Delay(sceneName, delayTime));
    }

    IEnumerator Delay(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
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
        while (!load.isDone)
        {
            if (load.progress >= 0.9f)
            {
                load.allowSceneActivation = true;
            }

            yield return null;
        }
        load.allowSceneActivation = true;


        transition.SetTrigger("Transition"); // Start transitioning scene back

        yield return new WaitForEndOfFrame();
        if (sceneName != "Main Menu")
        {
            PlayerController.instance.transform.position = spawnPosition;
            Camera.main.transform.position = (Vector3)spawnPosition - Vector3.forward * 10;
            UpgradeManager.instance.UpdateUpgrade();
        }
        yield return new WaitForSeconds(sceneTransitionTime); // Wait for transition
        loadingScene = false;

        yield return new WaitForSeconds(1);
        instance = this;
    }

    private void Update()
    {
        Time.timeScale = (paused || effect || upgrade) ? 0 : 1;

        if (!paused && Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
    }

    public void TogglePause()
    {
        paused = !paused;
        pauseMenu.SetBool("Paused", paused);
    }

    public IEnumerator HitEffect()
    {
        effect = true;
        yield return new WaitForSecondsRealtime(0.1f);
        effect = false;

    }
}
