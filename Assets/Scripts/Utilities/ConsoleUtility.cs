using System;
using System.Reflection;
using UnityEngine;

public static class ConsoleUtility
{
    // https://stackoverflow.com/questions/40577412/clear-editor-console-logs-from-script
    public static void ClearLog() {
#if UNITY_EDITOR
        Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        Type type = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method!.Invoke(new object(), null);
#endif
    }

    public static void OneLineLog(object m) {
        ClearLog();
        Debug.Log(m);
    }
}