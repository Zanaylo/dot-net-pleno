using System.ComponentModel.DataAnnotations;

namespace StallosDotnetPleno.Domain.Extensions
{
    public class RequiredListAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as IEnumerable<object>;
            return list != null && list.Any();
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} Precisa informar ao menos um item";
        }
    }
}