using UnityEngine;
using System.Collections;

public class SceneMgr  {

    public static void Load(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
	 
}
