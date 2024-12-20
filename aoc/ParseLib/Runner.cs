using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BindingFlags = System.Reflection.BindingFlags;

namespace aoc.ParseLib;

public static class Runner
{
    public static void Run(string fileName, Delegate solve)
    {
        Run(File.ReadAllLines(fileName), solve);
    }

    public static void Run<T>(string? fileName = null)
    {
        var dir = FileHelper.PatchDirectoryName(typeof(T).Namespace!.Replace(".", "/"));
        var fullFileNames = !string.IsNullOrEmpty(fileName)
            ? [Path.Combine(dir, fileName)]
            : Directory.GetFiles(dir, "*.txt");
        Console.WriteLine($"\n=== {typeof(T).FullName} ===");
        foreach (var name in fullFileNames)
        {
            Console.WriteLine($"\n{name}:");
            Run<T>(File.ReadAllLines(name));
        }
    }

    public static void RunString(string source, Delegate solve)
    {
        Run(source.Split('\n'), solve);
    }

    public static void RunString<T>(string source)
    {
        Run<T>(source.Split('\n'));
    }

    private static void Run(string[] source, Delegate solve)
    {
        Invoke(solve, Parser.ParseMethodParameterValues(solve.Method, source));
    }

    private static void Run<T>(string[] source)
    {
        var constructorInfo = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
        Invoke(constructorInfo, Parser.ParseMethodParameterValues(constructorInfo, source));
    }

    private static void Invoke(Delegate solve, object?[] args)
    {
        var parameters = solve.Method.GetParameters();
        var dynamicMethod = new DynamicMethod(
            Guid.NewGuid().ToString(),
            typeof(void),
            [typeof(object?[])],
            typeof(Runner),
            skipVisibility: true
        );
        var il = dynamicMethod.GetILGenerator();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            il.Emit(OpCodes.Ldarg_0); // [args]
            il.Emit(OpCodes.Ldc_I4, i); // [args, i]
            il.Emit(OpCodes.Ldelem, typeof(object)); // [args[i]]
            if (!parameter.ParameterType.IsValueType)
                il.Emit(OpCodes.Castclass, parameter.ParameterType); // [(ParameterType)args[i]]
            else
                il.Emit(OpCodes.Unbox_Any, parameter.ParameterType); // [(ParameterType)args[i]]
        }

        // [*args]
        il.Emit(OpCodes.Call, solve.Method); // []
        il.Emit(OpCodes.Ret); // []

        dynamicMethod.CreateDelegate<Action<object?[]>>()(args);
    }

    private static void Invoke(ConstructorInfo constructorInfo, object?[] args)
    {
        var solveMethod = constructorInfo.DeclaringType!.GetMethods(BindingFlags.Public | BindingFlags.Instance).Single(x => x.Name == "Solve");
        var parameters = constructorInfo.GetParameters();
        var dynamicMethod = new DynamicMethod(
            Guid.NewGuid().ToString(),
            typeof(void),
            [typeof(object?[])],
            typeof(Runner),
            skipVisibility: true
        );
        var il = dynamicMethod.GetILGenerator();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            il.Emit(OpCodes.Ldarg_0); // [args]
            il.Emit(OpCodes.Ldc_I4, i); // [args, i]
            il.Emit(OpCodes.Ldelem, typeof(object)); // [args[i]]
            if (!parameter.ParameterType.IsValueType)
                il.Emit(OpCodes.Castclass, parameter.ParameterType); // [(ParameterType)args[i]]
            else
                il.Emit(OpCodes.Unbox_Any, parameter.ParameterType); // [(ParameterType)args[i]]
        }

        // [*args]
        il.Emit(OpCodes.Newobj, constructorInfo); // [solverInstance]
        il.Emit(OpCodes.Callvirt, solveMethod); // []
        il.Emit(OpCodes.Ret); // []

        dynamicMethod.CreateDelegate<Action<object?[]>>()(args);
    }
}
