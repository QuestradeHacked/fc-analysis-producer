using MediatR;
using Questrade.FinCrime.Analysis.Producer.Domain.Exceptions;
using Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;
namespace Questrade.FinCrime.Analysis.Producer.Domain.Models;

public class CustomerProfileEmailUpdatedRequest : IRequest
{
    public CustomerProfileEmailUpdatedRequest(string userId, CrmPerson person, PersonAccount? personAccount)
    {
        CrmUserId = userId;
        ProfileId = person.PersonId;
        LoadCustomerInfo(person, personAccount);
    }

    public string? AccountDetailNumber { get; private set; }
    public int? AccountStatusId { get; private set; }
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string? CreatedAt { get; set; }
    public string? CrmUserId { get; }
    public DateTime? EffectiveDate { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? EnterpriseProfileId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string? ProfileId { get; }
    public string State { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string? UpdatedAt { get; set; }
    public string ZipCode { get; private set; } = string.Empty;

    private void LoadCustomerInfo(CrmPerson person, PersonAccount? personAccount)
    {
        if (person.InternationalAddress is null && person.DomesticAddress is null) throw new NoAddressFoundException();

        CreatedAt = person.Created;
        EnterpriseProfileId = "";
        FirstName = person.FirstName;
        LastName = person.LastName;
        PhoneNumber = GetPersonalPhoneNumber(person);
        UpdatedAt = person.Updated;

        if (string.IsNullOrEmpty(Email)) Email = person.Email!;

        if (person.DomesticAddress is not null)
        {
            City = person.DomesticAddress.City;
            Country = "Canada";
            State = person.DomesticAddress.Province;
            Street = person.DomesticAddress.StreetName;
            ZipCode = person.DomesticAddress.PostalCode;
        }
        else
        {
            City = person.InternationalAddress!.AddressLine2;
            Country = person.InternationalAddress.CountryName;
            State = person.InternationalAddress.ProvinceState;
            Street = person.InternationalAddress.AddressLine1;
            ZipCode = person.InternationalAddress.PostalCode;
        }

        if (personAccount != null)
        {
            AccountDetailNumber = personAccount.AccountNumber;
            AccountStatusId = personAccount.AccountStatusId;
            EffectiveDate = personAccount.EffectiveDate;
        }
    }

    private static string GetPersonalPhoneNumber(CrmPerson person)
    {
        var personalNumbers = person
            .PhoneNumbers
            .Where(p => p.Type == PhoneNumberType.Personal)
            .ToList();

        if (!personalNumbers.Any())
            return string.Empty;

        return personalNumbers
            .Select(p => p.Number)
            .FirstOrDefault() ?? string.Empty;
    }
}
