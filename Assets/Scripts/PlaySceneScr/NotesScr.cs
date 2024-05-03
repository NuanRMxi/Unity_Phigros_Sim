using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesScr : MonoBehaviour
{
    public GameObject fatherJudgeLine;
    public float xValue;
    public float speedMultiplier;
    public bool above;
    public double clickStartTime;
    public double clickEndTime;
    public RectTransform noteRectTransform;
    public double playStartUnixTime;
    //private Color noteAlpha;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(y_coordinate_calculation());
        //noteAlpha = GetComponent<Renderer>().material.color;
        noteRectTransform.transform.rotation = fatherJudgeLine.GetComponent<JudgeLineScr>().rectTransform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //实际speed = speed * speedMultiplier，单位为每一个速度单位648像素每秒，根据此公式实时演算相对于判定线的高度（y坐标）
        noteRectTransform.anchoredPosition = new Vector3(xValue, CalculateYPosition(clickStartTime, fatherJudgeLine.GetComponent<JudgeLineScr>().speed * speedMultiplier, (System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds) - playStartUnixTime), noteRectTransform.transform.position.z);       
    }

    public float CalculateYPosition(double targetTime, float speed, double currentTime)
    {
        //检查这是不是hold
        bool isHold = clickStartTime != clickEndTime;

        // 计算已经过去的时间（单位：秒）
        double elapsedTime = currentTime / 1000;

        // 计算目标时间（单位：秒）
        double targetTimeInSeconds = targetTime / 1000;

        // 如果已经过去的时间大于目标时间，那么直接摧毁自己
        if (elapsedTime >= clickEndTime / 1000 && isHold)
        {
            //直接摧毁自己
            Destroy(gameObject);
            return 1200;
        }
        else if (elapsedTime >= targetTimeInSeconds && !isHold)
        {
            //直接摧毁自己
            Destroy(gameObject);
            return 0;
            //noteAlpha.a = 0;
            //GetComponent<Renderer>().material.color = noteAlpha;
        }
        // 根据速度（像素/秒）计算y坐标
        float yPosition = (float)(speed * ((clickStartTime / 1000) - elapsedTime) * 648); // 这里加入了速度单位648像素/秒
        if (isHold)
        {
            yPosition += 1200f;
        }

        if (elapsedTime <= clickEndTime / 1000 && elapsedTime >= targetTimeInSeconds && isHold)
        {
            yPosition = 1200f;
        }


        return yPosition;
    }


}
