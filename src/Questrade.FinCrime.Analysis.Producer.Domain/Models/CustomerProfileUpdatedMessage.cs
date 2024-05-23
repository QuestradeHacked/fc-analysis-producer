namespace Questrade.FinCrime.Analysis.Producer.Domain.Models;

public class CustomerProfileUpdatedMessage
{
    public DateTime? BirthDate { get; set; }

    public CustomerDomesticAddress? DomesticAddress { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? Id { get; set; }

    public CustomerInternationalAddress? InternationalAddress { get; set; }

    public string? LastName { get; set; }

    public string? MaritalStatus { get; set; }

    public string? MiddleName { get; set; }

    public string? PersonId { get; set; }

    public IEnumerable<CustomerPhoneNumber>? PhoneNumbers { get; set; }

    public string? Sin { get; set; }

    public string? Ssn { get; set; }

    public string? UserId { get; set; }
}
