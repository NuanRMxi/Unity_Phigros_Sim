using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using EasyUI.Toast;

public class PAUScript : MonoBehaviour
{
    public Canvas rootCanvas;
    public Plane subPlane;
    public RectTransform rectTransform; // 需要适配的Canvas

    void Start()
    {
        //获取rootCanvas的宽和高
        float rootCanvasWidth = rootCanvas.GetComponent<RectTransform>().rect.width;
        float rootCanvasHeight = rootCanvas.GetComponent<RectTransform>().rect.height;
        //宽高比列表
        List<double> rationList = new List<double> { 1.333333333333333, 1.777777777777778, 1.6, 2.333333333333333, 1.5 };
        double toRatio = FindClosestAspectRatio(rootCanvasWidth, rootCanvasHeight, rationList);
        double screenWidth = rootCanvasHeight * toRatio;
        if (screenWidth <= 1000 && screenWidth > 500)
        {
            screenWidth = screenWidth * 2;
        }
        double toWidth = screenWidth - rootCanvasWidth;
        //把screenWindth改为负数
        toWidth = -toWidth;
        rectTransform.sizeDelta = new Vector2((float)toWidth, 0);
        rectTransform.localScale.Set(1, 1, 1);
    }
#if UNITY_EDITOR_WIN
#elif UNITY_STANDALONE_WIN
#elif UNITY_ANDROID
    bool isQuit = false;
#endif

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {

            //获取当前场景标识号
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName == "SetUp")
            {
                SceneManager.LoadScene(0);
            }
#if UNITY_EDITOR_WIN                 
            if (sceneName == "MainScene")
            {
                Toast.Show("调试模式无法使用退出函数QAQ");
            }
#elif UNITY_STANDALONE_WIN
            if (sceneName == "MainScene")
            {
                Toast.Show("很抱歉！请手动退出本程序！QAQ");
            }

#elif UNITY_ANDROID
            if (sceneName == "MainScene" && !isQuit)
            {
                //安卓弹出Toast询问是否退出
                showToast("=v=（再次返回退出程序）");
                isQuit = true;
            }
            else if (sceneName == "MainScene" && isQuit)
            {
                Application.Quit();
            }
#endif
            // 在这里插入你的代码，例如加载场景等
            // 或者你可以直接退出应用：
            // Application.Quit();
        }
    }
    public static double FindClosestAspectRatio(double width, double height, List<double> ratioList)
    {
        double targetRatio = width / height;
        double closestRatio = ratioList.OrderBy(x => Math.Abs(x - targetRatio)).First();
        return closestRatio;
    }
    public void showToast(string toastString)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toast = new AndroidJavaClass("android.widget.Toast");
                AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", toastString);
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaObject toastObject = toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, toast.GetStatic<int>("LENGTH_SHORT"));
                toastObject.Call("show");
            }));
        }
    }

}

public static class CanvasGetAllChildren
{
    public static List<GameObject> GetAllChildren(this GameObject obj)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in obj.transform)
        {
            children.Add(child.gameObject);
        }
        return children;
    }
}
