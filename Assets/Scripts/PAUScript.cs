using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

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

    void Update()
    {

    }
    public static double FindClosestAspectRatio(double width, double height, List<double> ratioList)
    {
        double targetRatio = width / height;
        double closestRatio = ratioList.OrderBy(x => Math.Abs(x - targetRatio)).First();
        return closestRatio;
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
