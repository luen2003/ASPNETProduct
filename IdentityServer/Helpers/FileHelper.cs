using System.Runtime.CompilerServices;

namespace IdentityServer.Helpers;
public class FileHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteFileDefault(string data) => File.AppendAllText("Temp.txt", data + Environment.NewLine);
}
