using System.Reflection;

namespace W2K.Common.Application.Validations;

public class ValidationLanguageManager : FluentValidation.Resources.LanguageManager
{
    public ValidationLanguageManager()
    {
        foreach (var language in new string[] { "en", "en-US" })
        {
            foreach (var field in typeof(ValidationCodes).GetFields())
            {
                var message = field.GetCustomAttribute<MessageAttribute>()?.Message;
                if (message is not null)
                {
                    AddTranslation(language, field.Name, message);
                }
            }
        }
    }
}
