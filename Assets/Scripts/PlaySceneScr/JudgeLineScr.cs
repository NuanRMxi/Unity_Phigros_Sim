using System.Collections;
using UnityEngine;

public class JudgeLineScr : MonoBehaviour
{
    public ChartReader.JudgeLine judgeLine;
    public double playStartUnixTime;
    public RectTransform rectTransform;
    public int whoami = 0;
    public float speed = 16384f;
    // Start is called before the first frame update
    void Start()
    {
        //调起协程，进行判定线事件读取
        StartCoroutine(judgeLineXEventReadAndMove());
        StartCoroutine(judgeLineYEventReadAndMove());
        StartCoroutine(judgeLineREventReadAndMove());
        StartCoroutine(judgeLineDEventReadAndFade());
        StartCoroutine(judgeLineSEventReadAndFade());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// X轴移动事件读取
    /// </summary>
    IEnumerator judgeLineXEventReadAndMove()
    {
        int Index = 0;
        while (true)
        {
            //获取当前unix时间戳，单位毫秒
            double unixTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            double startToNow = unixTime - playStartUnixTime;
            if (!(Index <= judgeLine.xMoveEventList.Count - 1))
            {
                break;
            }
            else if (startToNow >= judgeLine.xMoveEventList[Index].startTime)
            {
                //设置判定线位置
                StartCoroutine(MoveXOverTime(rectTransform, judgeLine.xMoveEventList[Index].startValue, judgeLine.xMoveEventList[Index].endValue, (float)(judgeLine.xMoveEventList[Index].endTime - judgeLine.xMoveEventList[Index].startTime) / 1000));
                Index++;
            }
            yield return null;
        }
    }
    /// <summary>
    /// Y轴移动事件读取
    /// </summary>
    IEnumerator judgeLineYEventReadAndMove()
    {
        int Index = 0;
        while (true)
        {
            //获取当前unix时间戳，单位毫秒
            double unixTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            double startToNow = unixTime - playStartUnixTime;
            if (!(Index <= judgeLine.yMoveEventList.Count - 1))
            {
                break;
            }
            else if (startToNow >= judgeLine.yMoveEventList[Index].startTime)
            {
                //设置判定线位置
                StartCoroutine(MoveYOverTime(rectTransform, judgeLine.yMoveEventList[Index].startValue, judgeLine.yMoveEventList[Index].endValue, (float)(judgeLine.yMoveEventList[Index].endTime - judgeLine.yMoveEventList[Index].startTime) / 1000));
                Index++;
            }
            yield return null;
        }
    }
    /// <summary>
    /// 旋转事件读取
    /// </summary>
    IEnumerator judgeLineREventReadAndMove()
    {
        int Index = 0;
        while (true)
        {
            //获取当前unix时间戳，单位毫秒
            double unixTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            double startToNow = unixTime - playStartUnixTime;
            if (!(Index <= judgeLine.rotateEventList.Count - 1))
            {
                break;
            }
            else if (startToNow >= judgeLine.rotateEventList[Index].startTime)
            {
                //设置判定线角度
                StartCoroutine(RotateOverTime(rectTransform, judgeLine.rotateEventList[Index].startValue, judgeLine.rotateEventList[Index].endValue, (float)(judgeLine.rotateEventList[Index].endTime - judgeLine.rotateEventList[Index].startTime) / 1000));
                Index++;
            }
            yield return null;
        }
    }
    IEnumerator judgeLineDEventReadAndFade()
    {
        int Index = 0;
        while (true)
        {
            //获取当前unix时间戳，单位毫秒
            double unixTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            double startToNow = unixTime - playStartUnixTime;
            if (!(Index <= judgeLine.alphaEventList.Count - 1))
            {
                break;
            }
            else if (startToNow >= judgeLine.alphaEventList[Index].startTime)
            {
                //使判定线渐变
                StartCoroutine(FadeTo(judgeLine.alphaEventList[Index].startValue, judgeLine.alphaEventList[Index].endValue,(judgeLine.alphaEventList[Index].endTime - judgeLine.alphaEventList[Index].startTime) / 1000));
                Index++;
            }
            yield return null;
        }
    }
    IEnumerator judgeLineSEventReadAndFade()
    {
        int Index = 0;
        while (true)
        {
            //获取当前unix时间戳，单位毫秒
            double unixTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
            double startToNow = unixTime - playStartUnixTime;
            if (!(Index <= judgeLine.speedEventList.Count - 1))
            {
                break;
            }
            else if (startToNow >= judgeLine.speedEventList[Index].startTime)
            {
                //使全局速度改变
                //StartCoroutine(LerpSpeed(judgeLine.speedEventList[Index].startValue, judgeLine.speedEventList[Index].endValue, (judgeLine.speedEventList[Index].endTime - judgeLine.speedEventList[Index].startTime) / 1000));
                speed = judgeLine.speedEventList[Index].startValue;
                Index++;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 移动X轴
    /// </summary>
    /// <param name="rTf"></param><summary>判定线的RectTransform</summary>
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
    /// <param name="rTf"></param><summary>判定线的RectTransform</summary>
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
    /// <param name="objectToRotate"></param><summary>要旋转的判定线的RectTransform</summary>
    /// <param name="startRotate"></param><summary>开始旋转角度</summary>
    /// <param name="endRotate"></param><summary>结束旋转角度</summary>
    /// <param name="duration"></param><summary>旋转时间（单位为秒）</summary>
    IEnumerator RotateOverTime(RectTransform rTf, float startRotate, float endRotate, float duration)
    {
        float startTime = Time.time;
        float deltaAngle = Mathf.DeltaAngle(startRotate, endRotate);
        while (Time.time < startTime + duration)
        {
            //rTf.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, startRotate), Quaternion.Euler(0, 0, counterClockwiseAngle), (Time.time - startTime) / duration);
            rTf.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, startRotate), Quaternion.Euler(0, 0, startRotate + deltaAngle), (Time.time - startTime) / duration);
            yield return null;
        }
        rTf.transform.rotation = Quaternion.Euler(0, 0, endRotate);
    }
    /// <summary>
    /// 透明度
    /// </summary>
    /// <param name="startOpacity"></param><summary>开始时的透明度</summary>
    /// <param name="targetOpacity"></param><summary>结束时的透明度</summary>
    /// <param name="duration"></param><summary>渐变时间（单位为秒）</summary>
    IEnumerator FadeTo(float startOpacity, float targetOpacity, double duration)
    {
        // 计算总时间
        float time = 0;
        while (time < duration)
        {
            // 更新时间
            time += Time.deltaTime;
            // 计算新的透明度
            float newOpacity = Mathf.Lerp(startOpacity, targetOpacity, (float)(time / duration));
            // 设置新的透明度
            Color color = GetComponent<Renderer>().material.color;
            color.a = newOpacity;
            GetComponent<Renderer>().material.color = color;
            yield return null; // 等待下一帧
        }
    }
    /// <summary>
    /// 改变判定线全局流速
    /// </summary>
    /// <param name="startSpeed"></param><summary>开始时的流速</summary>
    /// <param name="endSpeed"></param><summary>结束时的流速</summary>
    /// <param name="duration"></param><summary>变速时长（单位为秒）</summary>
    IEnumerator LerpSpeed(float startSpeed, float endSpeed, double duration)
    {
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float elapsed = Time.time - startTime;
            speed = Mathf.Lerp(startSpeed, endSpeed, (float)(elapsed / duration));
            yield return null;
        }
        // 确保speed变量达到结束数值
        speed = endSpeed;
    }
}
