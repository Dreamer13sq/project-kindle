using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MasterObject
{
    public static bool isPaused = false;
    private int currentTab = 1; //0 = Goals, 1 = Weapons, 2 = Enemies
    private int currentEnemyPage = 1;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject weaponPage;
    [SerializeField] GameObject weaponPage1;
    [SerializeField] GameObject weaponPage2;
    [SerializeField] GameObject goalsPage;
    [SerializeField] GameObject enemiesPage;
    [SerializeField] GameObject enemiesPage1;
    [SerializeField] GameObject enemiesPage2;
    [SerializeField] GameObject enemiesPage3;
    [SerializeField] GameObject weaponsButtonPreFab;
    [SerializeField] GameObject goalsButtonPreFab;
    [SerializeField] GameObject enemiesButtonPreFab;
    [SerializeField] UnityEngine.UI.Image[] arrowsprites;
    private Button weaponsButton;
    private Button goalsButton;
    private Button enemiesButton;
    public TMP_Text goalsText;
    private float statestep;
    private List<float> arrowystart = new List<float>();
    public List<string> objectives = new List<string>();

    bool leftHolding = false;
    bool rightHolding = false;
    bool upHolding = false;
    bool downHolding = false;

    void Start() {
        int len = arrowsprites.Length;
        for (int i = 0; i < len; i++)
        {
            arrowystart.Add(arrowsprites[i].transform.localPosition.y);
        }
        currentTab = 1;

        // Reset all pages
        goalsPage.SetActive(currentTab==0);
        weaponPage.SetActive(currentTab==1);
        enemiesPage.SetActive(currentTab==2);

        weaponsButton = weaponsButtonPreFab.GetComponent<Button>();
        goalsButton = goalsButtonPreFab.GetComponent<Button>();
        enemiesButton = enemiesButtonPreFab.GetComponent<Button>();

        //Populate objectives with starting goals
        objectives.Add("Destroy the 5 effigies located around the map.");
        objectives.Add("Collect all 3 health upgrades.");
        objectives.Add("Obtain the Meteor weapon.");
    }

    // Update is called once per frame
    void Update()
    {
        //input vars
        float xaxis = Input.GetAxisRaw("Horizontal");
        float yaxis = Input.GetAxisRaw("Vertical");

        //when the player stops holding a direction, reset the holding variable
        if (xaxis > -0.5)
            leftHolding = false;
        if (xaxis < 0.5)
            rightHolding = false;
        if (yaxis > -0.5)
            downHolding = false;
        if (yaxis < 0.5)
            upHolding = false;
        
        // Exit 
        if (game.EventIsRunning()) {return;}

        // Enter pause menu
        if(Input.GetButtonDown("Menu") || Input.GetKeyDown("return"))
        {
            if(isPaused)
            {
                Resume();
                game.GameFlagClear(GameFlag.pause);
                game.GameFlagClear(GameFlag.lock_player);
            }
            else
            {
                Pause();
                game.GameFlagSet(GameFlag.lock_player);
                game.GameFlagSet(GameFlag.pause);
            }
        }

        // Exit if not paused
        if (!isPaused) {return;}

        //Goals Page
        if(currentTab == 0)
        {
            goalsButton.Select();
        }

        //Weapons Page
        else if(currentTab == 1)
        {
            weaponsButton.Select();
            if(yaxis < -0.5)
            {
                weaponPage1.SetActive(false);
                weaponPage2.SetActive(true);
                statestep = 0.0f;
            }

            else if(yaxis > 0.5)
            {
                weaponPage2.SetActive(false);
                weaponPage1.SetActive(true);
                statestep = 0.0f;
            }
        }

        //Enemies Page
        else if(currentTab == 2)
        {
            enemiesButton.Select();
            if(yaxis < -0.5 && downHolding == false)
            {
                downHolding = true;
                if(currentEnemyPage == 1)
                {
                    enemiesPage1.SetActive(false);
                    enemiesPage2.SetActive(true);
                    currentEnemyPage = 2;
                }
                else if(currentEnemyPage == 2)
                {
                    enemiesPage2.SetActive(false);
                    enemiesPage3.SetActive(true);
                    currentEnemyPage = 3;
                }
            }

            if(yaxis > 0.5 && upHolding == false)
            {
                upHolding = true;
                if(currentEnemyPage == 2)
                {
                    enemiesPage1.SetActive(true);
                    enemiesPage2.SetActive(false);
                    currentEnemyPage = 1;
                }
                else if(currentEnemyPage == 3)
                {
                    enemiesPage2.SetActive(true);
                    enemiesPage3.SetActive(false);
                    currentEnemyPage = 2;
                }
            }
        }

        //navigate to the correct page
        if(xaxis < -0.5 && leftHolding == false)
        {
            leftHolding = true;
            //From weapons page, go to the goals page
            if(currentTab == 1)
            {
                weaponPage.SetActive(false);
                goalsPage.SetActive(true);
                currentTab = 0;
            }

            //from enemies page, go to weapons page
            else if(currentTab == 2)
            {
                enemiesPage.SetActive(false);
                weaponPage.SetActive(true);
                currentTab = 1;
            }
        }

        if(xaxis > 0.5 && rightHolding == false)
        {
            rightHolding = true;
            //From weapons page, go to enemies page
            if(currentTab == 1)
            {
                weaponPage.SetActive(false);
                enemiesPage.SetActive(true);
                currentTab = 2;
            }
            //From goals page, go to weapons page
            if(currentTab == 0)
            {
                goalsPage.SetActive(false);
                weaponPage.SetActive(true);
                currentTab = 1;
            }
        }

        // Arrow bobbing
        statestep += 0.1f;
        int index = 0;
        foreach(UnityEngine.UI.Image arrow in arrowsprites)
        {
            arrow.transform.localPosition = new Vector2(
                arrow.transform.localPosition.x,
                arrowystart[index] + Mathf.Sin(statestep)*4.0f
            );
            index++;
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        statestep = 0.0f;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        statestep = 0.0f;

        //Rewrite goals everytime game is paused so it is always up to date
        goalsText.text = ""; //clear text
        foreach(string goal in objectives)
        {
            goalsText.text += "- " + goal + "\n";
        }
    }

    //Mouse Controls
    public void OnClickGoals(){
        currentTab = 0;
        weaponPage.SetActive(false);
        enemiesPage.SetActive(false);
        goalsPage.SetActive(true);
    }

    public void OnClickWeapons(){
        currentTab = 1;
        weaponPage.SetActive(true);
        enemiesPage.SetActive(false);
        goalsPage.SetActive(false);
    }

    public void OnClickEnemies(){
        currentTab = 2;
        weaponPage.SetActive(false);
        enemiesPage.SetActive(true);
        goalsPage.SetActive(false);
    }

    public void CompleteObjective(int goalIndex)
    {
        //objectives.RemoveAt(goalIndex);
    }
}
