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
        public class XMove
        {
            /// <summary>
            /// �ƶ�ʱ�䣬��ǰΪ�ؼ�֡ʵ�֣�����û�п�ʼ�������ֻ�з���ʱ�䣬��λ����
            /// </summary>
            public float time { get; set; }
            /// <summary>
            /// �����ƶ�����Xλ��
            /// </summary>
            public float value { get; set; }
        }
        public class YMove
        {
            /// <summary>
            /// �ƶ�ʱ�䣬��ǰΪ�ؼ�֡ʵ�֣�����û�п�ʼ�������ֻ�з���ʱ�䣬��λ����
            /// </summary>
            public float time { get; set; }
            /// <summary>
            /// �����ƶ�����Yλ��
            /// </summary>
            public float value { get; set; }
        }
        public class RotateChangeEvents
        {
            /// <summary>
            /// ��תʱ�䣬��ǰΪ�ؼ�֡ʵ�֣�����û�п�ʼ�������ֻ�з���ʱ�䣬��λ����
            /// </summary>
            public float time { get; set; }
            /// <summary>
            /// ������ת���ĽǶ�
            /// </summary>
            public float value { get; set; }
        }
        public class DisappearEvents
        {
            /// <summary>
            /// ͸���ȸı�ʱ�䣬��ǰΪ�ؼ�֡ʵ�֣�����û�п�ʼ�������ֻ�з���ʱ�䣬��λ����
            /// </summary>
            public float time { get; set; }
            /// <summary>
            /// �����ı䵽�Ĳ�͸���ȣ�0����ȫ͸����1����ȫ��͸��
            /// </summary>
            public float value { get; set; }
        }
        
    }
    /// <summary>
    /// һ��note
    /// </summary>
    public class Note
    {
        /// <summary>
        /// note���͡�1ΪTap��2ΪDrag��3ΪHold��4ΪFlick
        /// </summary>
        public float noteType { get; set; }
        /// <summary>
        /// ʵ�ʱ���ʼ���ʱ�䣬��λΪ����
        /// </summary>
        public float clickStartTime { get; set; }
        /// <summary>
        /// �����noteΪHold����ֵΪHold�Ľ���ʱ�䣬��λ���룬��֮������ֵ����clickStartTime��ͬ
        /// </summary>
        public float clickEndTime { get; set; }
        /// <summary>
        /// ���䷽��trueʱΪ���Ϸ����䣬��֮���·�����
        /// </summary>
        public bool above { get; set; }
        /// <summary>
        /// �ٶȱ��ʣ�Խ��Խ�죬Ĭ��Ϊ1���������١�ʵ���ٶȹ�ʽΪ��ǰspeedEventValue * speedMultiplier
        /// </summary>
        public float speedMultiplier { get; set; }
        /// <summary>
        /// note������ж��ߵ�Xλ��
        /// </summary>
        public float X { get; set; }
    }

    public class JudgeLine
    {
        /// <summary>
        /// ����Y�ƶ��¼�����ʹ�ڹ����У�������ַ������������������Ƚ��в�ִ���
        /// </summary>
        public List<ChartEvents.YMove> yMoves { get; set; }
        /// <summary>
        /// ����X�ƶ��¼�����ʹ�ڹ����У�������ַ������������������Ƚ��в�ִ���
        /// </summary>
        public List<ChartEvents.XMove> xMoves { get; set; }
        /// <summary>
        /// �����ж�����ת�¼�
        /// </summary>
        public List<ChartEvents.RotateChangeEvents> rotateChangeEvents { get; set; }
        /// <summary>
        /// ���в�͸�����¼�
        /// </summary>
        public List<ChartEvents.DisappearEvents> disappearEvents { get; set; }

    }
    public class Chart
    {
        /// <summary>
        /// ����
        /// </summary>
        public Image Illustration { get; set; }
        /// <summary>
        /// �ж�������
        /// </summary>   
        public List<JudgeLine> judgeLines { get; set; }
    }
    /// <summary>
    /// ����ת�����������е�����ת��ΪRe:PhiEdit�е����꣬����ͳһ����
    /// </summary>
    public class CoordinateTransformer
    {
        private const float XMin = -675f;
        private const float XMax = 675f;
        private const float YMin = -450f;
        private const float YMax = 450f;
        /// <summary>
        /// �ṩ����X���꣬����Re:PhiEdit�е�X����
        /// </summary>
        /// <param name="x"><summary>����X����</summary></param>
        /// <returns>Re:PhiEdit��X����</returns>
        public static float TransformX(float x)
        {
            return x * (XMax - XMin) + XMin;
        }
        /// <summary>
        /// �ṩ����Y���꣬����Re:PhiEdit�е�X����
        /// </summary>
        /// <param name="y"><summary>����X����</summary></param>
        /// <returns>Re:PhiEdit��Y����</returns>
        public static float TransformY(float y)
        {
            return y * (YMax - YMin) + YMin;
        }
    }
    public static float CalculateOriginalTime(float T, float bpm)
    {
        float originalTime = (1.875f / bpm) * T;//���Ϊ��
        originalTime = originalTime * 1000f;//ת��Ϊ����
        return originalTime;//����
    }

    /// <summary>
    /// ����ת������
    /// </summary>
    /// <param name="ChartFilePath">
    /// <summary>
    /// �����ļ�·��
    /// </summary>
    /// </param>
    static public void ChartConvert(string ChartFilePath)
    {
        string chartString = File.ReadAllText(ChartFilePath);//��ȡ���ַ���
        dynamic chartJsonObject = JsonConvert.DeserializeObject<dynamic>(chartString);//ת��Ϊjson����
        if (chartJsonObject["formatVersion"].ToString() == "3")//����ʽ����ʽ����ȷ����������
        {
            for (int i = 0; i < chartJsonObject["judgeLineList"].Count; i++)//�����ж�����������i��
            {
                JudgeLine judgeLine = new JudgeLine();
                float judgeLineBPM = chartJsonObject["judgeLineList"][i]["bpm"];//��ȡ���ж���BPM��������ÿ����һ��BPM
                for (int moveEventCount = 0; moveEventCount < chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"].Count; moveEventCount++)//��ȡ�����ƶ��¼�
                {
                    float time = CalculateOriginalTime(chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventCount]["endTime"], judgeLineBPM);//ת����ʱ�䣬ʱ��Ϊ����
                    float xValue = CoordinateTransformer.TransformX(chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventCount]["end"]);//��ʺ���ף��Ҳ��������end��X
                    float yValue = CoordinateTransformer.TransformY(chartJsonObject["judgeLineList"][i]["judgeLineMoveEvents"][moveEventCount]["end2"]);//��ʺ���ף��Ҳ��������end2��Y
                    
                }
            }
        }
        else
        {
            //�ص�MainScene�����Ϊ0
            SceneManager.LoadScene(0);
        }
    }
}
