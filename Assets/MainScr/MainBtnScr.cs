using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;
using System.IO.Compression;

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
        try
        {

            string TempFilePath;
            //如果环境为Android，获取自身包名。以确定自身专有路径
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
                AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", Application.identifier, 0);
                string packageName = packageInfo.Get<string>("packageName");
                TempFilePath = "/data/data/" + packageName + "/files/ChartTemp";//私有目录地址
            }
            else
            {
                TempFilePath = Application.persistentDataPath + "/ChartTemp";//那就是Windows了，释放在程序目录
            }

            try
            {

                if (DeleteDirectory("ChartTemp"))
                {
                    DebugReadLog.text = "缓存文件已删除";
                }
                else
                {
                    DebugReadLog.text = "可能没有缓存文件...";
                }
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "可能没有缓存文件..." + ex.Message;
            }
            
            

            //找到并解压文件
            try
            {
                ZipFile.ExtractToDirectory(ChartZipFilePath.text, TempFilePath);//解压到指定文件夹
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "解压失败：" + ex.Message;
                throw;
            }

            //尝试读取配置文件
            try
            {
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(TempFilePath + "/config.json"));
                //SceneManager.LoadScene(1);
                
                //StartPlay.DrawPlayScene(Chart);
                ChartCache.Instance.chart = ChartReader.ChartConvert(TempFilePath + "\\" + json["Chart"].ToString());
            }
            catch (Exception)
            {
                //出问题输出日志
                DebugReadLog.text = "配置文件读取失败，你写了config.json吗？";
                throw;
            }

            //隐藏按钮
            //gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            DebugReadLog.text = "你大抵是寄了：" + ex.ToString();
        }



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
