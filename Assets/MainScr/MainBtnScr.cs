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
            // ÿ֡��תImage�ؼ�
            ������.transform.Rotate(0, 0, 50f * Time.deltaTime);
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
            //�������ΪAndroid����ȡ�����������ȷ������ר��·��
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
                AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", Application.identifier, 0);
                string packageName = packageInfo.Get<string>("packageName");
                TempFilePath = "/data/data/" + packageName + "/files/ChartTemp";//˽��Ŀ¼��ַ
            }
            else
            {
                TempFilePath = Application.persistentDataPath + "/ChartTemp";//�Ǿ���Windows�ˣ��ͷ��ڳ���Ŀ¼
            }

            try
            {

                if (DeleteDirectory("ChartTemp"))
                {
                    DebugReadLog.text = "�����ļ���ɾ��";
                }
                else
                {
                    DebugReadLog.text = "����û�л����ļ�...";
                }
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "����û�л����ļ�..." + ex.Message;
            }
            
            

            //�ҵ�����ѹ�ļ�
            try
            {
                ZipFile.ExtractToDirectory(ChartZipFilePath.text, TempFilePath);//��ѹ��ָ���ļ���
            }
            catch (Exception ex)
            {
                DebugReadLog.text = "��ѹʧ�ܣ�" + ex.Message;
                throw;
            }

            //���Զ�ȡ�����ļ�
            try
            {
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(TempFilePath + "/config.json"));
                SceneManager.LoadScene(1);
                ChartReader.ChartConvert(TempFilePath + "\\" + json["Chart"].ToString());
                //StartPlay.DrawPlayScene(Chart);
            }
            catch (Exception)
            {
                //�����������־
                DebugReadLog.text = "�����ļ���ȡʧ�ܣ���д��config.json��";
                throw;
            }

            //���ذ�ť
            //gameObject.SetActive(false);
        }
        catch (Exception ex)
        {
            DebugReadLog.text = "�����Ǽ��ˣ�" + ex.ToString();
        }



    }
    /// <summary>
    /// ɾ���ǿ��ļ���
    /// </summary>
    /// <param name="path">Ҫɾ�����ļ���Ŀ¼</param>
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
