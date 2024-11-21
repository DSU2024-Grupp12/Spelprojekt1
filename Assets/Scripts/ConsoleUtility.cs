using System.Reflection;

public static class ConsoleUtility
{
    // https://stackoverflow.com/questions/40577412/clear-editor-console-logs-from-script
    public static void ClearLog() {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method!.Invoke(new object(), null);
    }
}