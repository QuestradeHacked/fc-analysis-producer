using FluentValidation;
using Questrade.FinCrime.Analysis.Producer.Infra.Validators;
using Xunit;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Unit.Validators;

using Object = Google.Apis.Storage.v1.Data.Object;

public class BucketObjectValidatorTest
{
    private readonly IValidator<Object> _validation = new BucketObjectValidator();

    [Fact]
    public async Task ValidateAsync_ShouldReturnFalse_WhenFileIsNotCsv()
    {
        // Arrange
        var invalidObject = new Object { Name = "emailintelligence-batch-process-20231409034700.txt" };

        // Act
        var validationResult = await _validation.ValidateAsync(invalidObject);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Collection(validationResult.Errors, failure =>
            Assert.Equal("'Name' is not in the correct format.", failure.ErrorMessage)
        );
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnFalse_WhenFileNameContainsSpecialCharacters()
    {
        // Arrange
        var invalidObject = new Object { Name = "emailintelligence-batch-process-20231409034700#!.csv" };

        // Act
        var validationResult = await _validation.ValidateAsync(invalidObject);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Collection(validationResult.Errors, failure =>
            Assert.Equal("'Name' is not in the correct format.", failure.ErrorMessage)
        );
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnFalse_WhenFileNameDoesNotMatchPattern()
    {
        // Arrange
        var invalidObject = new Object { Name = "pretend_there_is_a_clever_name_here.csv" };

        // Act
        var validationResult = await _validation.ValidateAsync(invalidObject);


        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Collection(validationResult.Errors, failure =>
            Assert.Equal("'Name' is not in the correct format.", failure.ErrorMessage)
        );
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnTrue_WhenTheNameIsCorrect()
    {
        // Arrange
        var validObject = new Object { Name = "emailintelligence-batch-process-20231409034700.csv" };

        // Act
        var validationResult = await _validation.ValidateAsync(validObject);

        //Assert
        Assert.True(validationResult.IsValid);
    }
}
