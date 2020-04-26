string[] words = new string[]
{
                // index from start    index from end
    "The",      // 0                   ^9
    "quick",    // 1                   ^8
    "brown",    // 2                   ^7
    "fox",      // 3                   ^6
    "jumped",   // 4                   ^5
    "over",     // 5                   ^4
    "the",      // 6                   ^3
    "lazy",     // 7                   ^2
    "dog"       // 8                   ^1
};              // 9 (or words.Length) ^0

Console.WriteLine(words[^1]);
Console.WriteLine(string.Join(" ", words[5..^1]));
Console.WriteLine(string.Join(" ", words[5..^0]));
Console.WriteLine(string.Join(" ", words[..]));
Console.WriteLine(string.Join(" ", words[..^3]));
Console.WriteLine(string.Join(" ", words[^3..]));
