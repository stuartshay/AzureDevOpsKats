using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AzureDevOpsKats.Web.Components.Shared;

public class FluentValidationValidator : ComponentBase, IDisposable
{
    [CascadingParameter]
    private EditContext? EditContext { get; set; }

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    private ValidationMessageStore? _messageStore;

    protected override void OnInitialized()
    {
        if (EditContext is null)
        {
            throw new InvalidOperationException(
                $"{nameof(FluentValidationValidator)} requires a cascading parameter of type {nameof(EditContext)}. " +
                "Ensure this component is used inside an EditForm or that an EditContext is provided.");
        }

        _messageStore = new ValidationMessageStore(EditContext);
        EditContext.OnValidationRequested += HandleValidationRequested;
        EditContext.OnFieldChanged += HandleFieldChanged;
    }

    public void Dispose()
    {
        if (EditContext is not null)
        {
            EditContext.OnValidationRequested -= HandleValidationRequested;
            EditContext.OnFieldChanged -= HandleFieldChanged;
            _messageStore?.Clear();
        }
    }

    private void HandleValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        if (EditContext is null || _messageStore is null)
            return;

        _messageStore.Clear();

        var validatorType = typeof(IValidator<>).MakeGenericType(EditContext.Model.GetType());
        if (ServiceProvider.GetService(validatorType) is not IValidator validator)
            return;

        var context = new ValidationContext<object>(EditContext.Model);
        var result = validator.Validate(context);

        foreach (var error in result.Errors)
        {
            var fieldIdentifier = new FieldIdentifier(EditContext.Model, error.PropertyName);
            _messageStore.Add(fieldIdentifier, error.ErrorMessage);
        }

        EditContext.NotifyValidationStateChanged();
    }

    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        if (EditContext is null || _messageStore is null)
            return;

        _messageStore.Clear(e.FieldIdentifier);

        var validatorType = typeof(IValidator<>).MakeGenericType(EditContext.Model.GetType());
        if (ServiceProvider.GetService(validatorType) is not IValidator validator)
            return;

        var context = new ValidationContext<object>(EditContext.Model);
        var result = validator.Validate(context);

        foreach (var error in result.Errors.Where(err => err.PropertyName == e.FieldIdentifier.FieldName))
        {
            _messageStore.Add(e.FieldIdentifier, error.ErrorMessage);
        }

        EditContext.NotifyValidationStateChanged();
    }
}
