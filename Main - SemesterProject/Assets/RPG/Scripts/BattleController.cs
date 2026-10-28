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
    private List<GameObject> enemyHealthbars;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
        case BattleState.ENEMYTURN:
            enemyTurn();
            break;
        case BattleState.ENDWIN:
            break;
        case BattleState.ENDLOSE:
            break;
        case BattleState.ENDPREMATURE:
            break;
        }
    }

    void SetupBattle()
    {
        player.transform.position = playerPosition.position;
        SetupEnemyPositions();
        //CreateEnemyHealthbars();
        UpdateMenus();

        state = BattleState.PLAYERTURN;
    }

    // Sets up enemy positions based on the number of enemies in the battle. The enemy formations are set up in the
    // Unity Editor and are referenced here to position the enemies correctly.
    private void SetupEnemyPositions()  
    {
        switch (enemies.Count) // Manually have to add enemy positions for each formation.
        {
            case 1: // 1 enemy
                enemies[0].transform.position = enemyFormations[enemies.Count - 1].Find("Center").position;
                break;
            case 2: // 2 enemies
                enemies[0].transform.position = enemyFormations[enemies.Count - 1].Find("Top").position;
                enemies[1].transform.position = enemyFormations[enemies.Count - 1].Find("Bottom").position;
                break;
            case 3:
                enemies[0].transform.position = enemyFormations[enemies.Count - 1].Find("Top").position;
                enemies[1].transform.position = enemyFormations[enemies.Count - 1].Find("Center").position;
                enemies[2].transform.position = enemyFormations[enemies.Count - 1].Find("Bottom").position;
                break;
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

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].GetComponent<EnemyHealthbarController>().UpdateHealthbar(enemies[i].GetComponent<Entity>().HP, enemies[i].GetComponent<Entity>().MaxHP);
            
        }
    }

    public void onFightButtonClicked()
    {
        Debug.Log("Pressed Fight");
        enemies[0].GetComponent<Entity>().Damage(player.GetComponent<Entity>().Attack + Random.Range(-2, 2));
        Debug.Log("Enemy HP: " + enemies[0].GetComponent<Entity>().HP);
        UpdateMenus();
        state= BattleState.ENEMYTURN;
    }
    public void onSkillsButtonClicked()
    {
        Debug.Log("Pressed Skills");
        if(player.GetComponent<Entity>().SP >= 3)
        {
            enemies[0].GetComponent<Entity>().Damage(player.GetComponent<Entity>().Attack * 2 + Random.Range(-2, 2));
            player.GetComponent<Entity>().SP -= 3;
            Debug.Log("Enemy HP: " + enemies[0].GetComponent<Entity>().HP);
            UpdateMenus();
            state = BattleState.ENEMYTURN;
        }
        else
        {
            Debug.Log("Not enough SP");
        }

    }
    public void onItemsButtonClicked()
    {
        Debug.Log("Pressed Items");
        player.GetComponent<Entity>().Heal(8);
        UpdateMenus();
        state = BattleState.ENEMYTURN;
    }
    public void onRunButtonClicked()
    {
        Debug.Log("Pressed Run");
    }

    void enemyTurn()
    {
        player.GetComponent<Entity>().Damage(enemies[0].GetComponent<Entity>().Attack + Random.Range(-2,2));
        UpdateMenus();
        state = BattleState.PLAYERTURN;
    }
}
