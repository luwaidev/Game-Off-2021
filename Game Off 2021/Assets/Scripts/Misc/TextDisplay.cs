using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    [Header("References")]
    public GameObject currentDialogue;
    public string[] currentTexts;
    [Header("Text Settings")]
    [SerializeField] float textSpeed;
    [SerializeField] float punctuationTime;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator DisplayText()
    {
        OpenText();
        Transform cd = currentDialogue.transform;

        // Loop through all sections of dialogue
        for (int i = 0; i < cd.childCount; i++)
        {
            // Loop through individual texts in dialogue and record their text
            currentTexts = new string[cd.childCount];
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
                }

            }

            while (!Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                yield return null;
            }
            cd.GetChild(i).gameObject.SetActive(false);
        }

        CloseText();
    }
    void OpenText()
    {

    }
    void CloseText()
    {

    }
}
