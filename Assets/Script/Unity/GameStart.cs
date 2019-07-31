using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {

    private Game game;
    private KinectPointController kinect;

    void Start () {
        game = GameObject.Find("MainCamera").GetComponent<Game>();
        kinect = GameObject.Find("KinectPointMan").GetComponent<KinectPointController>();
    }
	
	void Update () {
        if (kinect.RaiseRightLeg() == true)
        {
            game.SwitchSatus(Game.GameStatus.gamePlay);
        }
        if (kinect.RaiseLeftLeg() == true)
        {
            game.SwitchSatus(Game.GameStatus.gameNew);
        }
        if (Input.GetKey(KeyCode.H))
        {
            game.SwitchSatus(Game.GameStatus.gamePlay);
        }
        if (Input.GetKey(KeyCode.M))
        {
            game.SwitchSatus(Game.GameStatus.gameNew);
        }
    }
}
