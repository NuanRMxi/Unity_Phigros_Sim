using System.Collections;
using UnityEngine;

public class JudgeLineScr : MonoBehaviour
{
    public ChartReader.JudgeLine judgeLine;
    public float playStartUnixTime;
    public RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(judgeLineEventReadAndMove());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    IEnumerator judgeLineEventReadAndMove()
    {
        Debug.Log("获取成功，第一个旋转事件的time为:" + judgeLine.rotateChangeEvents[0].time);
        int yIndex = 0;
        int xIndex = 0;
        int rotateIndex = 0;
        while (true)
        {
            //获取当前unix时间戳，单位毫秒
            float unixTime = (float)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            float startToNow = unixTime - playStartUnixTime;
            if (startToNow >= judgeLine.xMoves[xIndex].time)
            {
                //设置判定线位置
                //transform.position = new Vector3(judgeLine.xMoves[xIndex].value, transform.position.y, transform.position.z);
                rectTransform.anchoredPosition = new Vector3(judgeLine.xMoves[xIndex].value, transform.position.y, transform.position.z);
                xIndex++;
            }
            if (startToNow >= judgeLine.yMoves[yIndex].time)
            {
                //设置判定线位置
                //transform.position = new Vector3(transform.position.x, judgeLine.yMoves[yIndex].value, transform.position.z);
                rectTransform.anchoredPosition = new Vector3(transform.position.x, judgeLine.yMoves[yIndex].value, transform.position.z);
                yIndex++;
            }
            if (startToNow >= judgeLine.rotateChangeEvents[rotateIndex].time)
            {
                //设置判定线角度
                //transform.rotation = Quaternion.Euler(0, 0, judgeLine.rotateChangeEvents[rotateIndex].value);
                rectTransform.rotation = Quaternion.Euler(0, 0, judgeLine.rotateChangeEvents[rotateIndex].value);
                rotateIndex++;
            }
            yield return null;
        }
    }

}
