using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using UnityEngine.Localization.Settings;

public class LanguageUpdateScr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LanguageUpdate()
    {
        switch (GetComponent<TMP_Dropdown>().value) 
        {
            case 0:
                //设置语言为中文简体
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                break;
            case 1:
                //设置语言为中文繁体
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                break;
            case 2:
                //设置语言为英文
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[2];
                break;
            default:
                //闪退，数值被恶意篡改
                Application.Quit();
                break;
        }
    }
}
