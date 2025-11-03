// This file enables C# 9+ records and init-only properties on older target frameworks
// (netstandard2.0, net48, etc.) that don't have this type built-in.

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
#endif

