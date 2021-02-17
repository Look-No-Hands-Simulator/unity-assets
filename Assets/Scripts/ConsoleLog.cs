using UnityEngine;
using System.Collections;

public class ConsoleLog : MonoBehaviour
{
    //temporary logging system credit to answers.unity.com/questions/1020051/print-debuglog-to-screen-c.html
    string myLog;
    Queue myLogQueue = new Queue();

    void Start()
    {

    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
    }

    void OnGUI()
    {
        GUI.contentColor = Color.white;
        GUILayout.Box(myLog);
        //GUILayout.Label(myLog);
    }
}
