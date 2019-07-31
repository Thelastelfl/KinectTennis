using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    private GameStatus gameStatus;
    private GameObject gameStart;
    private GameObject gamePlay;
    private GameObject gameEnd1;
    private GameObject gameEnd2;
    private GameObject gameNew;
    private GameObject gamePlayTennis;

    void Start () {
        //gamePlayTennis = GameObject.Find("Tennis");
        //gamePlayTennis.GetComponent<Tennis>().enabled = false;
        gameStart = GameObject.Find("GameStart");
        gamePlay = GameObject.Find("GamePlay");
        gameEnd1 = GameObject.Find("GameEnd1");
        gameEnd2 = GameObject.Find("GameEnd2");
        gameNew = GameObject.Find("GameNew");

        gameStart.SetActive(true);
        gamePlay.SetActive(false);
        gameEnd1.SetActive(false);
        gameEnd2.SetActive(false);
        gameNew.SetActive(false);
    }
	
	void Update () {
		
	}
    public void SwitchSatus(GameStatus status)
    {
        gameStatus = status;
        if (gameStatus == GameStatus.gamePlay)
        {
            gameStart.SetActive(false);
            gamePlay.SetActive(true);
            gameEnd1.SetActive(false);
            gameEnd2.SetActive(false);
            gameNew.SetActive(false);
        }
        else if (gameStatus == GameStatus.gameEnd1)
        {
            gameStart.SetActive(false);
            gamePlay.SetActive(false);
            gameEnd1.SetActive(true);
            gameEnd2.SetActive(false);
            gameNew.SetActive(false);
        }
        else if (gameStatus == GameStatus.gameEnd2)
        {
            gameStart.SetActive(false);
            gamePlay.SetActive(false);
            gameEnd1.SetActive(false);
            gameEnd2.SetActive(true);
            gameNew.SetActive(false);
        }
        else if (gameStatus == GameStatus.gameNew)
        {
            gameStart.SetActive(false);
            gamePlay.SetActive(false);
            gameEnd1.SetActive(false);
            gameEnd2.SetActive(false);
            gameNew.SetActive(true);
        }
    }
    public enum GameStatus
    {
        gameStart,
        gamePlay,
        gameEnd1,
        gameEnd2,
        gameNew
    }
}
