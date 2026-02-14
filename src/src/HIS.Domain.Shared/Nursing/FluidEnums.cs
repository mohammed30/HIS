namespace HIS.Nursing;

public enum FluidType
{
    Input = 0,
    Output = 1
}

public enum FluidMetric
{
    // Input
    Oral = 0,
    IV = 1,
    TubeFeeding = 2,
    
    // Output
    Urine = 10,
    Stool = 11,
    Vomit = 12,
    Drain = 13,
    Sweat = 14
}
