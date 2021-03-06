﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : BaseController
{
    public GameObject CharacterObj;
    public GameObject enemyObj;
    public GameObject AvoidAgentObj;
    public GameObject AttackAgentObj;
    public delegate void DeadChecker(bool stat);
    public static event DeadChecker DeadActivation;
    public static event DeadChecker AvoidAgentDeadActivation;
    public static event DeadChecker AttackAgentDeadActivation;

    

    public static GameManager instance;
    int ClickCount;
    private void Awake() {
        if(!instance)
        {
            instance = this;
            Debug.Log("Game Start Now");
        }
    }

    public float reSpawnTime = 3f;
    public float reSpawnRestTime ;

    private bool activeRespawnEnumSwitch;
    private bool EnemyRespawnEnumSwitch;
    private bool AvoidAgentRespawnEnumSwitch;
    private bool AttackAgentRespawnEnumSwitch;

    private float runTime;
    private float runFillGauge = 1f;
    public Image runFillImage;
    public GameObject runBotton;
    
    public Text ScorePan;
    public Text AgentScorePan;
    public GameObject RespawnRestTimePan;
    public Text reSpawnText;

    public Text TimeText;
    private int GameTime = 60;
    public ParticleSystem respawnParc;

    public GameObject EndPanel;
    public Text winText;
    private void Start() {
        DeadActivation += ReSpawnCharacter;
        DeadActivation += ReSpawnEnemy;
        AvoidAgentDeadActivation += ReSpawnAvoidAgent;
        AttackAgentDeadActivation += ReSpawnAttackAgent;


        reSpawnRestTime = reSpawnTime;
        GameTime = 60;
        //TimeChecker(); // 학습환경에서 제외. 실제 게임환경에서 실행
    }

    public void TimeChecker()
    {
        GameTime = 60;
        StartCoroutine(DownTime());
    }
    IEnumerator DownTime()
    {
        while(GameTime > 0)
        {
            GameTime -= 1;
            yield return new WaitForSeconds(1);
        }
        EndPanel.SetActive(true);
        GetWinPanel();
        //GemCollectorAgent.instance.EndEpisode();
        // 학습환경에서 키기
    }
    public void GoMain()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void GetWinPanel()
    {
        if(DataVariables.characterScore > DataVariables.enemyScore)
        {
            winText.text = "Player Win " + " Player Score  " + DataVariables.characterScore.ToString() ;
        }
        else if(DataVariables.characterScore < DataVariables.enemyScore)
        {
            winText.text = "Agent Win "+ " Agent  Score  " + DataVariables.enemyScore.ToString();
        }
    }
    private void TimePanelUpdate() 
    {
        TimeText.text = "Time : " + GameTime.ToString();
    }
    private void CharacterScoreTextUpdate()
    {
        ScorePan.text = "Score : " + DataVariables.characterScore.ToString();
    }
    private void AgentScoreTextUpdate()
    {
        AgentScorePan.text = "Score : " + DataVariables.enemyScore.ToString();
    }
    private void Update() {
        TimePanelUpdate();
        CharacterScoreTextUpdate();
        AgentScoreTextUpdate();
        RunChecker();
        RunGaugeUpdate();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClickCount++;
            if (!IsInvoking("DoubleClick"))
                Invoke("DoubleClick", 1.0f);
        }
        else if (ClickCount == 2)
        {
            CancelInvoke("DoubleClick");
            SaveQuitGame();
        }
    }
    public void RunSwitchOn()
    {
        CharacterController.runSwitch = true;
    }
    private void RunChecker()
    {
        if(CharacterController.runSwitch)
        {
            runBotton.SetActive(false);
            runFillGauge -= Time.deltaTime / 10;
            if(runFillGauge <= 0 )
            {
                CharacterController.runSwitch = false;
            }
        }
        else
        {
            if(runFillGauge <= 1)
            {
                runFillGauge += Time.deltaTime / 5;
            }
            if(runFillGauge >= 1)
            {
                runBotton.SetActive(true);
            }
        }
    }
    private void RunGaugeUpdate()
    {
        runFillImage.fillAmount = runFillGauge;
    }
    public void ReSpawnCharacter(bool stat)
    {
        if(stat)
        {
            activeRespawnEnumSwitch = true;
            if(CharacterController.deathTriggerInt >= 1 )
            {
                StartCoroutine("RespawnTimeDown");
            }
            CharacterObj.transform.position = new Vector3(0,0,0);
            respawnParc.Play();
            Debug.Log("character is dead");
        }
        else
        {
            return;
        }
    }
    public void ReSpawnEnemy(bool stat)
    {
        if(stat)
        {
            EnemyRespawnEnumSwitch = true;
            if(GemCollectorAgent.deathTriggerInt >= 1 )
            {
                StartCoroutine("RespawnTimeEnemyDown");
            }
            enemyObj.transform.position = new Vector3(0,0,0);
            respawnParc.Play();
            Debug.Log("enemy is dead");

        }
        else
        {
            return;
        }
    }
    IEnumerator RespawnTimeEnemyDown()
    {
        while(EnemyRespawnEnumSwitch)
        {
            yield return new WaitForSeconds(3f);
            EnemyRespawnEnumSwitch = false;
            GemCollectorAgent.instance.MoveSwitch = true;
            GemCollectorAgent.instance.isDead = false;
        }
    }

    public void ReSpawnAvoidAgent(bool stat)
    {
        Debug.Log("asdfsadfasf");
        if(stat)
        {
            AvoidAgentRespawnEnumSwitch = true;
            if(AvoidAgentController.instance.deathTriggerInt >= 1 )
            {
                Debug.Log("intintintintint");
                StartCoroutine("RespawnTimeAvoidAgentDown");
            }
            AvoidAgentObj.transform.position = new Vector3(0,0,0);
            respawnParc.Play();
            Debug.Log("enemy is dead");

        }
        else
        {
            return;
        }
    }
    IEnumerator RespawnTimeAvoidAgentDown()
    {
        while(AvoidAgentRespawnEnumSwitch)
        {
            Debug.Log("123123");
            yield return new WaitForSeconds(3f);
            AvoidAgentRespawnEnumSwitch = false;
            AvoidAgentController.instance.MoveSwitch = true;
            AvoidAgentController.instance.isDead = false;
        }
    }

    public void ReSpawnAttackAgent(bool stat)
    {
        if(stat)
        {
            EnemyRespawnEnumSwitch = true;
            if(AttackAgentController.deathTriggerInt >= 1 )
            {
                StartCoroutine("RespawnTimeAttackAgentDown");
            }
            AttackAgentObj.transform.position = new Vector3(0,0,0);
            respawnParc.Play();
            Debug.Log("enemy is dead");

        }
        else
        {
            return;
        }
    }
    IEnumerator RespawnTimeAttackAgentDown()
    {
        while(AttackAgentRespawnEnumSwitch)
        {
            yield return new WaitForSeconds(3f);
            AttackAgentRespawnEnumSwitch = false;
            AttackAgentController.instance.MoveSwitch = true;
            AttackAgentController.instance.isDead = false;
        }
    }
    public void CharacterRespawnTextUpdate()
    {
        reSpawnText.text = "Time : \n"  + Mathf.Round(reSpawnRestTime).ToString();
    }
    IEnumerator RespawnTimeDown()
    {
        reSpawnRestTime = reSpawnTime;
        while(activeRespawnEnumSwitch)
        {
            RespawnRestTimePan.SetActive(true);
            if(RespawnRestTimePan)
            {
                Debug.Log("text update");
                CharacterRespawnTextUpdate();
            }
            if(reSpawnRestTime <= 0f )
            {
                CharacterController.isDead = false;
                reSpawnRestTime = reSpawnTime;
                RespawnRestTimePan.SetActive(false);
                activeRespawnEnumSwitch = false;
                CharacterController.MoveSwitch = true;
                CharacterController.deathTriggerInt = 0;
                break;
            }
            else
            {
                Debug.Log("minus 1");
                reSpawnRestTime -= 1f;
                Debug.Log(reSpawnRestTime);
                Debug.Log("this is rest time");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    
}
