using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
// hi jacob :)
enum BattleState { START, PLAYERTURN_START, PLAYERTURN_SELECT, PLAYERTURN_ACTION, ENEMYTURN_START, ENEMYTURN_ACTION, ENDWIN, ENDLOSE, ENDPREMATURE };
/*
START: First State, used to set up the fight before player is allowed to take action.
PLAYERTURN_START: Player Starts turn.
PLAYERTURN_SELECT: Player is selecting a target
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

    [Header("Selection Indicator")]
    public GameObject selectionIndicator;   // Triangle for now

    private int selectedEnemyIndex = 0; // Index of the currently selected enemy in the enemies list.

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

    private bool playerActionTaken = false; //just used to make sure the player action function only is called once.

    public List<EntityDataSO> enemyData;

    void Awake(){
        enemyData = saveState.instance.getBattleEnemies();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        saveState.controls.RPG_UI.Enable(); // Make sure to disable RPG controls when this scene finishes.

        
        state = BattleState.START;
        SetupBattle();
    }

    // Update is called once per frame
    void Update()
    {
        // enables / disables action menu:
        if (state == BattleState.PLAYERTURN_START) actionMenu.SetActive(true);
        else actionMenu.SetActive(false);

        // shows/hides selection triangle
        if (state == BattleState.PLAYERTURN_SELECT) selectionIndicator.SetActive(true);
        else selectionIndicator.SetActive(false);

        switch (state)
        {
            case BattleState.PLAYERTURN_START:
                battleUpdateText.text = "What will Player do?";       
                break;

            case BattleState.PLAYERTURN_SELECT:
                selectionIndicator.transform.position = enemies[selectedEnemyIndex].transform.position + new Vector3(-2, 0, 0);
                if(saveState.controls.RPG_UI.enabled)
                {
                    if(saveState.controls.RPG_UI.Down.WasPressedThisFrame())
                    {
                        selectedEnemyIndex--;
                        if (selectedEnemyIndex < 0) selectedEnemyIndex = enemies.Count - 1;
                    }
                    if (saveState.controls.RPG_UI.Up.WasPressedThisFrame())
                    {
                        selectedEnemyIndex++;
                        if (selectedEnemyIndex >= enemies.Count) selectedEnemyIndex = 0;
                    }
                    if (saveState.controls.RPG_UI.Select.WasPressedThisFrame())
                    {
                        state = BattleState.PLAYERTURN_ACTION;
                        playerActionTaken = false; // Reset the player action taken flag for the next turn.

                        Debug.Log("Selected Enemy: " + enemies[selectedEnemyIndex].GetComponent<Entity>().data.Name);
                    }
                    if(saveState.controls.RPG_UI.Return.WasPressedThisFrame())
                    {
                        state = BattleState.PLAYERTURN_START;
                    }

                }
                
                break;
            case BattleState.PLAYERTURN_ACTION:
                if(!playerActionTaken)
                {
                    playerAction();
                    playerActionTaken = true;
                }
                break;
            case BattleState.ENEMYTURN_START:
                if (areAllEnemiesDead())
                {
                    saveState.returning = true;
                    reportBattleUpdate(
                        "You won! :)",
                        BattleState.ENDWIN
                    );
                }
                else
                {
                    enemyTurn();
                }
                break;
            case BattleState.ENDWIN:
                saveState.controls.RPG_UI.Disable();

                SceneManager.LoadScene("Platformer");

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
        HPSlider.maxValue = player.GetComponent<Entity>().data.MaxHP;
        HPSlider.minValue = 0;
        HPSlider.value = player.GetComponent<Entity>().data.HP;

        SPSlider.maxValue = player.GetComponent<Entity>().data.MaxSP;
        SPSlider.minValue = 0;
        SPSlider.value = player.GetComponent<Entity>().data.SP;

        // Update the HP and SP text as well
        HPText.text = player.GetComponent<Entity>().data.HP + "/" + player.GetComponent<Entity>().data.MaxHP;
        SPText.text = player.GetComponent<Entity>().data.SP + "/" + player.GetComponent<Entity>().data.MaxSP;

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].GetComponent<EnemyHealthbarController>().UpdateHealthbar(enemies[i].GetComponent<Entity>().data.HP, enemies[i].GetComponent<Entity>().data.MaxHP);
            
        }
    }

    public void onFightButtonClicked()
    {
        Debug.Log("Pressed Fight");

        state = BattleState.PLAYERTURN_SELECT;
    }
    public void onSkillsButtonClicked()
    {
        Debug.Log("Pressed Skills");
        if(player.GetComponent<Entity>().data.SP >= 3)
        {
            (int dmg, bool isDead) dmgResult = enemies[0].GetComponent<Entity>().Damage(player.GetComponent<Entity>().data.Attack * 2 + Random.Range(-2, 2));
            player.GetComponent<Entity>().data.SP -= 3;
            Debug.Log("Enemy HP: " + enemies[0].GetComponent<Entity>().data.HP);
            UpdateMenus();
            state = BattleState.PLAYERTURN_ACTION;
            reportBattleUpdate(
                player.GetComponent<Entity>().data.Name + " strongly attacks " + enemies[0].GetComponent<Entity>().data.Name + " for " + dmgResult.dmg + " damage!", 
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
        reportBattleUpdate(player.GetComponent<Entity>().data.Name + " healed " + heal + " HP!", BattleState.ENEMYTURN_START);
        UpdateMenus();
        state = BattleState.PLAYERTURN_ACTION;
    }
    public void onRunButtonClicked()
    {
        Debug.Log("Pressed Run");
        //state = BattleState.ENDPREMATURE;
    }

    void playerAction()
    {
        (int dmg, bool isDead) dmgResult = enemies[selectedEnemyIndex].GetComponent<Entity>().Damage(player.GetComponent<Entity>().data.Attack + Random.Range(-2, 2));
        Debug.Log("Enemy HP: " + enemies[selectedEnemyIndex].GetComponent<Entity>().data.HP);
        UpdateMenus();
        state = BattleState.PLAYERTURN_ACTION;
        reportBattleUpdate(
            player.GetComponent<Entity>().data.Name + " attacks " + enemies[selectedEnemyIndex].GetComponent<Entity>().data.Name + " for " + dmgResult.dmg + " damage!",
            BattleState.ENEMYTURN_START
            );
    }

    void enemyTurn()
    {
        (int dmg, bool isDead) dmgResult = player.GetComponent<Entity>().Damage(enemies[0].GetComponent<Entity>().data.Attack + Random.Range(-2,2));
        UpdateMenus();
        state = BattleState.ENEMYTURN_ACTION;
        reportBattleUpdate(
            enemies[0].GetComponent<Entity>().data.Name + " attacks " + player.GetComponent<Entity>().data.Name + " for " + dmgResult.dmg + " damage!", 
            BattleState.PLAYERTURN_START
            );
    }

    bool areAllEnemiesDead()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Entity>().data.HP > 0)
            {
                return false;
            }
        }
        return true;
    }

    public EntityDataSO getEnemyData()
    {
        for(int i = 0; i < enemyData.Count; ++i)
        {
            Debug.Log("Checking enemy data: " + enemyData[i].Name);
            if (enemyData[i].isInBattle)
            {
                enemyData[i].isInBattle = false;  
                return enemyData[i];
            }
        }
        return null;
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
