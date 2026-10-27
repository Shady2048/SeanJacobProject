using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

enum BattleState { START, PLAYERTURN, ENEMYTURN, ENDWIN, ENDLOSE, ENDPREMATURE };
public class BattleController : MonoBehaviour
{
    BattleState state;
    [Header("Player")]
    public GameObject player;
    [Header("Player Position")]
    public Transform playerPosition;

    [Header("Enemies")]
    public List<GameObject> enemies;
    [Header("Enemy Formations")]
    public Transform[] enemyFormations;

    

    [Header("HP Slider")]
    public Slider HPSlider;
    public TextMeshProUGUI HPText;
    [Header("SP Slider")]
    public Slider SPSlider;
    public TextMeshProUGUI SPText;

    [Header("Enemy Healthbar Prefab")]
    public GameObject enemyHealthbarPrefab;

    [Header("Enemy Canvas")]
    public Canvas enemyCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //HPPanel = canvas.transform.Find("HPPanel").GetComponent<Canvas>();
        //SPPanel = canvas.transform.Find("SPPanel").GetComponent<Canvas>();


        state = BattleState.START;
        SetupBattle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetupBattle()
    {
        player.transform.position = playerPosition.position;
        SetupEnemyPositions();
        CreateEnemyHealthbars();
        UpdateMenus();
    }

    // Sets up enemy positions based on the number of enemies in the battle. The enemy formations are set up in the
    // Unity Editor and are referenced here to position the enemies correctly.
    private void SetupEnemyPositions()  
    {
        switch (enemies.Count) // Manually have to add enemy positions for each formation.
        {
            case 1:
                enemies[0].transform.position = enemyFormations[0].Find("Center").position;
                break;
            case 2:
                enemies[0].transform.position = enemyFormations[1].Find("Top").position;
                enemies[1].transform.position = enemyFormations[1].Find("Bottom").position;
                break;
            case 3:
                enemies[0].transform.position = enemyFormations[0].Find("Top").position;
                enemies[1].transform.position = enemyFormations[1].Find("Center").position;
                enemies[2].transform.position = enemyFormations[2].Find("Bottom").position;
                break;
        }
    }

    private void CreateEnemyHealthbars()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            GameObject enemyHealthbar = Instantiate(enemyHealthbarPrefab, enemyCanvas.transform);
            enemyHealthbar.transform.SetParent(enemyCanvas.transform, false);
            // Position the healthbar above the enemy's position
            enemyHealthbar.transform.position = enemies[i].transform.position - new Vector3(0, 1, 0);
        }
    }

    private void UpdateMenus()
    {
        // Update the HP and SP bars to be accurate to the player's current stats.
        HPSlider.maxValue = player.GetComponent<Entity>().MaxHP;
        HPSlider.minValue = 0;
        HPSlider.value = player.GetComponent<Entity>().HP;

        SPSlider.maxValue = player.GetComponent<Entity>().MaxSP;
        SPSlider.minValue = 0;
        SPSlider.value = player.GetComponent<Entity>().SP;

        // Update the HP and SP text as well
        HPText.text = player.GetComponent<Entity>().HP + "/" + player.GetComponent<Entity>().MaxHP;
        SPText.text = player.GetComponent<Entity>().SP + "/" + player.GetComponent<Entity>().MaxSP;
    }
}
