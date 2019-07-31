using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameNewControl : MonoBehaviour {

    public GameObject pointStartA;
    public GameObject pointEnd;
    public GameObject pointStartB;
    public GameObject infotext;

    private TennisBat canHit;
    private Text infoMessage;
    private Animator animator;
    private Rigidbody rigidbody;
    private Transform transform;
    private KinecAction kinecAction;
    private KinectPointController kinectPointController;
    private GameObject start;
    private GameObject news;
    private Game game;

    private float gravity = 9.8f;
    private float drog;
    float times = 0;
    private float dTime = 0; //控制球做抛物线运动
    public float ShotSpeed = 10;
    private float time = 1;//代表从A点出发到B经过的时长
    public float g = -10;//重力加速度
    private Vector3 speed;//初速度向量
    private Vector3 Gravity;//重力向量
    private Vector3 startPosition;

    //游戏控制
    private bool move = false;
    private bool hitMoving = false;
    private bool left = false;
    private bool right = false;
    void Start () {
        kinectPointController = GameObject.Find("KinectPointMan").GetComponent<KinectPointController>();
        kinecAction = GameObject.Find("B20_Ch_01_Avatar").GetComponent<KinecAction>();
        canHit = GameObject.Find("PlayerTennisBat").GetComponent<TennisBat>();
        infoMessage = GameObject.Find("InfoMessage").GetComponent<Text>();
        animator = GameObject.Find("B20_Ch_01_Avatar").GetComponent<Animator>();
        transform = gameObject.GetComponent<Transform>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
        drog = this.GetComponent<Rigidbody>().drag;
        startPosition = new Vector3(-0.84f, 1.15f, 9.62f);
        transform.position = startPosition;
        game = GameObject.Find("MainCamera").GetComponent<Game>();
        start = GameObject.Find("GameStart");
        news = GameObject.Find("GameNew");
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
        this.transform.position += (speeds + Gravity) * Time.fixedDeltaTime;
    }
    float con_time = 0;
    float DURATION = 2.5f;
    void Update () {
        float temp = con_time;
        temp += 0.01f;
        con_time = temp;
        infoMessage.fontSize = 20;
        infoMessage.color = Color.white;
        if (con_time > 0 && con_time <= 2.5f)
        {
            Fading(con_time, "欢迎进入新手教程", DURATION);
        }
        if(con_time > 2.5f && con_time < 5)
        {
            infoMessage.gameObject.SetActive(true);
            infoMessage.color = Color.red;
            Fading(con_time, "游戏开始时，默认" + "\n" + "为对方发球局，"+ "\n" + "请下蹲开始游戏", DURATION + 2.5f);
        }
        //Debug.Log(con_time);
    }
    float tempTime = 0;
    private void FixedUpdate()
    {
        animator.SetBool("hit", true);
        if (kinectPointController.Squats()) //下蹲
        {
            animator.SetBool("hit", false);
            Quaternion a = new Quaternion(0, 180, 0, 0);
            pointStartA.transform.rotation = a;
            move = true;
        }
        if (Input.GetKey(KeyCode.I))
        {
            animator.SetBool("hit", false);
            Quaternion a = new Quaternion(0, 180, 0, 0);
            pointStartA.transform.rotation = a;
            move = true;
        }

        if (move == true)
        {
            speed = TestTennisMove(startPosition, pointEnd.transform.position);
            left = false;
            right = false;
        }
        //玩家能否回击条件判定
        if (canHit.canHit == true)
        {
            tempTime +=Time.fixedDeltaTime;
            move = false;
            hitMoving = true;
            infoMessage.gameObject.SetActive(true);
            Fading(tempTime, "现在回击该球！", DURATION);
            //infoMessage.text = "现在回击该球";
            //正手挥拍，X轴偏移
            if (Input.GetKey(KeyCode.K))
            {
                left = true;
            }
            if (kinectPointController.TwoWave())
            {
                left = true;
            }
            ////反手挥拍，X轴偏移
            if (kinectPointController.ThreeWave())
            {
                right = true;
            }
        }
        if (kinecAction.robotHit == true)
        {
            canHit.canHit = false;
            speed = TestTennisMove(kinecAction.startPosition, pointEnd.transform.position);
            animator.SetBool("hit", false);
            Quaternion a = new Quaternion(0, 180, 0, 0);
            pointStartA.transform.rotation = a;
        }

        if (hitMoving == true)
        {
            if (left == true)
            {
                tempTime += Time.fixedDeltaTime;
                speed = TestTennisMove(canHit.startPosition, pointEnd.transform.position);
                speed += new Vector3(-1.75f, 0, 0);
                TennisMove(speed);
                Fading(tempTime, "恭喜您获得该球得分！", DURATION);
                Debug.Log(tempTime);
                if (tempTime > 13)
                {
                    //game.SwitchSatus(Game.GameStatus.gameNew);
                    start.SetActive(true);
                    news.SetActive(false);
                }
            }
            if (right == true)
            {
                tempTime += Time.fixedDeltaTime;
                speed = TestTennisMove(canHit.startPosition, pointEnd.transform.position);
                speed += new Vector3(3, 0, 0);
                TennisMove(speed);
                Fading(tempTime, "恭喜您获得该球得分！", DURATION);
                if (tempTime > 13)
                {
                    //game.SwitchSatus(Game.GameStatus.gameNew);
                    start.SetActive(true);
                    news.SetActive(false);
                }
            }
        }
        if (move == true)
        {
            TennisMove(speed);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plane1")
        {
            dTime = 0;
        }
    }
    public void Fading(float time, string text,float DURATION)
    {
        if (time > DURATION)
        {
            infoMessage.gameObject.SetActive(false);
        }
        infoMessage.text = text.Replace("\\n","\n");
        Color newColor = infoMessage.color;
        float proportion = (time / DURATION);
        newColor.a = Mathf.Lerp(1, 0, proportion);
        infoMessage.color = newColor;
    }
}
