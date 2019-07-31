using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tennis : MonoBehaviour
{
    private Game game;
    private Rigidbody rigidbody;
    private Transform transform;
    private KinecT kinecAction;
    private Text robotGame;
    private Text robotSet;
    private Text playerGame;
    private Text playerSet;
    private KinectPointController kinectPointController;
    private PlayerStatus playerStatus = PlayerStatus.robotServe;
    private TennisBat playerHit;
    private Vector3 playerServePos;
    private Animator animator;
    public Transform pointA;
    public Transform pointB;//中间过渡点

    private int serveTurn = 0; //发球权。0表示机器人发球。
    private int hitTurn = 1; //击球权。0表示机器人击球。1表示玩家击球。
    public float ShotSpeed = 10;
    private float time = 1;//代表从A点出发到B经过的时长
    
    public float g = -10;//重力加速度
    private Vector3 speed;//初速度向量
    private Vector3 Gravity;//重力向量

    Vector3 tempPos;
    private float gravity = 9.8f;
    private float drog;
    float times = 0;
    private float dTime = 0; //控制球做抛物线运动
    private bool move = false; //机器人发球后的持续运动标志
    private bool hitMoving = false; //玩家发球后的持续运动标志
    public GameObject particle;
    private Vector3 startPosition;

    void Start()
    {
        
        animator = GameObject.Find("Robot").GetComponent<Animator>();
        playerHit = GameObject.Find("PlayerTennisBats").GetComponent<TennisBat>();
        game = GameObject.Find("MainCamera").GetComponent<Game>();
        kinectPointController = GameObject.Find("KinectPointMan").GetComponent<KinectPointController>();
        kinecAction = GameObject.Find("Robot").GetComponent<KinecT>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
        transform = this.GetComponent<Transform>();
        drog = this.GetComponent<Rigidbody>().drag;
        robotGame = GameObject.Find("RobotGame").GetComponent<Text>();
        robotSet = GameObject.Find("RobotSet").GetComponent<Text>();
        playerGame = GameObject.Find("PlayerGame").GetComponent<Text>();
        playerSet = GameObject.Find("PlayerSet").GetComponent<Text>();
        startPosition = new Vector3(-0.37f, 0.56f, 9.96f);
        playerServePos = new Vector3(0.54f, 0.92f, -7.88f);
        this.transform.position = startPosition;
    }

    private Vector3 TestTennisMove(Vector3 pointAs, Vector3 pointBs)
    {
        time = Vector3.Distance(pointAs, pointBs) / ShotSpeed;
        //通过一个式子计算初速度
        speed = new Vector3((pointBs.x - pointAs.x) / time,
            (pointBs.y - pointAs.y) / time - 0.5f * g * time, (pointBs.z - pointAs.z) / time);
        return speed;
    }
    private void TennisMove(Vector3 speeds)
    {
        Gravity = Vector3.zero;//重力初始速度为0
        Gravity.y = g * (dTime += 1.5f * Time.fixedDeltaTime);//v=at
                                                              //模拟位移
        transform.position += (speeds + Gravity) * Time.fixedDeltaTime;
        //currentAngle.x = -Mathf.Atan((speeds.y + Gravity.y) / speeds.z) * Mathf.Rad2Deg;
        //transform.eulerAngles = currentAngle;
    }
    //小球旋转角度归0
    private void RotateSetZero()
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = new Vector3(0, 0, 0);
        transform.rotation = q;
    }
    private bool leftX = false;
    private bool rightX = false;
    private int robotRandom = 0;
    private bool fa = false;
    private void FixedUpdate()
    {
        animator.SetBool("hit", true);
        this.particle.transform.position = this.transform.position;
        times += Time.fixedDeltaTime;

       // kinectPointController.ThreeWave();
        //机器人发球kinectPointController.Squats()
        if (playerStatus == PlayerStatus.robotServe)
        {
            if (Input.GetKey(KeyCode.W))
            {
                move = true;
            }
            if (kinectPointController.Squats())
            {
                move = true;
            }
        }
        if (move == true)
        {
            rightX = false;
            leftX = false;
            speed = TestTennisMove(startPosition, pointB.position);
            TennisMove(speed);
            hitTurn = 1; //机器人发球之后，轮到玩家击球。
        }
        if (playerStatus == PlayerStatus.serve)
        {
            //move = false;kinectPointController.HandUp()
            if (Input.GetKey(KeyCode.S))
            {
                hitTurn = 0;
                hitMoving = true;
                fa = true;
            }
            if (kinectPointController.HandUp())
            {
                hitTurn = 0;
                hitMoving = true;
                fa = true;

            }
        }
        if (hitMoving == true)
        {
            if (fa == true)
            {
                speed = TestTennisMove(playerServePos, pointB.position);
                TennisMove(speed);
            }
            if (leftX == true)
            {
                int i = 0;
                speed = TestTennisMove(playerServePos, pointB.position);
                speed += new Vector3(-1.75f,0,0);
                TennisMove(speed);
                i++;
                if (i == 1)
                {
                    robotRandom = Random.Range(0, 10);
                    i = 0;
                }
                hitTurn = 0;
                if (robotRandom >= 8)
                {

                }
            }
            if (rightX == true)
            {
                int i = 0;
                speed = TestTennisMove(playerServePos, pointB.position);
                speed += new Vector3(3, 0, 0);
                TennisMove(speed);
                i++;
                if (i == 1)
                {
                    robotRandom = Random.Range(0, 10);
                    i = 0;
                }
                hitTurn = 0;
                if (robotRandom >= 8)
                {

                }
            }
        }
        if (playerHit.canHit == true && hitTurn == 1)
        {
            Debug.Log(hitTurn);
            fa = false;
            move = false;
            hitMoving = true;
            if (Input.GetKey(KeyCode.A))  //左方向X轴
            {
                hitTurn = 0;
                leftX = true;

            }
            if (kinectPointController.TwoWave())
            {
                hitTurn = 0;
                leftX = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                hitTurn = 0;
                rightX = true;
            }
            if (kinectPointController.ThreeWave())
            {
                hitTurn = 0;
                rightX = true;
            }
        }
        //机器人回击
        if (kinecAction.robotHit && hitTurn == 0 && transform.position.z > pointB.position.z)
        {
            playerHit.canHit = false;
            if (robotRandom > 6)
            {
                animator.SetBool("hit", true);
                Quaternion a = new Quaternion(0, 180, 0, 0);
                pointA.transform.rotation = a;
                move = false;
                hitTurn = 1;
            }
            else
            {
                animator.SetBool("hit", false);
                Quaternion a = new Quaternion(0, 180, 0, 0);
                pointA.transform.rotation = a;
                move = true;
                hitMoving = false;
                hitTurn = 1;
            }
            
        }
        //暂停
        if (kinectPointController.Stop() == true)
        {
            move = false;
            hitMoving = false;
        }
        End();//得分
        #region 游戏结束
        if (int.Parse(robotGame.text) >= 7)
        {
            if (int.Parse(robotGame.text) - int.Parse(playerGame.text) >= 2)
            {
                game.SwitchSatus(Game.GameStatus.gameEnd1);
                
            }
            
        }
        if (int.Parse(playerGame.text) >= 7)
        {
            if (int.Parse(playerGame.text) - int.Parse(robotGame.text) >= 2)
            {
                game.SwitchSatus(Game.GameStatus.gameEnd2);
            }
        }
        #endregion
        
    }
    private void End()
    {
        rigidbody.velocity = Vector3.zero;
        RotateSetZero();
        Vector3 endTag = this.transform.position;
        if (endTag.x > 5.32 || endTag.x < -5.32 || endTag.z > 12.71 || endTag.z < -9.7)
        {
            move = false;
            hitMoving = false;
            this.gameObject.SetActive(false);
            if (hitTurn == 0)
            {
                if (playerSet.text == "30")
                {
                    playerSet.text = "40";
                }
                else if (playerSet.text == "40")
                {
                    playerSet.text = "0";
                    robotSet.text = "0";
                    int game = int.Parse(playerGame.text);
                    game += 1;
                    playerGame.text = game.ToString();
                    if (playerStatus == PlayerStatus.robotServe)  //轮到玩家发球
                    {
                        playerStatus = PlayerStatus.serve;
                        //初始化球的位置
                        this.gameObject.SetActive(true);
                        this.transform.position = playerServePos;
                        return;
                    }
                    else //轮到机器人发球
                    {
                        this.gameObject.SetActive(true);
                        this.transform.position = startPosition;
                        playerStatus = PlayerStatus.robotServe;
                        return;
                    }
                }
                else
                {
                    int goal = int.Parse(playerSet.text);
                    goal += 15;
                    playerSet.text = goal.ToString();
                }
            }
            else if (hitTurn == 1)
            {
                if (robotSet.text == "30")
                {
                    robotSet.text = "40";
                }
                else if (robotSet.text == "40")
                {
                    playerSet.text = "0";
                    robotSet.text = "0";
                    int game = int.Parse(robotGame.text);
                    game += 1;
                    robotGame.text = game.ToString();

                    if (playerStatus == PlayerStatus.robotServe)  //轮到玩家发球
                    {
                        move = false;
                        playerHit.canHit = false;
                        playerStatus = PlayerStatus.serve;
                        //初始化球的位置
                        this.gameObject.SetActive(true);
                        this.transform.position = playerServePos;
                        return;
                    }
                    else //轮到机器人发球
                    {
                        move = false;
                        playerHit.canHit = false;
                        this.gameObject.SetActive(true);
                        this.transform.position = startPosition;
                        playerStatus = PlayerStatus.robotServe;
                        return;
                    }
                }
                else
                {
                    int goal = int.Parse(robotSet.text);
                    goal += 15;
                    robotSet.text = goal.ToString();
                }
            }
            
            this.gameObject.SetActive(false);
            if (playerStatus == PlayerStatus.robotServe)
            {
                times = 0;
                move = false;
                hitMoving = false;
                this.gameObject.SetActive(true);
                this.transform.position = startPosition;
                return;
            }
            else
            {
                times = 0;
                move = false;
                hitMoving = false;
                this.gameObject.SetActive(true);
                this.transform.position = playerServePos;
                return;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plane1")
        {
            dTime = 0;
        }
        //结束判定之一
        if (collision.gameObject.tag == "End")
        {
            if (hitTurn == 0) //机器人击的球
            {
                if (robotSet.text == "30")
                {
                    robotSet.text = "40";
                }
                else if (robotSet.text == "40")
                {
                    playerSet.text = "0";
                    robotSet.text = "0";
                    int game = int.Parse(robotGame.text);
                    game += 1;
                    robotGame.text = game.ToString();

                    if (playerStatus == PlayerStatus.robotServe)  //轮到玩家发球
                    {
                        move = false;
                        hitMoving = false;
                        playerStatus = PlayerStatus.serve;
                        //初始化球的位置
                        this.gameObject.SetActive(true);
                        this.transform.position = playerServePos;
                        return;
                    }
                    else
                    {
                        move = false;
                        hitMoving = false;
                        this.gameObject.SetActive(true);
                        this.transform.position = startPosition;
                        playerStatus = PlayerStatus.robotServe;
                        return;
                    }
                }
                else
                {
                    int goal = int.Parse(robotSet.text);
                    goal += 15;
                    robotSet.text = goal.ToString();
                }
            }
            else if (hitTurn == 1)
            {
                if (playerSet.text == "30")
                {
                    playerSet.text = "40";
                }
                else if (playerSet.text == "40")
                {
                    playerSet.text = "0";
                    robotSet.text = "0";
                    int game = int.Parse(playerGame.text);
                    game += 1;
                    playerGame.text = game.ToString();
                    if (playerStatus == PlayerStatus.robotServe)  //轮到玩家发球
                    {
                        move = false;
                        hitMoving = false;
                        playerStatus = PlayerStatus.serve;
                        //初始化球的位置
                        this.gameObject.SetActive(true);
                        this.transform.position = playerServePos;
                        return;
                    }
                    else
                    {
                        move = false;
                        hitMoving = false;
                        this.gameObject.SetActive(true);
                        this.transform.position = startPosition;
                        playerStatus = PlayerStatus.robotServe;
                        return;
                    }
                }
                else
                {
                    int goal = int.Parse(playerSet.text);
                    goal += 15;
                    playerSet.text = goal.ToString();
                }
            }
            this.gameObject.SetActive(false);
            if (playerStatus == PlayerStatus.robotServe)
            {
                times = 0;
                move = false;
                hitMoving = false;
                this.gameObject.SetActive(true);
                this.transform.position = startPosition;
                return;
            }
            else
            {
                times = 0;
                move = false;
                hitMoving = false;
                this.gameObject.SetActive(true);
                this.transform.position = playerServePos;
                return;
            }
        }
    }
    public enum PlayerStatus
    {
        robotServe,
        serve
    }
}
