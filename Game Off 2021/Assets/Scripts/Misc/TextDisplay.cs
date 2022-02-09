using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    [Header("References")]
    public static TextDisplay instance;
    public Transform display;
    public Transform bar;
    public GameObject currentDialogue;
    public string[] currentTexts;

    [Header("Text Settings")]
    public bool shown;
    [SerializeField] float textSpeed;
    [SerializeField] float punctuationTime;

    [Header("Dialogue Position Settings")]
    public float transitionTime;
    public float displaySpeed;
    public float barSpeed;

    public float visiblePosition;
    public float hiddenPosition;
    public float visibleBarSize;
    public float hiddenBarSize;

    public bool unlockMelee;
    public bool unlockDash;
    public bool unlockIcicle;

    public AudioSource dialogueSound;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        display.position =
            new Vector2(display.position.x,
                        Mathf.Lerp(display.position.y,
                        (shown ? visiblePosition : hiddenPosition),
                        displaySpeed));

        bar.localScale = new Vector2(1,
                                 Mathf.Lerp(bar.localScale.y,
                                 (shown ? visibleBarSize : hiddenBarSize),
                                 barSpeed));
    }
    public IEnumerator DisplayText()
    {
        shown = true;
        Debug.Log("Start");
        OpenText();

        PlayerController.instance.movementLocked = true;
        Transform cd = currentDialogue.transform;
        yield return new WaitForSeconds(transitionTime);

        // Loop through all sections of dialogue
        for (int i = 0; i < cd.childCount; i++)
        {

            cd.GetChild(i).gameObject.SetActive(false);
            // Loop through individual texts in dialogue and record their text
            currentTexts = new string[cd.GetChild(i).childCount];
            for (int j = 0; j < cd.GetChild(i).transform.childCount; j++)
            {
                currentTexts[j] = cd.GetChild(i).GetChild(j).GetComponent<TMP_Text>().text;
                cd.GetChild(i).GetChild(j).GetComponent<TMP_Text>().text = "";
            }

            cd.GetChild(i).gameObject.SetActive(true);

            // Loop through the individial texts again
            for (int j = 0; j < cd.GetChild(i).transform.childCount; j++)
            {
                // Get text object 
                TMP_Text text = cd.GetChild(i).transform.GetChild(j).GetComponent<TMP_Text>();

                // Loop through the characters and set the texts to them
                char[] characters = currentTexts[j].ToCharArray();
                for (int x = 0; x < characters.Length; x++)
                {
                    text.text += characters[x];

                    if (characters[x] == ',' || characters[x] == '.')
                    {
                        yield return new WaitForSeconds(punctuationTime);
                    }
                    else
                    {
                        yield return new WaitForSeconds(textSpeed);
                    }
                    if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    {
                        break;
                    }
                    dialogueSound.Play();
                }
                text.text = currentTexts[j];

            }

            PlayerController.instance.movementLocked = true;
            yield return new WaitForSeconds(textSpeed);
            while (!Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                yield return null;
            }
            cd.GetChild(i).gameObject.SetActive(false);
        }

        Debug.Log("End");
        CloseText();
        shown = false;

        yield return new WaitForSeconds(transitionTime);



        if (unlockMelee)
        {
            PlayerController.instance.hasMelee = true;
            StartCoroutine(UpgradeManager.instance.ShowUpgrade("Melee Attack", "Left Click to Attack"));
            unlockMelee = false;
        }
        else if (unlockDash)
        {
            PlayerController.instance.abilities.Add(PlayerController.instance.Dash);
            StartCoroutine(UpgradeManager.instance.ShowUpgrade("Dash", "Press Shift to Dash"));
            unlockDash = false;
        }
        else if (unlockIcicle)
        {
            PlayerController.instance.abilities.Add(PlayerController.instance.Shoot);
            StartCoroutine(UpgradeManager.instance.ShowUpgrade("Ranged Attack", "Right Click to Fire"));
            unlockIcicle = false;
        }
        PlayerController.instance.movementLocked = false;
    }
    void OpenText()
    {

    }
    void CloseText()
    {

    }
}
