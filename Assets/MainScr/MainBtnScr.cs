using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO.Compression;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Android;

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
    public void PlayButtonOnClick()
    {
        if (ChartCache.Instance.chart.chartVersion != 0)
        {
            //切换场景
            SceneManager.LoadScene(1);
        }
        else
        {
            DebugReadLog.text = "请先选取并加载文件，否则无法直接加载";
        }
    }

    public void LoadButtonOnClick()
    {
        try
        {
#if UNITY_EDITOR_WIN
            string TempFilePath = Application.dataPath + "/ChartTemp";
#elif UNITY_ANDROID
            RPR:
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
                goto RPR;
            }
            RPW:
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                goto RPW;
            }
            string TempFilePath = Application.persistentDataPath + "/ChartTemp";
#elif UNITY_STANDALONE_WIN
            string TempFilePath = Application.dataPath + "/ChartTemp";
#endif
            try
            {
                if (DeleteDirectory(TempFilePath))
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
                //新建文件夹
                Directory.CreateDirectory(TempFilePath);
                //复制压缩文件到缓存目录
                File.Copy(ChartZipFilePath.text, TempFilePath + "/Chart.zip", true);
                //解压
                ZipFile.ExtractToDirectory(TempFilePath + "/Chart.zip", TempFilePath);
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "解压失败：" + ex.Message;
                UnityEngine.Debug.Log(ex.ToString());
            }
            //读取配置文件
            try
            {
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(TempFilePath + "/config.json"));
                ChartReader.Chart chart = ChartReader.ChartConvert(TempFilePath + "/" + json["Chart"].ToString());
                AudioClip clip = Resources.Load<AudioClip>("116136");
                //AudioClip clip = Resources.Load<AudioClip>("DHQ");
                //chart.music = Resources.Load<AudioClip>(TempFilePath + "/" + json["Song"].ToString());
                chart.music = clip;
                ChartCache.Instance.chart = chart;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.ToString());
            }
        }
        catch (Exception ex)
        {
            DebugReadLog.text = "未归类致命错误：" + ex.ToString();
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
