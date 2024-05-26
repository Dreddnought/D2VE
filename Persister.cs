using Microsoft.Win32;

namespace D2VE;

// Haven't decided whether to use registry or a file for persistence.
public static class Persister
{
    public static void Save(string key, string value)
    {
        Registry.SetValue(@"HKEY_CURRENT_USER\Software\D2VE", key, value, RegistryValueKind.String);
    }
    public static string Load(string key)
    {
        return Registry.GetValue(@"HKEY_CURRENT_USER\Software\D2VE", key, null) as string;
    }
}
