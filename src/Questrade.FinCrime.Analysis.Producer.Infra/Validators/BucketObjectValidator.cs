using FluentValidation;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Questrade.FinCrime.Analysis.Producer.Infra.Validators;

public class BucketObjectValidator : AbstractValidator<Object>
{
    private const string NameRegex = @"emailintelligence-batch-process-[A-z0-9]+\.csv$";

    public BucketObjectValidator()
    {
        RuleFor(obj => obj.Name).NotNull();
        RuleFor(obj => obj.Name).NotEmpty();
        RuleFor(obj => obj.Name).Matches(NameRegex);
    }
}
