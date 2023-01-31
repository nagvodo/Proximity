namespace PROXX.Domain.Exceptions
{
    public class MaxAdjacentCellsExceededException : Exception
    {
        public MaxAdjacentCellsExceededException(string message) : base(message) { }
    }
}
