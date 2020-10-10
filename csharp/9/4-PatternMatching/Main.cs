using System;

static decimal CalculateToll(object vehicle) =>
    vehicle switch
    {
        Car             => 2.00m, // Simple type patterns
        Taxi            => 3.50m,
        Bus             => 5.00m,
        DeliveryTruck t => t.GrossWeightClass switch // This switch demonstrates Relational Patterns
        {
            < 3000 => 10.00m - 2.00m,
            >= 3000 and <= 5000 => 10.00m, // Logical patterns using and, or, not
            > 5000 => 10.00m + 5.00m,
        },
        null            => throw new ArgumentNullException(nameof(vehicle)),
        _               => throw new ArgumentException("Not a known vehicle type", nameof(vehicle))
    };

public class DeliveryTruck
{
    public int GrossWeightClass { get; set; }
}

public class Car
{
    public int Passengers { get; set; }
}

public class Taxi
{
    public int Fares { get; set; }
}

public class Bus
{
    public int Capacity { get; set; }
    public int Riders { get; set; }
}
