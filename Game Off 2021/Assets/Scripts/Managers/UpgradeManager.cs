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

    [Header("Animations")]
    public float startTime;
    public float textTime;
    public TMP_Text text;
    public TMP_Text upgradeText;
    public TMP_Text descriptionText;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void LateUpdate()
    {
        if (instance != this)
        {
            instance = this;

        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position;
    }

    public void Upgrade(int upgradeType)
    {
        switch (upgradeType)
        {
            case 0:
                HealthController.instance.playerMaxHealth += 1;
                PlayerController.instance.health += 1;
                playerHealth = HealthController.instance.playerMaxHealth;
                StartCoroutine(ShowUpgrade("Health Upgrade", "+1 Health"));
                HealthController.instance.OnAddHealth();
                break;
            case 1:
                PlayerController.instance.meleeDamage += meleeDamageUpgrade;
                PlayerController.instance.meleeDamage = meleeDamageUpgrade;
                StartCoroutine(ShowUpgrade("Melee Upgrade", "Melee Damage Up"));
                break;
            case 2:
                PlayerController.instance.icicleDamage += icicleDamageUpgrade;
                PlayerController.instance.icicleDamage = icicleDamageUpgrade;
                StartCoroutine(ShowUpgrade("Icicle Upgrade", "Icicle Damage Up"));
                break;
            case 10:
                PlayerController.instance.abilities.Add(PlayerController.instance.Shoot);
                StartCoroutine(ShowUpgrade("Icicle Unlocked", "Right Click to fire an Icicle"));
                break;
            case 11:
                PlayerController.instance.abilities.Add(PlayerController.instance.Dash);
                StartCoroutine(ShowUpgrade("Dash Unlocked", "Shift to Dash"));
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

        if (PlayerController.instance != null && PlayerController.instance.abilities.Count > 0)
        {
            Destroy(GameObject.Find("Icicle Upgrade"));
        }
    }

    public IEnumerator ShowUpgrade(string upgrade, string description)
    {
        GameManager.instance.upgrade = true;
        GetComponent<Animator>().SetTrigger("Show");
        yield return new WaitForSecondsRealtime(1.1f);

        for (int i = 0; i < "You Got:".Length; i++)
        {
            text.text += "You Got:".ToCharArray()[i];
            yield return new WaitForSecondsRealtime(textTime);
        }

        yield return new WaitForSecondsRealtime(1);
        for (int i = 0; i < upgrade.Length; i++)
        {

            upgradeText.text += upgrade.ToCharArray()[i];
            yield return new WaitForSecondsRealtime(textTime);
        }
        yield return new WaitForSecondsRealtime(1);
        for (int i = 0; i < description.Length; i++)
        {

            descriptionText.text += description.ToCharArray()[i];
            yield return new WaitForSecondsRealtime(textTime);
        }

        yield return new WaitForSecondsRealtime(2);

        GetComponent<Animator>().SetTrigger("Hide");
        yield return new WaitForSecondsRealtime(1.1f);
        text.text = "";
        upgradeText.text = "";
        GameManager.instance.upgrade = false;
    }
}
