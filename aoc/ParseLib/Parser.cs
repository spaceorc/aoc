using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aoc.Lib;
using aoc.ParseLib.Attributes;
using aoc.ParseLib.Structures;

namespace aoc.ParseLib;

public static class Parser
{
    public static object?[] ParseMethodParameterValues(MethodBase method, string[] lines)
    {
        ICustomAttributeProvider methodAttributesTarget = method is ConstructorInfo constructorInfo
            ? constructorInfo.DeclaringType!
            : method;
        var parameters = method.GetParameters();
        if (parameters.Length == 1 
            && parameters[0].ParameterType == typeof(string[]) 
            && !parameters[0].GetCustomAttributes().OfType<StructureAttribute>().Any()
            && !methodAttributesTarget.GetCustomAttributes(true).OfType<StructureAttribute>().Any())
            return [lines];

        if (methodAttributesTarget.GetCustomAttributes(true).OfType<StructureAttribute>().Any())
        {
            var methodStructure = MethodStructure.CreateStructure(method);
            return methodStructure.CreateParameters(string.Join('\n', lines));
        }

        var regions = lines.Regions();
        if (parameters.All(p => p.GetCustomAttribute<ParamArrayAttribute>() == null) &&
            parameters.Length != regions.Count)
            throw new Exception($"Input regions count {regions.Count}, but parameters count {parameters.Length}");

        var args = new List<object?>();

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                var restRegions = regions.Skip(i).ToArray();

                var regionItemType = parameter.ParameterType.GetElementType()!;
                if (regionItemType == typeof(string[]))
                {
                    args.Add(restRegions);
                    break;
                }

                var arg = Array.CreateInstance(regionItemType, restRegions.Length);
                for (var r = 0; r < restRegions.Length; r++)
                    arg.SetValue(ParseParameterValue(parameter, regionItemType, restRegions[r]), r);

                args.Add(arg);
                break;
            }

            var region = regions[i];
            args.Add(
                parameter.ParameterType == typeof(string[]) && parameter.GetCustomAttribute<NonArrayAttribute>() == null
                    ? region
                    : ParseParameterValue(parameter, parameter.ParameterType, region)
            );
        }

        return args.ToArray();
    }

    private static object ParseParameterValue(ICustomAttributeProvider customAttributeProvider, Type parameterType, string[] lines)
    {
        if (parameterType.IsArray && !customAttributeProvider.GetCustomAttributes(false).OfType<NonArrayAttribute>().Any())
        {
            var itemStructure = TypeStructureParser.Parse(parameterType.GetElementType()!, customAttributeProvider);
            var parseAllGeneric = typeof(Parser).GetMethod(nameof(ParseAll), BindingFlags.NonPublic | BindingFlags.Static);
            var parseAll = parseAllGeneric!.MakeGenericMethod(parameterType.GetElementType()!);
            return parseAll.Invoke(null, [itemStructure, lines])!;
        }

        var structure = TypeStructureParser.Parse(parameterType, customAttributeProvider);
        var parseGeneric = typeof(Parser).GetMethod(nameof(Parse), BindingFlags.NonPublic | BindingFlags.Static);
        var parse = parseGeneric!.MakeGenericMethod(parameterType);
        return parse.Invoke(null, [structure, string.Join('\n', lines)])!;
    }

    private static T[] ParseAll<T>(TypeStructure structure, IEnumerable<string> lines) => lines.Select(x => Parse<T>(structure, x)).ToArray();
    private static T Parse<T>(TypeStructure structure, string line) => (T)structure.CreateObject(line);
}
