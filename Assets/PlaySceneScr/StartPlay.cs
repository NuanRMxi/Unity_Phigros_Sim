



using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static ChartReader;

public class StartPlay : MonoBehaviour
{
    public RectTransform canvasRectTransform;//�������
    public Image Background_Board;//������

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DrawPlayScene(Chart chart)
    {
        //��ȡ��Ļ�ֱ���
        int screenW = Screen.width;
        int screenH = Screen.height;
        //�Էֱ��ʽ��в��еȲ���������ָ���ֱ��ʣ��������������������Ļ������ʹ��16:9�������������
        Screen.SetResolution(1920, 1080, true);
        //���û����Ĵ�С
        canvasRectTransform.sizeDelta = new Vector2(1920, 1080);
        

        




        // �������Ѿ�����һ��RectTransform���󣬴�����Ļ���

        // ���û����Ĵ�С
        //canvasRectTransform.sizeDelta = new Vector2(1920, 1080);

        // �����е�����
        Vector2 scorePosition = new Vector2(0, 0);

        /*
        // �������е�����ӳ�䵽������
        Vector2 canvasPosition = Vector2.Lerp(
            new Vector2(-canvasRectTransform.sizeDelta.x / 2, -canvasRectTransform.sizeDelta.y / 2),
            new Vector2(canvasRectTransform.sizeDelta.x / 2, canvasRectTransform.sizeDelta.y / 2),
            new Vector2((scorePosition.x + 675) / (2 * 675), (scorePosition.y + 450) / (2 * 450))
        );
        */
        // �ֱ�����ֵ����
        float factorX = (scorePosition.x + 675) / (2 * 675);
        float factorY = (scorePosition.y + 450) / (2 * 450);

        // ʹ�ò�ֵ���ӽ�scorePositionӳ�䵽canvasPosition
        Vector2 canvasPosition = Vector2.Lerp(
            new Vector2(-canvasRectTransform.sizeDelta.x / 2, -canvasRectTransform.sizeDelta.y / 2),
            new Vector2(canvasRectTransform.sizeDelta.x / 2, canvasRectTransform.sizeDelta.y / 2),
            factorX
        );
        // �ڻ����������µ�λ��
        RectTransform judgeLineRectTransform; // �������������RectTransform
        
        //noteRectTransform.anchoredPosition = canvasPosition;



        

    }
}
