using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FruitType
{
    One=0,
    Two=1,
    Three=2,
    Four =3,
    Five=4,
    Six=5,
    Seven=6,
    Eight=7,
    Nine=8,
    Ten=9,
    Eleven=10,
}
//默认为Ready
//待命以及鼠标控制移动位置时为StandyBy
//跌落为Dropping
//碰撞到地板或者其他水果为Collsion

public enum FruitState
{
    Ready=0,  //没有创建出来时
    StandBy = 1,//在待命时
    Dropping = 2,//跌落的过程
    Collision = 3,//碰撞
}

public class Fruits : MonoBehaviour
{
    //Unity,在脚本中Public的变量或者参数，可以在Unity引擎的Inspector检查视图中可视化的修改
    public FruitType fruitType = FruitType.One;
    private bool IsMove = false;
    public FruitState fruitState = FruitState.Ready;
    public float limit_x = 2f;
    public Vector3 originalScale = Vector3.zero;
    public float scaleSpeed = 0.1f;
    public float fruitScore = 1f;

    void Awake() {
        originalScale = new Vector3(1.0f,1.0f,1.0f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //游戏状态是Standby，水果状态也为Standby，可以点击鼠标控制移动，以及松开鼠标跌落
        if (GameManager.gameManagerInstance.gameState == GameState.StandBy && fruitState == FruitState.StandBy)
        {
             if(Input.GetMouseButtonDown(0))
            {
                IsMove = true;
            }
            //松开鼠标
            if(Input.GetMouseButtonUp(0)&&IsMove)
            {
                IsMove = false;
                //改变重力，让水果自行掉落
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                fruitState = FruitState.Dropping;
                GameManager.gameManagerInstance.gameState = GameState.InProgress;
                Debug.Log("gameState:"+GameManager.gameManagerInstance.gameState);
                Debug.Log("fruitobject:"+this);
                //创建新的待命水果
                //GameManager.gameManagerInstance.InvokeCreatFruit(0.5f);
            }
            if(IsMove)
            {
                //移动位置
                //mossePos为屏幕坐标 Input.mousePosition
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//将屏幕坐标转换为unity世界坐标
                //new Vector3(mousePos.x,this.gameObject.GetComponent<Transform>().position.y,this.gameObject.GetComponent<Transform>().position.z)
                this.gameObject.GetComponent<Transform>().position = new Vector3(mousePos.x,this.gameObject.GetComponent<Transform>().position.y,this.gameObject.GetComponent<Transform>().position.z);
            }
            //x方向范围进行一个限制
            if (this.transform.position.x > limit_x)
            {
                this.transform.position = new Vector3(limit_x,this.transform.position.y,this.transform.position.z);
            }
            if (this.transform.position.x < -limit_x)
            {
                this.transform.position = new Vector3(-limit_x,this.transform.position.y,this.transform.position.z);
            }
            
        }
        //尺寸恢复
        if (this.transform.localScale.x < originalScale.x)
        {
            this.transform.localScale += new Vector3(1.0f,1.0f,1.0f)*scaleSpeed;
        }
        if (this.transform.localScale.x > originalScale.x)
        {
            this.transform.localScale = originalScale;
        }
    }
    //碰撞在这里进行检测，会一直不停的执行检测,每个水果身上都会进行碰撞检测
    void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("OnColli");
        if (fruitState == FruitState.Dropping)
        {
            //碰撞到floor
            if (other.gameObject.tag.Contains("Floor"))
            {
                GameManager.gameManagerInstance.gameState = GameState.StandBy;
                fruitState = FruitState.Collision;
                GameManager.gameManagerInstance.hitSource.Play();
                //创建新的待命水果
                GameManager.gameManagerInstance.InvokeCreatFruit(0.5f);
            }
            //碰撞到fruit
            if (other.gameObject.tag.Contains("Fruit"))
            {
                GameManager.gameManagerInstance.gameState = GameState.StandBy;
                fruitState = FruitState.Collision;
                GameManager.gameManagerInstance.hitFruitSource.Play();
                //创建新的待命水果
                GameManager.gameManagerInstance.InvokeCreatFruit(0.5f);
            }
        }
        
        //状态判断
        //Dropping,Collision可以进行合成
        if((int)fruitState >= (int)FruitState.Dropping)
        {
            if (other.gameObject.tag.Contains("Fruit"))
            {
                if (fruitType == other.gameObject.GetComponent<Fruits>().fruitType)
                {   
                    //限制只执行一个水果的碰撞合成方法
                    float thisPosxy = this.transform.position.x + this.transform.position.y;
                    float otherPosxy = other.transform.position.x + other.transform.position.y;
                    if (thisPosxy > otherPosxy)
                    {
                        //合成新的水果，在碰撞的位置生成新的大一号的水果，尺寸由小变大
                        //两个位置信息，fruitType
                        GameManager.gameManagerInstance.CombineNewFruit(fruitType,this.transform.position,other.transform.position);
                        //添加分数，进行更新
                        GameManager.gameManagerInstance.TotalScore += fruitScore;
                        GameManager.gameManagerInstance.totalScore.text = GameManager.gameManagerInstance.TotalScore.ToString();
                        Destroy(this.gameObject);
                        Destroy(other.gameObject);
                        if ((int)fruitType > (int)FruitType.Five)
                        {
                            GameManager.gameManagerInstance.CheerSource.Play();
                        }
                        
                    }
                    
                }
            }
        }
    }
}
