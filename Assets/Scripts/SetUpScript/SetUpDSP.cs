using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SetUpDSP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //设置DSP默认参数为256
        AudioSettings.SetDSPBufferSize(256, 256);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
