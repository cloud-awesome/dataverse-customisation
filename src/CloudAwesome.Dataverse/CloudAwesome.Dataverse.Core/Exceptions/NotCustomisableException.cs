using System.Diagnostics.CodeAnalysis;

namespace CloudAwesome.Dataverse.Core.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotCustomisableException: Exception
    {
        public NotCustomisableException()
        {

        }

        public NotCustomisableException(string message) : base(message)
        {

        }

        public NotCustomisableException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
