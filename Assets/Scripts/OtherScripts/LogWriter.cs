using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogWirte : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class LogWriter
    {
        public enum LogType
        {
            Debug,
            Info,
            Warning,
            Error,
            Fatal
        }
        public static void Write(string msg, LogType type)
        {
#if UNITY_EDITOR_WIN
            string LogPath = Application.dataPath + "/Logs.log";
#elif UNITY_STANDALONE_WIN
        string LogPath = Application.dataPath + "/Logs.log";
#elif UNITY_ANDROID
        string LogPath = Application.persistentDataPath + "/Logs.log";
#endif
            //检查文件是否存在，不存在则创建
            if (!System.IO.File.Exists(LogPath))
            {
                using (var stream = System.IO.File.Create(LogPath))
                {
                    // 关闭FileStream对象
                }
            }
            string now = File.ReadAllText(LogPath);
            now = now + "\n";
            switch (type)
            {
                case LogType.Debug:
                    now = now + "[DEBUG-" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + msg;
                    break;
                case LogType.Info:
                    now = now + "[INFO-" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + msg;
                    break;
                case LogType.Warning:
                    Debug.LogWarning(msg);
                    now = now + "[WARN-" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + msg;
                    break;
                case LogType.Error:
                    Debug.LogError(msg);
                    now = now + "[ERROR-" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + msg;
                    break;
                case LogType.Fatal:
                    Debug.LogError(msg);
                    now = now + "[FATAL-" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + msg;
                    break;
            }
            File.WriteAllText(LogPath, now);
        }
    }
}
