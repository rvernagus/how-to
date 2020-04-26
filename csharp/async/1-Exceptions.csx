using System;
using System.Threading.Tasks;

async Task NoTry()
{
    await Task.Run(() =>
    {
        Console.WriteLine("Running bad Task...");
        throw new Exception("BOOM!");
    });
}

async Task WithTry()
{
    try
    {
        await Task.Run(() =>
        {
            Console.WriteLine("Running bad Task...");
            throw new Exception("BOOM!");
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Caught exception: {ex.Message}");
    }
}

await WithTry();
await NoTry();
