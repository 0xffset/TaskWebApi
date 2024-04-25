using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tasks.API.Helpers
{
    public static class ModalStateHelper
    {
        public static string GetErrors(ModelStateDictionary modelState)
        {
            var errors = modelState
                .Where(e => e.Value.Errors.Any())
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => string.Join(", ", kvp.Value.Errors.Select(error => error.ErrorMessage))
                );

            return string.Join(", ", errors.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }
    }
}
