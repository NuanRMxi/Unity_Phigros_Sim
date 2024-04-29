using SimpleJSON;
using System.IO;
using TMPro;
using UnityEngine;
using System;
using System.IO.Compression;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using static LogWirte;

public class MainBtnScrt : MonoBehaviour
{
    public TMP_Text DebugReadLog;
    public TMP_InputField ChartZipFilePath;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 1145141919;
    }


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
    public void ToSetUpScene()
    {
        SceneManager.LoadScene(2);
    }
    public void LoadButtonOnClick()
    {
        try
        {
            LogWriter.Write("加载按钮被点击，开始加载流程",LogWriter.LogType.Debug);
#if UNITY_EDITOR_WIN
            string TempFilePath = Application.dataPath + "/ChartTemp";
            LogWriter.Write("检测到当前为WINDOWS_DEBUG环境，当前缓存文件路径为：" + TempFilePath, LogWriter.LogType.Debug);
            //使用预设谱面文件
            //ChartZipFilePath.text = "D:\\PhiOfaChart\\2023sdy.zip";
            ChartZipFilePath.text = "D:\\PhiOfaChart\\A39sdy.zip";
#elif UNITY_ANDROID
            LogWriter.Write("检测当前环境为ANDROID，申请基础权限",LogWriter.LogType.Debug);
            RPR:
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                LogWriter.Write("读权限不足，申请ExternalStorageRead权限",LogWriter.LogType.Debug);
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
                goto RPR;
            }
            LogWriter.Write("已获取读权限",LogWriter.LogType.Debug);
            RPW:
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                LogWriter.Write("写权限不足，申请ExternalStorageWrite权限",LogWriter.LogType.Debug);
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                goto RPW;
            }
            LogWriter.Write("已获取写权限",LogWriter.LogType.Debug);
            string TempFilePath = Application.persistentDataPath + "/ChartTemp";
            LogWriter.Write("检测到当前为ANDROID环境，当前缓存文件路径为：" + TempFilePath,LogWriter.LogType.Debug);
#elif UNITY_STANDALONE_WIN
            string TempFilePath = Application.dataPath + "/ChartTemp";
            LogWriter.Write("检测到当前为WINDOWS环境，当前缓存文件路径为：" + TempFilePath,LogWriter.LogType.Debug);
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
                LogWriter.Write("缓存文件清理完成", LogWriter.LogType.Info);
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "可能没有缓存文件..." + ex.Message;
                LogWriter.Write("删除缓存时出现致命错误，日志为：" + ex.ToString(),LogWriter.LogType.Fatal);
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
                LogWriter.Write("文件释放成功", LogWriter.LogType.Debug);
                
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "解压失败：" + ex.Message;
                UnityEngine.Debug.Log(ex.ToString());
                LogWriter.Write("解压时出现致命错误，日志为：" + ex.ToString(), LogWriter.LogType.Fatal);
            }
            //读取配置文件
            try
            {
                var json = JSON.Parse(File.ReadAllText(TempFilePath + "/config.json"));
                ChartReader.Chart chart = ChartReader.ChartConvert(TempFilePath + "/" + json["Chart"]);
                byte[] data = File.ReadAllBytes(TempFilePath + "/" + json["Song"]);
                AudioClip clip = ToAudioClip(data);
                //AudioClip clip = Resources.Load<AudioClip>("116136");
                clip.LoadAudioData();
                //AudioClip clip = Resources.Load<AudioClip>("DHQ");
                //chart.music = Resources.Load<AudioClip>(TempFilePath + "/" + json["Song"].ToString());
                chart.music = clip;
                ChartCache.Instance.chart = chart;
                LogWriter.Write("读取配置文件成功，谱面转换工作结束，谱面信息：" + chart.chartVersion.ToString() + "，等待加载", LogWriter.LogType.Debug);
            }
            catch (Exception ex)
            {
                LogWriter.Write("读写配置文件时出现致命错误，日志为：" + ex.ToString() + "配置文件路径为：" + TempFilePath + "/config.json", LogWriter.LogType.Fatal);
            }
        }
        catch (Exception ex)
        {
            DebugReadLog.text = "未归类致命错误：" + ex.ToString();
            LogWriter.Write("意外的严重错误，日志为：" + ex.ToString(), LogWriter.LogType.Fatal);
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
    /// <summary>
    /// 读取bytes转为AudioClip
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns>bytes的音频剪辑</returns>
    AudioClip ToAudioClip(byte[] bytes)
    {
        // WAV文件的头部是44字节
        int samples = (bytes.Length - 44) / 2; // 16-bit stereo
        AudioClip clip = AudioClip.Create("MySound", samples, 2, 44100, false);
        float[] data = new float[samples];

        int offset = 44; // WAV头部
        for (int i = 0; i < samples; i++)
        {
            data[i] = (short)(bytes[offset] | bytes[offset + 1] << 8) / 32768.0F;
            offset += 2;
        }
        clip.SetData(data, 0);
        return clip;
    }
}
