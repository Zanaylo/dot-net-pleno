namespace StallosDotnetPleno.Api.Security;

public class PessoaValidationException : Exception
{
    public IDictionary<string, IEnumerable<string>> Errors { get; }

    public PessoaValidationException(IDictionary<string, IEnumerable<string>> errors)
    {
        Errors = errors;
    }
}
