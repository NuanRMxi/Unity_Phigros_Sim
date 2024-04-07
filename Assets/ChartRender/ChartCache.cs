using UnityEngine;

public class ChartTemp : MonoBehaviour
{
    public static ChartTemp Instance { get; private set; }
    public ChartReader.Chart chart { get; set; } = new ChartReader.Chart();

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
