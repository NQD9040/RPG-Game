using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefabs;
    [SerializeField] private float xRange = 6f;
    [SerializeField] private float yRange = 3f;
    [SerializeField] private Canvas selectCanvas;
    private int enemySpawnPerWave = 1;
    private PlayerController playerController;
    private int enemyKillCount = 1;
    private SelectManager select;
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        select = selectCanvas.GetComponent<SelectManager>();
        for (int i = 0; i < enemySpawnPerWave; i++)
        {
            SpawnEnemy();
        }
    }

    void Update()
    {
        if (playerController.getEnemyKillCount() == enemyKillCount)
        {
            enemySpawnPerWave += 1;
            enemyKillCount += enemySpawnPerWave;
            for (int i = 0; i < enemySpawnPerWave; i++)
            {
                SpawnEnemy();
            }
            selectCanvas.gameObject.SetActive(true);
            select.SetOpenSelectTrue();
            Time.timeScale = 0f;
        }   
    }
    void SpawnEnemy()
    {
        float posX = Random.Range(-xRange, xRange);
        float posY = Random.Range(-yRange, yRange);
        Vector3 pos = new Vector3(posX, posY, 0);
        Instantiate(enemyPrefabs, pos, enemyPrefabs.transform.rotation);
    }
}
