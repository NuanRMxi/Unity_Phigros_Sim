using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO.Compression;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainBtnScrt : MonoBehaviour
{
    public TMP_Text DebugReadLog;
    public TMP_InputField ChartZipFilePath;
    // Start is called before the first frame update
    void Start()
    {

        //StartCoroutine(RotateImageContinuously());

    }

    /*
    IEnumerator RotateImageContinuously()
    {
        while (true)
        {
            // 每帧旋转Image控件
            背景板.transform.Rotate(0, 0, 50f * Time.deltaTime);
            yield return null;
        }
    }
    */

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClick()
    {
        List<string> log = new List<string>();
        log.Add("开始旅途...");
        try
        {
            log.Add("准备解压文件");
            string TempFilePath;
            //如果环境为Android，获取自身包名。以确定自身专有路径
            if (Application.platform == RuntimePlatform.Android)
            {
                log.Add("看起来是安卓设备，设定缓存目录...");
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
                AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", Application.identifier, 0);
                string packageName = packageInfo.Get<string>("packageName");
                TempFilePath = "/data/data/" + packageName + "/files/ChartTemp";//私有目录地址
                //新建文件夹ChartTemp
                if (!Directory.Exists(TempFilePath))
                {
                    Directory.CreateDirectory(TempFilePath);
                    log.Add("尝试创建了缓存目录...");
                }
                else
                {
                    log.Add("um，目录已存在...");
                }
            }
            else
            {
                log.Add("看起来不是安卓设备，先当Windows设定缓存目录...");
                TempFilePath = Application.persistentDataPath + "/ChartTemp";//那就是Windows了，释放在程序目录
                //TempFilePath = "D:\\ChartTemp";//方便调试
            }

            try
            {

                if (DeleteDirectory(TempFilePath))
                {
                    DebugReadLog.text = "缓存文件已删除";
                    log.Add("新建了又删掉了，看起来权限没问题...");
                }
                else
                {
                    DebugReadLog.text = "可能没有缓存文件...";
                    log.Add("新建失败了？看起来权限有问题...");
                }
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "可能没有缓存文件..." + ex.Message;
                log.Add("这怎么会报错呢...\n" + ex.Message);
            }
            log.Add("缓存部分结束，尝试解压释放文件...");
            //找到并解压文件
            try
            {
                log.Add("开始解压...");
                ZipFile.ExtractToDirectory(ChartZipFilePath.text, TempFilePath);//解压到指定文件夹
                log.Add("解压成功...");
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "解压失败：" + ex.Message;
                UnityEngine.Debug.Log(ex.ToString());
                log.Add("又报错了...\n" + ex.Message);
            }
            log.Add("解压部分结束，尝试读取配置文件...");
            //尝试读取配置文件
            try
            {
                log.Add("读取配置文件...");
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(TempFilePath + "/config.json"));
                log.Add("能读到...\n开始转换...");
                ChartReader.Chart chart = ChartReader.ChartConvert(TempFilePath + "/" + json["Chart"].ToString());
                log.Add("转换结束...");
                AudioClip clip = Resources.Load<AudioClip>("116136");
                //AudioClip clip = Resources.Load<AudioClip>("DHQ");
                log.Add("debug音乐加载正常...");
                //chart.music = Resources.Load<AudioClip>(TempFilePath + "/" + json["Song"].ToString());
                chart.music = clip;
                if (chart.music == null)
                {
                    UnityEngine.Debug.Log("加载出来的音乐是空的...?");
                    log.Add("好吧，是null...");
                }
                ChartCache.Instance.chart = chart;
                log.Add("结束...\n切换场景...");
            }
            catch (Exception ex)
            {
                //出问题输出日志
                DebugReadLog.text = "配置文件读取失败，你写了config.json吗？";
                UnityEngine.Debug.Log(ex.ToString());
                log.Add("um...\n" + ex.Message);
            }
            //切换场景
            SceneManager.LoadScene(1);
            //隐藏按钮
            //gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            DebugReadLog.text = "你大抵是寄了：" + ex.ToString();
            log.Add("致命错误..." + ex.Message);
        }
        //尽量写到本地目录
        //File.WriteAllLines("/storage/emulated/0/Download/Chart/" + "/log.txt", log.ToArray());
    }
    /// <summary>
    /// 删除非空文件夹
    /// </summary>
    /// <param name="path">要删除的文件夹目录</param>
    bool DeleteDirectory(string path)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
            }
            return true;
        }
        catch (Exception)
        {
            return false;
            //throw;
        }
        
    }
}
