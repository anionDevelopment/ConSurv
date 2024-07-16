namespace ContinuousSurveillanceBackend.Core.Services
{
    public class MathService : IMathService
    {
        public decimal Add(decimal parameter1, decimal parameter2)
        {
            return DotNetLibraryCodeUnit.Core.Miscellaneous.Utilities.Add(parameter1, parameter2);
        }
        public decimal Sub(decimal parameter1, decimal parameter2)
        {
            return DotNetLibraryCodeUnit.Core.Miscellaneous.Utilities.Sub(parameter1, parameter2);
        }
        public decimal Mul(decimal parameter1, decimal parameter2)
        {
            return DotNetLibraryCodeUnit.Core.Miscellaneous.Utilities.Mul(parameter1, parameter2);
        }
        public decimal Div(decimal parameter1, decimal parameter2)
        {
            return DotNetLibraryCodeUnit.Core.Miscellaneous.Utilities.Div(parameter1, parameter2);
        }
    }
}
