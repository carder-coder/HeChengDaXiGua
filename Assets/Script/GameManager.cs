using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//默认为Ready
//点击开始到鼠标点击控制水果位置，StandBy;
//松开鼠标水果跌落，Inprogress
//水果跌落碰到地板或者其他水果之后，回滚到StandBy
public enum GameState
{
    Ready=0,  //游戏启动后，没有点击开始前
    StandBy = 1,//点击游戏开始后，水果生成
    InProgress = 2,//水果跌落过程中，完了后到StandBy
    GameOver = 3,//水果超出边界，游戏结束
    CalculateScore = 4,//游戏结束后，延时0.5s计算分数
}

public class GameManager : MonoBehaviour
{
    public GameObject[] fruitList;
    public GameObject bornFruitPosition;
    public GameObject startBtn;

    public GameState gameState = GameState.Ready;

    public float TotalScore = 0f;

    public Text totalScore;//using UnityEngine.UI;

    public Text HighestScore;//using UnityEngine.UI;

    public AudioSource combineSource;
    public AudioSource hitSource;
    public AudioSource CheerSource;
    public AudioSource hitFruitSource;

    public static GameManager gameManagerInstance; //静态的可以直接在别的类使用

    public Vector3 combineScale = new Vector3(0.5f,0.5f,0.5f);
    // Start is called before the first frame update
    void Start()
    {
        gameManagerInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        //游戏开始
        Debug.Log("StartGame");
        //Instantiate()
        CreateFruit();
        float highestScore = PlayerPrefs.GetFloat("HighestScore");
        HighestScore.text = "历史得分：" + highestScore;
        gameState = GameState.StandBy;
        startBtn.SetActive(false);
    }
    public void InvokeCreatFruit(float invokeTime)
    {
        Invoke("CreateFruit",invokeTime);
    }
    public void CreateFruit()
    {
        if ((int)gameState > (int)GameState.InProgress)
        {
            return ;
        }
        int index = Random.Range(0,5);//随机：0,1,2,3,4
        if(fruitList.Length >= index && fruitList[index] != null)
        {
            GameObject fruitObj = fruitList[index];
            var currentFruit = Instantiate(fruitObj,bornFruitPosition.transform.position,fruitObj.transform.rotation);   
            currentFruit.GetComponent<Fruits>().fruitState = FruitState.StandBy;
        }
    }
    //currentFruitType当前碰杠的水果类型
    //currentPos当前水果的位置
    //collisionPos 碰撞的水果的位置
    public void CombineNewFruit(FruitType currentFruitType,Vector3 currentPos,Vector3 collisionPos)
    {
        Vector3 centerPos = (currentPos + collisionPos)/2;
        int index = (int)currentFruitType + 1;
        if(fruitList.Length >= index && fruitList[index] != null)
        {
            GameObject fruitObj = fruitList[index];
            var currentFruit = Instantiate(fruitObj,centerPos,fruitObj.transform.rotation);  
            currentFruit.GetComponent<Rigidbody2D>().gravityScale = 1f; //给合成的水果添加重力
            currentFruit.GetComponent<Fruits>().fruitState = FruitState.Collision;
            currentFruit.transform.localScale = combineScale;

            //音效
            combineSource.Play();
        }
    }
}
