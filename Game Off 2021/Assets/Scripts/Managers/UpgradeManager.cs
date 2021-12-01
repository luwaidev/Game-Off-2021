using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;
    public int meleeDamageUpgrade;
    public int icicleDamageUpgrade;


    public int meleeDamage;
    public int icicleDamage;
    public int playerHealth;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void LateUpdate()
    {
        if (instance != this)
        {
            instance = this;
            if (PlayerController.instance.abilities.Count > 0)
            {
                Destroy(GameObject.Find("Icicle Upgrade"));
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void Upgrade(int upgradeType)
    {
        switch (upgradeType)
        {
            case 0:
                HealthController.instance.playerMaxHealth += 1;
                PlayerController.instance.health += 1;
                playerHealth = HealthController.instance.playerMaxHealth;
                ShowUpgrade("Health Up");
                HealthController.instance.OnAddHealth();
                break;
            case 1:
                PlayerController.instance.meleeDamage += meleeDamageUpgrade;
                PlayerController.instance.meleeDamage = meleeDamageUpgrade;
                ShowUpgrade("Melee Up");
                break;
            case 2:
                PlayerController.instance.icicleDamage += icicleDamageUpgrade;
                PlayerController.instance.icicleDamage = icicleDamageUpgrade;
                ShowUpgrade("Icicle Up");
                break;
            case 10:
                PlayerController.instance.abilities.Add(PlayerController.instance.Shoot);
                ShowUpgrade("Icicle Unlocked");
                break;
            default:
                break;
        }
    }

    public void UpdateUpgrade()
    {
        HealthController.instance.playerMaxHealth = playerHealth;
        PlayerController.instance.health = playerHealth;
        PlayerController.instance.meleeDamage = meleeDamageUpgrade;
        PlayerController.instance.icicleDamage = icicleDamageUpgrade;
    }

    void ShowUpgrade(string upgrade)
    {
        GameObject.Find("Upgrade Indicator").transform.parent.GetComponent<Animator>().Play("Show Upgrade UI");
        GameObject.Find("Upgrade Indicator").GetComponent<TMP_Text>().text = upgrade;
    }
}
