using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Prism.Validation;

namespace GrepRipper.UI.Validation;

public static class ValidatableBindableBaseExtensions
{
    public static void SetGeneralError(this ValidatableBindableBase model, Exception ex)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model)); 
        }

        model.SetGeneralError(ex.ToStringDemystified());
    }

    public static void SetGeneralError(this ValidatableBindableBase model, string error)
    {
        model.SetAllErrors(
            new Dictionary<string, ReadOnlyCollection<string>>
            {
                [string.Empty] = new(new[] { error })
            });
    }

    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    public static void BubbleFutureGeneralError(this ValidatableBindableBase model, ValidatableBindableBase parent)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        model.ErrorsChanged += (sender, args) =>
        {
            if (sender is not BindableValidator bindableValidator)
            {
                return;
            }

            if (args.PropertyName?.Length != 0 
                || !bindableValidator.Errors.TryGetValue(args.PropertyName, out var errors) 
                || errors.Count <= 0)
            {
                return;
            }

            var error = errors[0];
            bindableValidator.Errors.Remove(args.PropertyName);
            parent.SetGeneralError(error);
        };
    }
}
