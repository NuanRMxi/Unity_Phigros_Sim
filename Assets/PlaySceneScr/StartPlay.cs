using E7.Native;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ChartReader;

public class StartPlay : MonoBehaviour
{
    public RectTransform canvasRectTransform;//打击画布
    public Image Background_Board;//背景板
    public GameObject JudgeLine;//判定线预制件
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

    IEnumerator WaitAndPlay(AudioSource aS, float time)
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

    public void DrawPlayScene(Chart chart)
    {
        //获取当前unix时间戳，单位毫秒
        float unixTime = (float)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
        unixTime = unixTime + 10f;
        for (int i = 0; i < chart.judgeLines.Count; i++)
        {
            // 实例化预制件，位置为 (0, 0, 0)，旋转为零旋转
            GameObject instance = Instantiate(JudgeLine, new Vector3(0, 0, 0), Quaternion.identity);

            // 找到父 GameObject
            GameObject parent = GameObject.Find("Canvas");
            // 将实例化的预制件设置为父 GameObject 的子对象
            instance.transform.SetParent(parent.transform);

            // 设置实例化的预制件的位置
            instance.transform.position = new Vector3(0, 0, 0f);

            // 获取预制件的脚本组件
            JudgeLineScr script = instance.GetComponent<JudgeLineScr>();

            // 设置脚本中的公共变量
            script.playStartUnixTime = unixTime;
            script.judgeLine = chart.judgeLines[i];
        }
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = chart.music;
        audioSource.loop = false; //控制循环播放
        StartCoroutine(WaitAndPlay(audioSource, unixTime));
        return;




#if UNITY_ANDROID
        //初始化
        NativeAudio.Initialize();
        //预加载音乐
        NativeAudioPointer audioPointer;
        audioPointer = NativeAudio.Load(chart.music);
        NativeSource nS = new NativeSource();
        while (true)
        {
            if (unixTime <= System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds)
            {
                break;
            }
        }
        nS.Play(audioPointer);
#elif UNITY_EDITOR_WIN
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = chart.music;
        audioSource.loop = false; //控制循环播放
        while (true)
        {
            if (unixTime <= System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds)
            {
                break;
            }
        }
        audioSource.Play();
#endif
    }
}
