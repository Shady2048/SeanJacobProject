using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene switching

public class StartScreen : MonoBehaviour
{

    public EntityDataSO playerStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        saveState.controls.TransitionScreens.Enable();
        saveState.instance.clearAllEnemyData();
    }

    // Update is called once per frame
    void Update()
    {
        if(saveState.controls.TransitionScreens.enabled){
            if(saveState.controls.TransitionScreens.Start.IsPressed()){
                saveState.returning = false;
                playerStats.HP = playerStats.MaxHP;
                playerStats.SP = playerStats.MaxSP;
                saveState.controls.TransitionScreens.Disable();
                SceneManager.LoadScene("Platformer");
            }
        }
    }
}
