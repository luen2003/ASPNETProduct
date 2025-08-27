using System.Reflection;
using System.Runtime.CompilerServices;

namespace IdentityServer.Helpers;
public class AssemblyHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetProjectName() => Assembly.GetCallingAssembly().GetName().Name ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetProjectName(Assembly assembly) => assembly.GetName().Name ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetAssemblyName() => Assembly.GetCallingAssembly().GetName().ToString() ?? string.Empty;
}
