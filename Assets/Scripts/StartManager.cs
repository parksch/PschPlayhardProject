using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : Singleton<StartManager>
{
    protected override void Awake()
    {
        
    }

    public void OnClick()
    {
        SceneManager.LoadScene(1);
    }
}
