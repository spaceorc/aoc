using System;

namespace aoc.Lib;

public static class DirExtensions
{
    public static Dir RotateCW(this Dir dir, long k = 1) => (Dir)((int)dir + k).Mod(4);
    public static Dir RotateCCW(this Dir dir, long k = 1) => (Dir)((int)dir - k).Mod(4);
    
    public static char ToChar(this Dir dir) => dir switch
    {
        Dir.Up => '^',
        Dir.Right => '>',
        Dir.Down => 'v',
        Dir.Left => '<',
    };
}
