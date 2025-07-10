using System.Reflection;

namespace TestProjectMap;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}