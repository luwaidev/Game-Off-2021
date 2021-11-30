using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public Transform healthUi;
    public int playerMaxHealth;
    public int currentPlayerHealth;
    public GameObject healthObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayerHealth != PlayerController.instance.health)
        {
            OnHealthChange();
        }
    }

    void OnHealthChange()
    {
        for (int i = 0; i < healthUi.childCount; i++)
        {
            if (i > PlayerController.instance.health - 1)
            {
                healthUi.GetChild(i).GetComponent<Animator>().SetBool("Empty", true);
            }
            else
            {
                healthUi.GetChild(i).GetComponent<Animator>().SetBool("Empty", false);
            }
        }
        currentPlayerHealth = PlayerController.instance.health;
    }

    void OnAddHealth()
    {
        Vector2 position = Vector2.zero;
        position.y = 4;
        position.x = healthUi.GetChild(healthUi.childCount - 1).transform.position.x + 1.25f;
        GameObject health = Instantiate(healthObject, position, Quaternion.identity);
    }

    public void playerRegenHealth()
    {
        PlayerController.instance.health = playerMaxHealth;
    }
}
