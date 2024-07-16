namespace ContinuousSurveillanceBackend.Core.Services
{
    public interface IMathService
    {
        decimal Add(decimal parameter1, decimal parameter2);
        decimal Sub(decimal parameter1, decimal parameter2);
        decimal Mul(decimal parameter1, decimal parameter2);
        decimal Div(decimal parameter1, decimal parameter2);
    }
}
