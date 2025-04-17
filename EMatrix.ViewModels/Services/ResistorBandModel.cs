namespace EMatrix.ViewModels.Services;

public class ResistorBandModel
{
    public string Color { get; set; } = null!;

    public int? FirstDigit { get; set; }

    public int? SecondDigit { get; set; }

    public int? ThirdDigit { get; set; }

    public double? Multiplier { get; set; }

    public double? Tolerance { get; set; }

    public int? TempCoefficient { get; set; }
}