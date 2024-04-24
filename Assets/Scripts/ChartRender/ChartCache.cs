using UnityEngine;

public class ChartCache : MonoBehaviour
{
    public static ChartCache Instance { get; private set; }//我不知道有啥用，别删就是了，GPT写的
    public ChartReader.Chart chart { get; set; } = new ChartReader.Chart();//这是缓存
    public bool debugMode { get; set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
