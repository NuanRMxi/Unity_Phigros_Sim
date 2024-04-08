using E7.Native;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static ChartReader;

public class StartPlay : MonoBehaviour
{
    public RectTransform canvasRectTransform;//打击画布
    public Image Background_Board;//背景板
    public GameObject JudgeLine;//判定线预制件

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
    public void DrawPlayScene(Chart chart)
    {
        //获取屏幕分辨率。指定屏幕比例，高不变，只改变宽，然后按照16:9的比例进行裁切





        //获取屏幕分辨率
        int screenW = Screen.width;
        int screenH = Screen.height;
        //对分辨率进行裁切等操作，进行指定分辨率，方便适配各种异性形屏幕，这是使用16:9，方便后续调用
        Screen.SetResolution(1920, 1080, true);
        //设置画布的大小
        canvasRectTransform.sizeDelta = new Vector2(1920, 1080);
        

        




        // 假设你已经有了一个RectTransform对象，代表你的画布

        // 设置画布的大小
        //canvasRectTransform.sizeDelta = new Vector2(1920, 1080);

        // 谱面中的坐标
        Vector2 scorePosition = new Vector2(0, 0);

        /*
        // 将谱面中的坐标映射到画布上
        Vector2 canvasPosition = Vector2.Lerp(
            new Vector2(-canvasRectTransform.sizeDelta.x / 2, -canvasRectTransform.sizeDelta.y / 2),
            new Vector2(canvasRectTransform.sizeDelta.x / 2, canvasRectTransform.sizeDelta.y / 2),
            new Vector2((scorePosition.x + 675) / (2 * 675), (scorePosition.y + 450) / (2 * 450))
        );
        */
        // 分别计算插值因子
        float factorX = (scorePosition.x + 675) / (2 * 675);
        float factorY = (scorePosition.y + 450) / (2 * 450);

        // 使用插值因子将scorePosition映射到canvasPosition
        Vector2 canvasPosition = Vector2.Lerp(
            new Vector2(-canvasRectTransform.sizeDelta.x / 2, -canvasRectTransform.sizeDelta.y / 2),
            new Vector2(canvasRectTransform.sizeDelta.x / 2, canvasRectTransform.sizeDelta.y / 2),
            factorX
        );
        // 在画布上设置新的位置
        //RectTransform judgeLineRectTransform; // 这是你的音符的RectTransform

        //noteRectTransform.anchoredPosition = canvasPosition;

        //获取当前unix时间戳，单位毫秒
        float unixTime = (float)System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds;
        unixTime = unixTime + 3000f;
        for (int i = 0; i < chart.judgeLines.Count; i++)
        {
            // 实例化预制件，位置为 (0, 0, 0)，旋转为零旋转
            GameObject instance = Instantiate(JudgeLine, new Vector3(0, 0, 0), Quaternion.identity);

            // 找到父 GameObject
            GameObject parent = GameObject.Find("Canvas");
            // 将实例化的预制件设置为父 GameObject 的子对象
            instance.transform.SetParent(parent.transform);

            // 获取预制件的脚本组件
            JudgeLineScr script = instance.GetComponent<JudgeLineScr>();

            // 设置脚本中的公共变量
            script.playStartUnixTime = unixTime;
            script.judgeLine = chart.judgeLines[i];
        }
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
    }
}
