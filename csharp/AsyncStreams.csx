async static IAsyncEnumerable<string> EnumerateFamily()
{
    var names = new[] { "Ray", "Karenn", "Ari", "Rahm" };
    foreach (var name in names)
    {
        await Task.Delay(1000);
        yield return name;
    }
}

await foreach (var name in EnumerateFamily())
{
    Console.WriteLine(name);
}
