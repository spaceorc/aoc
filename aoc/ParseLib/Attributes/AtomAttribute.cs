using System;
using aoc.ParseLib.Structures;

namespace aoc.ParseLib.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class AtomAttribute(string separators = TypeStructureParser.DefaultSeparators) : StructureAttribute
{
    public string Separators { get; } = separators;

    public override string ToString() => $"Atom[{Separators}], {base.ToString()}";

    public override TypeStructure CreateStructure(Type type, TypeStructureParserContext context)
    {
        return new AtomStructure(type, Separators);
    }
}
