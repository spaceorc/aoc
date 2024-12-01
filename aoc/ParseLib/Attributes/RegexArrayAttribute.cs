using System;
using System.Linq;
using System.Text.RegularExpressions;
using aoc.ParseLib.Structures;

namespace aoc.ParseLib.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class RegexArrayAttribute(string regex) : StructureAttribute
{
    public string Regex { get; } = regex;

    public override string ToString() => $"RegexArray[{Regex}], {base.ToString()}";

    public override TypeStructure CreateStructure(Type type, TypeStructureParserContext context)
    {
        var regex = CreateRegex();
        var groupNames = regex.GetGroupNames();
        if (!type.IsArray)
            throw new InvalidOperationException($"Regex array attribute can be applied only to array. Target: {context.Target}");
        var groupName = groupNames.Single(n => n != "0");
        return new RegexArrayStructure(type, regex, TypeStructureParser.Parse(type.GetElementType()!, null, context.Nested(groupName)));
    }

    private Regex CreateRegex()
    {
        return new Regex(Regex, RegexOptions.Compiled | RegexOptions.Singleline);
    }
}
