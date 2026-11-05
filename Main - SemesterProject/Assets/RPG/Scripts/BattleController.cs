using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

enum BattleState { START, PLAYERTURN_START, PLAYERTURN_ACTION, ENEMYTURN_START, ENEMYTURN_ACTION, ENDWIN, ENDLOSE, ENDPREMATURE };
/*
START: First State, used to set up the fight before player is allowed to take action.
PLAYERTURN_START: Player Starts turn.
PLAYERTURN_ACTION: Player has chosen an action, waiting for the action to finish.
ENEMYTURN_START: Enemy Starts turn.
ENEMYTURN_ACTION: Enemy attacks happen
ENDWIN: Player has won the battle.
ENDLOSE: Player has lost the battle.
ENDPREMATURE: Player leaves battle by running, or Enemy has forced player out of battle.
*/
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

    // UI

    [Header("Action Menu")]
    public GameObject actionMenu;

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

    [Header("Battle Update Text")]
    public TextMeshProUGUI battleUpdateText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    // Update is called once per frame
    void Update()
    {
        // enables / disables action menu:
        if (state == BattleState.PLAYERTURN_START) actionMenu.SetActive(true);
        else actionMenu.SetActive(false);

        switch (state)
        {
            case BattleState.PLAYERTURN_START:
                battleUpdateText.text = "What will Player do?";       
                break;
            case BattleState.ENEMYTURN_START:
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

        state = BattleState.PLAYERTURN_START;
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
        (int dmg, bool isDead) dmgResult = enemies[0].GetComponent<Entity>().Damage(player.GetComponent<Entity>().Attack + Random.Range(-2, 2));
        //Debug.Log("Enemy HP: " + enemies[0].GetComponent<Entity>().HP);
        UpdateMenus();
        state= BattleState.PLAYERTURN_ACTION;
        reportBattleUpdate(
            player.GetComponent<Entity>().Name + " attacks " + enemies[0].GetComponent<Entity>().Name + " for " + dmgResult.dmg + " damage!", 
            BattleState.ENEMYTURN_START
            );
    }
    public void onSkillsButtonClicked()
    {
        Debug.Log("Pressed Skills");
        if(player.GetComponent<Entity>().SP >= 3)
        {
            (int dmg, bool isDead) dmgResult = enemies[0].GetComponent<Entity>().Damage(player.GetComponent<Entity>().Attack * 2 + Random.Range(-2, 2));
            player.GetComponent<Entity>().SP -= 3;
            Debug.Log("Enemy HP: " + enemies[0].GetComponent<Entity>().HP);
            UpdateMenus();
            state = BattleState.PLAYERTURN_ACTION;
            reportBattleUpdate(
                player.GetComponent<Entity>().Name + " strongly attacks " + enemies[0].GetComponent<Entity>().Name + " for " + dmgResult.dmg + " damage!", 
                BattleState.ENEMYTURN_START
                );
        }
        else
        {
            Debug.Log("Not enough SP");
        }

    }
    public void onItemsButtonClicked()
    {
        int heal = 8;
        Debug.Log("Pressed Items");
        player.GetComponent<Entity>().Heal(heal);
        reportBattleUpdate(player.GetComponent<Entity>().Name + " healed " + heal + " HP!", BattleState.ENEMYTURN_START);
        UpdateMenus();
        state = BattleState.PLAYERTURN_ACTION;
    }
    public void onRunButtonClicked()
    {
        Debug.Log("Pressed Run");
        //state = BattleState.ENDPREMATURE;
    }

    void enemyTurn()
    {
        (int dmg, bool isDead) dmgResult = player.GetComponent<Entity>().Damage(enemies[0].GetComponent<Entity>().Attack + Random.Range(-2,2));
        UpdateMenus();
        state = BattleState.ENEMYTURN_ACTION;
        reportBattleUpdate(
            enemies[0].GetComponent<Entity>().Name + " attacks " + player.GetComponent<Entity>().Name + " for " + dmgResult.dmg + " damage!", 
            BattleState.PLAYERTURN_START
            );
    }

    void reportBattleUpdate(string update, BattleState nextState)
    {
        battleUpdateText.text = update;
        StartCoroutine(reportBattleUpdateIENUM(nextState));
    }
    private IEnumerator reportBattleUpdateIENUM(BattleState nextState)
    { 
        yield return new WaitForSeconds(2f);
        state = nextState;
    }
}
