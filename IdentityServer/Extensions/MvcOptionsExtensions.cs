using IdentityServer.Extensions;
using System.ComponentModel;

namespace IdentityServer.Extensions;

public static class MvcOptionsExtensions
{
    public static void UseDateOnlyTimeOnlyStringConverters()
    {
        TypeDescriptor.AddAttributes(typeof(DateOnly), new TypeConverterAttribute(typeof(DateOnlyTypeConverter)));
    }
}