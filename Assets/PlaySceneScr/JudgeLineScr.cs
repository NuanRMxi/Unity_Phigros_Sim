using System.Collections;
using UnityEngine;

public class JudgeLineScr : MonoBehaviour
{
    public ChartReader.JudgeLine judgeLine;
    public double playStartUnixTime;
    public RectTransform rectTransform;
    public int whoami = 0;
    // Start is called before the first frame update
    void Start()
    {
        //调起协程，进行判定线事件读取
        StartCoroutine(judgeLineEventReadAndMove());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 判定线事件读取协程
    /// </summary>
    IEnumerator judgeLineEventReadAndMove()
    {
        int yIndex = 0;
        int xIndex = 0;
        int rotateIndex = 0;
        while (true)
        {
            //获取当前unix时间戳，单位毫秒
            double unixTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            double startToNow = unixTime - playStartUnixTime;
            if (startToNow >= judgeLine.xMoves[xIndex].startTime)
            {
                //设置判定线位置
                StartCoroutine(MoveXOverTime(rectTransform, judgeLine.xMoves[xIndex].startValue, judgeLine.xMoves[xIndex].endValue, (float)(judgeLine.xMoves[xIndex].endTime - judgeLine.xMoves[xIndex].startTime) / 1000));
                xIndex++;
            }
            if (startToNow >= judgeLine.yMoves[yIndex].startTime)
            {
                //设置判定线位置
                StartCoroutine(MoveYOverTime(rectTransform, judgeLine.yMoves[yIndex].startValue, judgeLine.yMoves[yIndex].endValue, (float)(judgeLine.yMoves[yIndex].endTime - judgeLine.yMoves[yIndex].startTime) / 1000));
                yIndex++;
            }
            if (startToNow >= judgeLine.rotateChangeEvents[rotateIndex].startTime)
            {
                //设置判定线角度
                StartCoroutine(RotateOverTime(rectTransform, judgeLine.rotateChangeEvents[rotateIndex].startValue, judgeLine.rotateChangeEvents[rotateIndex].endValue, (float)(judgeLine.rotateChangeEvents[rotateIndex].endTime - judgeLine.rotateChangeEvents[rotateIndex].startTime) / 1000));
                rotateIndex++;
            }
            yield return null;
        }
        
    }
    /// <summary>
    /// 移动X轴
    /// </summary>
    /// <param name="rTf"></param><summary>GameObject的RectTransform</summary>
    /// <param name="startXValue"></param><summary>开始X坐标</summary>
    /// <param name="endXValue"></param><summary>结束X坐标</summary>
    /// <param name="duration"></param><summary>移动时间（单位为秒）</summary>
    IEnumerator MoveXOverTime(RectTransform rTf, float startXValue, float endXValue, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            rTf.anchoredPosition = Vector3.Lerp(new Vector3(startXValue, rTf.anchoredPosition.y), new Vector3(endXValue, rTf.anchoredPosition.y), (Time.time - startTime) / duration);
            yield return null;
        }
        rTf.anchoredPosition = new Vector3(endXValue, rTf.transform.position.y, rTf.transform.position.z);
    }
    /// <summary>
    /// 移动Y轴
    /// </summary>
    /// <param name="rTf"></param><summary>GameObject的RectTransform</summary>
    /// <param name="startXValue"></param><summary>开始Y坐标</summary>
    /// <param name="endXValue"></param><summary>结束Y坐标</summary>
    /// <param name="duration"></param><summary>移动时间（单位为秒）</summary>
    IEnumerator MoveYOverTime(RectTransform rTf, float startYValue, float endYValue, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            rTf.anchoredPosition = Vector3.Lerp(new Vector3(rTf.anchoredPosition.x, startYValue), new Vector3(rTf.anchoredPosition.x, endYValue), (Time.time - startTime) / duration);
            yield return null;
        }
        rTf.anchoredPosition = new Vector3(rTf.anchoredPosition.x, endYValue);
    }

    /// <summary>
    /// 旋转
    /// </summary>
    /// <param name="objectToRotate"></param><summary>要旋转的GameObject的RectTransform</summary>
    /// <param name="startRotate"></param><summary>开始旋转角度</summary>
    /// <param name="endRotate"></param><summary>结束旋转角度</summary>
    /// <param name="duration"></param><summary>旋转时间（单位为秒）</summary>
    IEnumerator RotateOverTime(RectTransform rTf, float startRotate, float endRotate, float duration)
    {
        float startTime = Time.time;
        //float counterClockwiseAngle = (360 - endRotate) % 360;
        while (Time.time < startTime + duration)
        {
            //rTf.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, startRotate), Quaternion.Euler(0, 0, counterClockwiseAngle), (Time.time - startTime) / duration);
            rTf.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, startRotate), Quaternion.Euler(0, 0, endRotate), (Time.time - startTime) / duration);
            yield return null;
        }
        rTf.transform.rotation = Quaternion.Euler(0, 0, endRotate);
    }

}
