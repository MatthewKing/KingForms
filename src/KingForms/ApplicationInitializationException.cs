using System.Runtime.Serialization;

namespace KingForms;

public class ApplicationInitializationException : Exception
{
    public ApplicationInitializationException() { }

    public ApplicationInitializationException(string message)
        : base(message) { }

    public ApplicationInitializationException(string message, Exception innerException)
        : base(message, innerException) { }

    protected ApplicationInitializationException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
