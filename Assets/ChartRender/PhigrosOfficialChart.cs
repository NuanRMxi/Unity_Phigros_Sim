using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class ChartReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public class ChartEvents
    {
        /// <summary>
        /// X坐标移动事件
        /// </summary>
        public class XMove
        {
            /// <summary>
            /// 开始移动时间，单位毫秒
            /// </summary>
            public double startTime { get; set; }
            /// <summary>
            /// 结束移动时间，单位毫秒
            /// </summary>
            public double endTime { get; set; }
            /// <summary>
            /// 即被移动到的X位置起点
            /// </summary>
            public float startValue { get; set; }
            /// <summary>
            /// 即被移动到的X位置终点
            /// </summary>
            public float endValue { get; set; }
        }
        /// <summary>
        /// Y坐标移动事件
        /// </summary>
        public class YMove
        {
            /// <summary>
            /// 开始移动时间，单位毫秒
            /// </summary>
            public double startTime { get; set; }
            /// <summary>
            /// 结束移动时间，单位毫秒
            /// </summary>
            public double endTime { get; set; }
            /// <summary>
            /// 移动Y位置起点
            /// </summary>
            public float startValue { get; set; }
            /// <summary>
            /// 移动Y位置终点
            /// </summary>
            public float endValue { get; set; }
        }
        /// <summary>
        /// 角度改变事件
        /// </summary>
        public class RotateChangeEvents
        {
            /// <summary>
            /// 旋转开始时间，单位毫秒
            /// </summary>
            public double startTime { get; set; }
            /// <summary>
            /// 旋转结束时间，单位毫秒
            /// </summary>
            public double endTime { get; set; }
            /// <summary>
            /// 即开始时的角度
            /// </summary>
            public float startValue { get; set; }
            /// <summary>
            /// 即结束时的角度
            /// </summary>
            public float endValue { get; set; }
        }
        /// <summary>
        /// 透明度（消失）事件
        /// </summary>
        public class DisappearEvents
        {
            /// <summary>
            /// 透明度改变时间，当前为关键帧实现，所以没有开始与结束，只有发生时间，单位毫秒
            /// </summary>
            public double time { get; set; }
            /// <summary>
            /// 即开始时的不透明度，0是完全透明，1是完全不透明
            /// </summary>
            public float startValue { get; set; }
            /// <summary>
            /// 即被改变到的不透明度，0是完全透明，1是完全不透明
            /// </summary>
            public float endValue { get; set; }
        }
        
    }
    /// <summary>
    /// 一个note
    /// </summary>
    public class Note
    {
        /// <summary>
        /// note类型。1为Tap，2为Drag，3为Hold，4为Flick
        /// </summary>
        public float noteType { get; set; }
        /// <summary>
        /// 实际被开始打击时间，单位为毫秒
        /// </summary>
        public float clickStartTime { get; set; }
        /// <summary>
        /// 如果此note为Hold，此值为Hold的结束时间，单位毫秒，反之，此数值会与clickStartTime相同
        /// </summary>
        public float clickEndTime { get; set; }
        /// <summary>
        /// 下落方向，true时为从上方下落，反之从下方下落
        /// </summary>
        public bool above { get; set; }
        /// <summary>
        /// 速度倍率，越大越快，默认为1，即不加速。实际速度公式为当前speedEventValue * speedMultiplier
        /// </summary>
        public float speedMultiplier { get; set; }
        /// <summary>
        /// note相对于判定线的X位置
        /// </summary>
        public float X { get; set; }
    }

    public class JudgeLine
    {
        /// <summary>
        /// 所有Y移动事件，即使在官谱中，不会出现分离的情况，但是我们先进行拆分处理
        /// </summary>
        public List<ChartEvents.YMove> yMoves { get; set; }
        /// <summary>
        /// 所有X移动事件，即使在官谱中，不会出现分离的情况，但是我们先进行拆分处理
        /// </summary>
        public List<ChartEvents.XMove> xMoves { get; set; }
        /// <summary>
        /// 所有判定线旋转事件
        /// </summary>
        public List<ChartEvents.RotateChangeEvents> rotateChangeEvents { get; set; }
        /// <summary>
        /// 所有不透明度事件
        /// </summary>
        public List<ChartEvents.DisappearEvents> disappearEvents { get; set; }

    }
    public class Chart
    {
        /// <summary>
        /// 谱面版本
        /// </summary>
        public int chartVersion { get; set; }
        /// <summary>
        /// 曲绘
        /// </summary>
        public Image Illustration { get; set; }
        /// <summary>
        /// 判定线数组
        /// </summary>   
        public List<JudgeLine> judgeLines { get; set; }
        /// <summary>
        /// 音乐
        /// </summary>
        public AudioClip music { get; set; }
    }
    /// <summary>
    /// 坐标转换，将官谱中的坐标转换为Re:PhiEdit中的坐标，方便统一计算
    /// </summary>
    public class CoordinateTransformer
    {
        private const float XMin = -675f;
        private const float XMax = 675f;
        private const float YMin = -450f;
        private const float YMax = 450f;
        /// <summary>
        /// 提供官谱X坐标，返回Re:PhiEdit中的X坐标
        /// </summary>
        /// <param name="x"><summary>官谱X坐标</summary></param>
        /// <returns>Re:PhiEdit的X坐标</returns>
        public static float TransformX(float x)
        {
            //return (x - 0) / (1 - 0) * (675 - -675) + -675;
            return x * (XMax - XMin) + XMin;
        }

        /// <summary>
        /// 提供官谱Y坐标，返回Re:PhiEdit中的X坐标
        /// </summary>
        /// <param name="y"><summary>官谱X坐标</summary></param>
        /// <returns>Re:PhiEdit的Y坐标</returns>
        public static float TransformY(float y)
        {
            return y * (YMax - YMin) + YMin;
        }
    }
    public static double CalculateOriginalTime(double T, float bpm)
    {
        double originalTime = (T / bpm) * 1.875;//结果为秒
        originalTime = originalTime * 1000;//转换为毫秒
        //Debug.Log("谱面中原始数据:" + T.ToString() + "\n判定线原始BPM:" + bpm.ToString() + "\n转换结果(毫秒):" + originalTime.ToString());
        return originalTime;//返回
    }

    /// <summary>
    /// 官谱转换方法
    /// </summary>
    /// <param name="ChartFilePath">
    /// <summary>
    /// 官谱文件路径
    /// </summary>
    /// </param>
    static public Chart ChartConvert(string ChartFilePath)
    {
        if (ChartFilePath == null)
        {
            //你他妈给我传了个什么给我
            return null;
        }
        string chartString = File.ReadAllText(ChartFilePath);//读取到字符串
        dynamic chartJsonObject = JsonConvert.DeserializeObject<dynamic>(chartString);//转换为json对象
        Chart chart = new Chart();//创建chart对象
        chart.judgeLines = new List<JudgeLine>();//创建judgeLines列表
        if (chartJsonObject == null)
        {
            //你他妈又导入了个什么给我
            return null;
        }
        else if (chartJsonObject["formatVersion"].ToString() == "3")//检查格式，格式不正确将结束运行
        {
            for (int i = 0; i < chartJsonObject["judgeLineList"].Count; i++)//按照判定线数量运行i次
            {
                JudgeLine judgeLine = new JudgeLine();
                judgeLine.yMoves = new List<ChartEvents.YMove>();//创建yMove列表
                judgeLine.xMoves = new List<ChartEvents.XMove>();//创建xMove列表
                judgeLine.rotateChangeEvents = new List<ChartEvents.RotateChangeEvents>();//创建rotateChangeEvents列表
                //judgeLine.disappearEvents = new List<ChartEvents.DisappearEvents>();//创建disappearEvents列表
                float judgeLineBPM = chartJsonObject["judgeLineList"][i]["bpm"];//读取此判定线BPM，官谱中每条线一个BPM
                for (int moveEventIndex = 0; moveEventIndex < chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"].Count; moveEventIndex++)//读取所有移动事件
                {
                    double startTime = CalculateOriginalTime((double)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["startTime"], judgeLineBPM);//开始时间转换，单位为毫秒
                    double endTime = CalculateOriginalTime((double)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["endTime"], judgeLineBPM);//结束时间转换，单位为毫秒
                    if (startTime <= 0)
                    {
                        startTime = 0;
                    }
                    if (endTime >= 999999)
                    {
                        endTime = startTime;
                    }
                    float xStartValue = CoordinateTransformer.TransformX((float)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["start"]);//读取start为xStartValue
                    float xEndValue = CoordinateTransformer.TransformX((float)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["end"]);//读取end为xEndValue
                    ChartEvents.XMove xMove = new ChartEvents.XMove() {
                        startTime = startTime,
                        endTime = endTime,
                        startValue = xStartValue,
                        endValue = xEndValue
                    };
                    float yStartValue = CoordinateTransformer.TransformY((float)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["start2"]);//读取start2为yStartValue
                    float yEndValue = CoordinateTransformer.TransformY((float)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["end2"]);//读取end2为yEndValue
                    ChartEvents.YMove yMove = new ChartEvents.YMove() { 
                        startTime = startTime,
                        endTime = endTime,
                        startValue = yStartValue,
                        endValue = yEndValue
                    };
                    judgeLine.xMoves.Add(xMove); judgeLine.yMoves.Add(yMove);
                }

                for (int rotateEventIndex = 0; rotateEventIndex < chartJsonObject["judgeLineList"][i]["judgeLineRotateEvents"].Count; rotateEventIndex++)//读取所有角度事件
                {
                    double startTime = CalculateOriginalTime((double)chartJsonObject["judgeLineList"][i]["judgeLineRotateEvents"][rotateEventIndex]["startTime"], judgeLineBPM);//转换开始时间，单位为毫秒
                    double endTime = CalculateOriginalTime((double)chartJsonObject["judgeLineList"][i]["judgeLineRotateEvents"][rotateEventIndex]["endTime"], judgeLineBPM);//转换结束时间，单位为毫秒
                    if (startTime <= 0)
                    {
                        startTime = 0;
                    }
                    if (endTime >= 999999)
                    {
                        endTime = startTime;
                    }
                    float rotateStartValue = (float)chartJsonObject["judgeLineList"][i]["judgeLineRotateEvents"][rotateEventIndex]["start"];//读取start为rotateStartValue
                    float rotateEndValue = (float)chartJsonObject["judgeLineList"][i]["judgeLineRotateEvents"][rotateEventIndex]["end"];//读取end为rotateEndValue
                    ChartEvents.RotateChangeEvents rotateChangeEvents = new ChartEvents.RotateChangeEvents() { 
                        startTime = startTime,
                        endTime = endTime,
                        startValue = rotateStartValue,
                        endValue = rotateEndValue
                    };
                    judgeLine.rotateChangeEvents.Add(rotateChangeEvents);
                }
                chart.judgeLines.Add(judgeLine);
            }
            chart.chartVersion = (int)chartJsonObject["formatVersion"];
            return chart;//返回谱面
        }
        else
        {
            return null;//否则为空，留着报错吧（笑
        }
    }
}
