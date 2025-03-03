namespace StudyFlow.Application.Common.Exceptions;
public class InvalidCodeException : Exception
{
    public InvalidCodeException() : base("Code is exist") { }
}
