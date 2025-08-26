using System.Runtime.Serialization;

namespace SMS.Service.Common.Exceptions;
[Serializable]
internal class NotFountException : Exception
{
    public NotFountException()
    {
    }

    public NotFountException(string? message) : base(message)
    {
    }

    public NotFountException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected NotFountException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}