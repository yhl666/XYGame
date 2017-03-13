using UnityEngine;
using System.Collections;

public class ClientTestHost : MonoBehaviour
{
    static ClientTestApp app = null;
    // Use this for initialization
    void Awake()
    {
      ///  GameObject.DontDestroyOnLoad(this);
        if (app == null)
        {
            app = ClientTestApp.ins;
        }
    }

    // Update is called once per frame
    void Update()
    {
        app.UpdateMS();
    }
    void OnDestroy()
    {
       // app.Dispose();
    }
}
