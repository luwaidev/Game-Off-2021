using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CameraSection : MonoBehaviour
{
    public Vector3 maxPosition;
    public Vector3 minPosition;
    public bool oneWay;

    [Header("Wave Settings")]
    public bool waveStarted;
    public GameObject Wall;
    public GameObject[] enemies;
    public int waveSize;

    private void Start()
    {

        if (oneWay) Wall.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.maxPosition = maxPosition;
            cameraController.minPosition = minPosition;
            if (oneWay) Wall.SetActive(true);
            waveStarted = true;
        }
    }
    private void Update()
    {
        if (oneWay && waveStarted)
        {
            bool areAllItemsNull = enemies.All(x => x == null);
            if (enemies.All(x => x == null))
            {
                Wall.SetActive(false);
            }


            bool areAllEnemiesEnabled = enemies.All(x => x == null || x.activeSelf == false);

            if (enemies.All(x => x == null || x.activeSelf == false))
            {
                int enemiesSpawned = 0;
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i] != null)
                    {
                        enemiesSpawned += 1;
                        enemies[i].SetActive(true);
                    }
                    if (enemiesSpawned > waveSize) break;
                }
            }
        }
    }
}
