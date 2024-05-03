using E7.Native;
using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ChartReader;

public class StartPlay : MonoBehaviour
{
    public RectTransform canvasRectTransform;//打击画布
    //public Image Background_Board;//背景板
    public GameObject JudgeLine;//判定线预制件
    public GameObject TapNote;//Tap音符预制件
    public GameObject FlickNote;//Flick音符预制件
    public GameObject DragNote;//Drag音符预制件
    public GameObject HoldNote;//Hold音符预制件
    public TMP_Text TimeReads;
    public bool MusicisPlay = false;
    // Start is called before the first frame update
    // 此方法会在第一帧前调用
    void Start()
    {
        Chart chart = ChartCache.Instance.chart;
        if (chart == null)
        {
            //新建一个Exception，表示缓存是空的
            throw new FileNotFoundException("没有找到文件，缓存为空\nCache is empty");
        }
        else
        {
            //绘制谱面到屏幕
            DrawPlayScene(chart);
        }
        
    }

    // Update is called once per frame
    // 此方法会被每一帧调用
    void Update()
    {
        
    }
    IEnumerator WindowsWaitAndPlay(AudioSource aS, double time)
    {
        while (true)
        {
            TimeReads.text = "NowTime:" + (System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds - time).ToString();
            if (time <= System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds && !MusicisPlay)
            {
                MusicisPlay = true;
                aS.Play();
            }
            yield return null;
        }
    }
    IEnumerator AndroidWaitAndPlay(AudioClip music, double time)
    {
        //预加载音乐
        AudioClip aC = music;
        NativeAudioPointer audioPointer;
        audioPointer = NativeAudio.Load(aC);
        NativeSource nS = new NativeSource();
        while (true)
        {
            TimeReads.text = "NowTime:" + (System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds - time).ToString();
            if (time <= System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds && !MusicisPlay)
            {
                MusicisPlay = true;
                nS.Play(audioPointer);
            }
            yield return null;
        }
    }
    public void DrawPlayScene(Chart chart)
    {

        //获取当前unix时间戳，单位毫秒
        double unixTime = System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
        unixTime = unixTime + 3000f;
        for (int i = 0; i < chart.judgeLineList.Count; i++)
        {
            // 生成判定线实例
            GameObject instance = Instantiate(JudgeLine);
            // 设置父对象为画布
            GameObject parent = GameObject.Find("Canvas");
            instance.transform.SetParent(parent.transform);
            // 设置判定线位置到画布正中间
            instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            // 获取预制件的脚本组件
            JudgeLineScr script = instance.GetComponent<JudgeLineScr>();
            // 设置脚本中的公共变量
            script.playStartUnixTime = unixTime;
            script.judgeLine = chart.judgeLineList[i];
            script.whoami = i;
            //SpriteRenderer sprRend = instance.GetComponent<SpriteRenderer>();
            //sprRend.size = new Vector2(6220.8f, 8.11f);
            for (int noteIndex = 0; noteIndex < chart.judgeLineList[i].noteList.Count; noteIndex++)
            {
                GameObject note;
                switch (chart.judgeLineList[i].noteList[noteIndex].noteType)
                {
                    case 1:
                        note = Instantiate(TapNote);
                        break;
                    case 2:
                        note = Instantiate(DragNote);
                        break;
                    case 3:
                        note = Instantiate(HoldNote);
                        break;
                    case 4:
                        note = Instantiate(FlickNote);
                        break;
                    default:
                        throw new ArgumentException("不是Tap，不是Flick，不是Drag，不是Hold，你是谁？");
                        //break;
                }
                note.transform.SetParent(instance.transform);//这段无法删减，如果此代码被删除，会导致note脱离判定线；如果在Note里设置父物体，则会导致报错null
                NotesScr nScr = note.GetComponent<NotesScr>();
                nScr.above = chart.judgeLineList[i].noteList[noteIndex].above;
                nScr.playStartUnixTime = unixTime;
                nScr.fatherJudgeLine = instance;
                nScr.clickStartTime = chart.judgeLineList[i].noteList[noteIndex].clickStartTime;
                nScr.clickEndTime = chart.judgeLineList[i].noteList[noteIndex].clickEndTime;
                nScr.speedMultiplier = chart.judgeLineList[i].noteList[noteIndex].speedMultiplier;
                nScr.xValue = chart.judgeLineList[i].noteList[noteIndex].X;
            }
        }
#if UNITY_EDITOR
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = chart.music;
        audioSource.loop = false; //控制循环播放
        StartCoroutine(WindowsWaitAndPlay(audioSource, unixTime));
#elif UNITY_ANDROID
        //初始化
        NativeAudio.Initialize();
        //调用方法准备播放
        StartCoroutine(AndroidWaitAndPlay(chart.music, unixTime));
#elif UNITY_STANDALONE_WIN
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = chart.music;
        audioSource.loop = false; //控制循环播放
        StartCoroutine(WindowsWaitAndPlay(audioSource, unixTime));
#endif
    }
}
