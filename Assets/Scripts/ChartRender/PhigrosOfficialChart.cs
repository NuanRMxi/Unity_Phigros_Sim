using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using static LogWirte;
using System;
using EasyUI.Toast;

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
        public class XMoveEvent
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
        public class YMoveEvent
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
        public class RotateEvent
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
        /// 不透明度事件
        /// </summary>
        public class AlphaEvent
        {
            /// <summary>
            /// 不透明度改变开始时间
            /// </summary>
            public double startTime { get; set; }
            /// <summary>
            /// 不透明度改变结束时间
            /// </summary>
            public double endTime { get; set; }
            /// <summary>
            /// 即开始时的不透明度，0是完全透明，1是完全不透明
            /// </summary>
            public float startValue { get; set; }
            /// <summary>
            /// 即被改变到的不透明度，0是完全透明，1是完全不透明
            /// </summary>
            public float endValue { get; set; }
        }
        
        public class SpeedEvent
        {
            /// <summary>
            /// 改变速度开始时间
            /// </summary>
            public double startTime { get; set; }
            /// <summary>
            /// 改变速度结束时间
            /// </summary>
            public double endTime { get; set; }
            /// <summary>
            /// 即开始时的速度
            /// </summary>
            public float startValue { get; set; }
            /// <summary>
            /// 即结束时的速度
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
        public int noteType { get; set; }
        /// <summary>
        /// 实际被开始打击时间，单位为毫秒
        /// </summary>
        public double clickStartTime { get; set; }
        /// <summary>
        /// 如果此note为Hold，此值为Hold的结束时间，单位毫秒，反之，此数值会与clickStartTime相同
        /// </summary>
        public double clickEndTime { get; set; }
        /// <summary>
        /// 下落方向，true时为从上方下落，反之从下方下落
        /// </summary>
        public bool above { get; set; }
        /// <summary>
        /// 速度倍率，越大越快，默认为1，即不加速。实际速度公式为当前speedEventValue * speedMultiplier（仅官谱）
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
        /// 所有Y移动事件，官谱中XY事件始终绑定
        /// </summary>
        public List<ChartEvents.YMoveEvent> yMoveEventList { get; set; }
        /// <summary>
        /// 所有X移动事件，官谱中XY事件始终绑定
        /// </summary>
        public List<ChartEvents.XMoveEvent> xMoveEventList { get; set; }
        /// <summary>
        /// 所有判定线旋转事件
        /// </summary>
        public List<ChartEvents.RotateEvent> rotateEventList { get; set; }
        /// <summary>
        /// 所有不透明度事件
        /// </summary>
        public List<ChartEvents.AlphaEvent> alphaEventList { get; set; }
        /// <summary>
        /// 所有速度事件
        /// </summary>
        public List<ChartEvents.SpeedEvent> speedEventList { get; set; }
        /// <summary>
        /// 所有音符
        /// </summary>
        public List<Note> noteList { get; set; }

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
        public List<JudgeLine> judgeLineList { get; set; }
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
        //private const float XMin = -675f;
        //private const float XMax = 675f;
        //private const float YMin = -450f;
        //private const float YMax = 450f;
        private const float XMin = -960f;
        private const float XMax = 960f;
        private const float YMin = -540f;
        private const float YMax = 540f;
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
        try
        {
            if (ChartFilePath == null)
            {
                LogWriter.Write("传入路径为空，无法转换文件，返回null结束转谱程序", LogWriter.LogType.Error);
                return null;
            }
            string chartString = File.ReadAllText(ChartFilePath);//读取到字符串
            var chartJsonObject = JSON.Parse(chartString);
            Chart chart = new Chart();//创建chart对象
            chart.judgeLineList = new List<JudgeLine>();//创建judgeLines列表
            if (chartJsonObject == null)
            {
                LogWriter.Write("传入路径" + ChartFilePath + "序列化后为空，无法转换文件，返回null结束转谱程序", LogWriter.LogType.Error);
                return null;
            }
            else if ((string)chartJsonObject["formatVersion"] == "3")//检查格式，格式不正确将结束运行
            {
                for (int i = 0; i < chartJsonObject["judgeLineList"].Count; i++)//按照判定线数量运行i次
                {
                    JudgeLine judgeLine = new JudgeLine();
                    judgeLine.yMoveEventList = new List<ChartEvents.YMoveEvent>();//创建yMove列表
                    judgeLine.xMoveEventList = new List<ChartEvents.XMoveEvent>();//创建xMove列表
                    judgeLine.rotateEventList = new List<ChartEvents.RotateEvent>();//创建rotateChangeEvents列表
                    judgeLine.alphaEventList = new List<ChartEvents.AlphaEvent>();//创建disappearEvents列表
                    judgeLine.speedEventList = new List<ChartEvents.SpeedEvent>();//创建speedEvent列表
                    judgeLine.noteList = new List<Note>();//创建note列表
                    float judgeLineBPM = (float)chartJsonObject["judgeLineList"][i]["bpm"];//读取此判定线BPM，官谱中每条线一个BPM
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
                            endTime = startTime + 1000;
                        }
                        float xStartValue = CoordinateTransformer.TransformX((float)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["start"]);//读取start为xStartValue
                        float xEndValue = CoordinateTransformer.TransformX((float)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["end"]);//读取end为xEndValue
                        ChartEvents.XMoveEvent xMove = new ChartEvents.XMoveEvent()
                        {
                            startTime = startTime,
                            endTime = endTime,
                            startValue = xStartValue,
                            endValue = xEndValue
                        };
                        float yStartValue = CoordinateTransformer.TransformY((float)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["start2"]);//读取start2为yStartValue
                        float yEndValue = CoordinateTransformer.TransformY((float)chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventIndex]["end2"]);//读取end2为yEndValue
                        ChartEvents.YMoveEvent yMove = new ChartEvents.YMoveEvent()
                        {
                            startTime = startTime,
                            endTime = endTime,
                            startValue = yStartValue,
                            endValue = yEndValue
                        };
                        judgeLine.xMoveEventList.Add(xMove); judgeLine.yMoveEventList.Add(yMove);
                    }
                    LogWriter.Write("第" + (i + 1).ToString() + "线的Move事件转换结束，共有" + judgeLine.xMoveEventList.Count + "个Move事件", LogWriter.LogType.Debug);
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
                            endTime = startTime + 1000;
                        }
                        float rotateStartValue = (float)chartJsonObject["judgeLineList"][i]["judgeLineRotateEvents"][rotateEventIndex]["start"];//读取start为rotateStartValue
                        float rotateEndValue = (float)chartJsonObject["judgeLineList"][i]["judgeLineRotateEvents"][rotateEventIndex]["end"];//读取end为rotateEndValue
                        ChartEvents.RotateEvent rotateChangeEvents = new ChartEvents.RotateEvent()
                        {
                            startTime = startTime,
                            endTime = endTime,
                            startValue = rotateStartValue,
                            endValue = rotateEndValue
                        };
                        judgeLine.rotateEventList.Add(rotateChangeEvents);
                    }
                    LogWriter.Write("第" + (i + 1).ToString() + "线的Rotate事件转换结束，共有" + judgeLine.rotateEventList.Count + "个Rotate事件", LogWriter.LogType.Debug);
                    for (int AlphaEventIndex = 0; AlphaEventIndex < chartJsonObject["judgeLineList"][i]["judgeLineDisappearEvents"].Count; AlphaEventIndex++)
                    {
                        double startTime = CalculateOriginalTime((double)chartJsonObject["judgeLineList"][i]["judgeLineDisappearEvents"][AlphaEventIndex]["startTime"], judgeLineBPM);
                        double endTime = CalculateOriginalTime((double)chartJsonObject["judgeLineList"][i]["judgeLineDisappearEvents"][AlphaEventIndex]["endTime"], judgeLineBPM);
                        if (startTime <= 0)
                        {
                            startTime = 0;
                        }
                        if (endTime >= 999999)
                        {
                            endTime = startTime + 1000;
                        }
                        float alphaStartValue = (float)chartJsonObject["judgeLineList"][i]["judgeLineDisappearEvents"][AlphaEventIndex]["start"];
                        float alphaEndValue = (float)chartJsonObject["judgeLineList"][i]["judgeLineDisappearEvents"][AlphaEventIndex]["end"];
                        ChartEvents.AlphaEvent disappearEvents = new ChartEvents.AlphaEvent()
                        {
                            startTime = startTime,
                            endTime = endTime,
                            startValue = alphaStartValue,
                            endValue = alphaEndValue
                        };
                        judgeLine.alphaEventList.Add(disappearEvents);
                    }
                    LogWriter.Write("第" + (i + 1).ToString() + "线的Alpha事件转换结束，共有" + judgeLine.alphaEventList.Count + "个Alpha事件", LogWriter.LogType.Debug);
                    for (int AlphaEventIndex = 0; AlphaEventIndex < chartJsonObject["judgeLineList"][i]["speedEvents"].Count; AlphaEventIndex++)
                    {
                        double startTime = CalculateOriginalTime((double)chartJsonObject["judgeLineList"][i]["speedEvents"][AlphaEventIndex]["startTime"], judgeLineBPM);
                        double endTime = CalculateOriginalTime((double)chartJsonObject["judgeLineList"][i]["speedEvents"][AlphaEventIndex]["endTime"], judgeLineBPM);
                        if (startTime <= 0)
                        {
                            startTime = 0;
                        }
                        if (endTime >= 999999)
                        {
                            endTime = startTime + 1000;
                        }
                        float speedStartValue = (float)chartJsonObject["judgeLineList"][i]["speedEvents"][AlphaEventIndex]["value"];
                        float speedEndValue = (float)chartJsonObject["judgeLineList"][i]["judgeLineDisappearEvents"][AlphaEventIndex]["value"];
                        ChartEvents.SpeedEvent speedEvent = new ChartEvents.SpeedEvent()
                        {
                            startTime = startTime,
                            endTime = endTime,
                            startValue = speedStartValue,
                            endValue = speedEndValue
                        };
                        judgeLine.speedEventList.Add(speedEvent);
                    }
                    LogWriter.Write("第" + (i + 1).ToString() + "线的Speed事件转换结束，共有" + judgeLine.speedEventList.Count + "个Speed事件", LogWriter.LogType.Debug);
                    for (int AboveNoteIndex = 0; AboveNoteIndex < chartJsonObject["judgeLineList"][i]["notesAbove"].Count; AboveNoteIndex++)
                    {
                        Note note = new Note();
                        note.above = true;
                        var noteJsonObject = chartJsonObject["judgeLineList"][i]["notesAbove"][AboveNoteIndex];
                        if (noteJsonObject["type"] != 3)
                        {
                            note.noteType = (int)noteJsonObject["type"];
                            note.clickStartTime = CalculateOriginalTime((float)noteJsonObject["time"], judgeLineBPM);
                            note.clickEndTime = note.clickStartTime;
                            note.X = (float)noteJsonObject["positionX"] * 108f;
                            note.speedMultiplier = (float)noteJsonObject["speed"];
                        }
                        else
                        {
                            note.noteType = 3;
                            note.clickStartTime = CalculateOriginalTime((float)noteJsonObject["time"], judgeLineBPM);
                            note.clickEndTime = CalculateOriginalTime((float)noteJsonObject["time"], judgeLineBPM) + CalculateOriginalTime((float)noteJsonObject["holdTime"], judgeLineBPM);
                            note.X = (float)noteJsonObject["positionX"] * 108f;
                            note.speedMultiplier = (float)noteJsonObject["speed"];
                        }
                        judgeLine.noteList.Add(note);
                    }
                    LogWriter.Write("第" + (i + 1).ToString() + "线的下落Note转换结束，共有" + judgeLine.noteList.Count + "个下落Note", LogWriter.LogType.Debug);
                    for (int BelowNoteIndex = 0; BelowNoteIndex < chartJsonObject["judgeLineList"][i]["notesBelow"].Count; BelowNoteIndex++)
                    {
                        Note note = new Note();
                        note.above = false;
                        var noteJsonObject = chartJsonObject["judgeLineList"][i]["notesBelow"][BelowNoteIndex];
                        if (noteJsonObject["type"] != 3)
                        {
                            note.noteType = (int)noteJsonObject["type"];
                            note.clickStartTime = CalculateOriginalTime((float)noteJsonObject["time"], judgeLineBPM);
                            note.clickEndTime = note.clickStartTime;
                            note.X = (float)noteJsonObject["positionX"] * 108f;
                            note.speedMultiplier = (float)noteJsonObject["speed"];
                        }
                        else
                        {
                            note.noteType = 3;
                            note.clickStartTime = CalculateOriginalTime((float)noteJsonObject["time"], judgeLineBPM);
                            note.clickEndTime = CalculateOriginalTime((float)noteJsonObject["time"], judgeLineBPM) + CalculateOriginalTime((float)noteJsonObject["holdTime"], judgeLineBPM);
                            note.X = (float)noteJsonObject["positionX"] * 108f;
                            note.speedMultiplier = (float)noteJsonObject["speed"];
                        }
                        judgeLine.noteList.Add(note);
                    }
                    LogWriter.Write("第" + (i + 1).ToString() + "线的上落Note转换结束，共有" + judgeLine.noteList.Count + "个上落Note", LogWriter.LogType.Debug);
                    chart.judgeLineList.Add(judgeLine);
                }
                chart.chartVersion = (int)chartJsonObject["formatVersion"];
                LogWriter.Write("转谱结束，共有" + chart.judgeLineList.Count.ToString() + "条判定线", LogWriter.LogType.Info);
                Toast.Show("转谱结束，共有" + chart.judgeLineList.Count.ToString() + "条判定线");
                return chart;//返回谱面
            }
            else
            {
                LogWriter.Write("要转换的谱面版本不正确或不存在，返回null结束转谱程序", LogWriter.LogType.Error);
                return null;//否则为空，留着报错吧（笑
            }
        }
        catch (Exception ex)
        {
            LogWriter.Write("我没归类，反正结果是这样的：" + ex.ToString(), LogWriter.LogType.Fatal);
            return null;
        }
        
    }
}
