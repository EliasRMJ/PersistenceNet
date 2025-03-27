namespace PersistenceNet.Extensions
{
    public static class ExceptionExtension
    {
        public static string AggregateMessage(this Exception ex)
        {
            var message = ex.Message;
            var inner = (ex.InnerException is not null);
            var innerEx = ex.InnerException;
            while (inner)
            {
                message += $"-->> {innerEx?.Message}";
                inner = (innerEx?.InnerException is not null);
                innerEx = innerEx?.InnerException;
            }

            return message;
        }
    }
}