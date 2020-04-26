public struct Point
{
    public double X { get; set; }
    public double Y { get; set; }
    // Add/remove readonly declaration to see compiler warning
    public readonly double Distance => Math.Sqrt(X * X + Y * Y);

    // Will not compile as long as X and Y are writeable
    public readonly void Translate(int xOffset, int yOffset)
    {
        X += xOffset;
        Y += yOffset;
    }

    public readonly override string ToString() =>
        $"({X}, {Y}) is {Distance} from the origin";
}

var point = new Point();
Console.WriteLine(point);
