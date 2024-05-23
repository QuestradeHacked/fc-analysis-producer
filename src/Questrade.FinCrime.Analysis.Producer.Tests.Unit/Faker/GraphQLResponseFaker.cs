using Bogus;
using GraphQL;
using Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Unit.Faker;

public static class GraphQLResponseFaker
{
    public static GraphQLResponse<CrmResponse> GenerateValidResponseForPersonId()
    {
        var crmPersonFaker = new Faker<CrmPerson>()
            .RuleFor(u => u.FirstName, p => p.Person.FirstName)
            .RuleFor(u => u.LastName, p => p.Person.LastName)
            .RuleFor(u => u.PersonId, _ => _.Random.Number(1000000, 9999999).ToString());

        var userPersonsFaker = new Faker<UserPersons>()
            .RuleFor(u => u.Persons, crmPersonFaker.Generate(1));

        var personAccountFaker = new Faker<PersonAccount>()
            .RuleFor(u => u.EffectiveDate, _ => _.Date.Past());

        return new GraphQLResponse<CrmResponse>
        {
            Data = new CrmResponse
            {
                UserPerson = userPersonsFaker.Generate(1),
                PersonAccounts = personAccountFaker.Generate(1)
            }
        };
    }

    public static GraphQLResponse<CrmResponse> GenerateNonPersonId()
    {
        var personAccountFaker = new Faker<PersonAccount>()
            .RuleFor(u => u.EffectiveDate, _ => _.Date.Past());

        return new GraphQLResponse<CrmResponse>
        {
            Data = new CrmResponse
            {
                UserPerson = new List<UserPersons>(),
                PersonAccounts = personAccountFaker.Generate(1)
            }
        };
    }

    public static GraphQLResponse<CrmResponse> GenerateInvalidResponse()
    {
        var crmPersonFaker = new Faker<CrmPerson>()
            .RuleFor(u => u.FirstName, p => p.Person.FirstName)
            .RuleFor(u => u.LastName, p => p.Person.LastName)
            .RuleFor(u => u.PersonId, _ => _.Random.Number(1000000, 9999999).ToString());

        var userPersonsFaker = new Faker<UserPersons>()
            .RuleFor(u => u.Persons, crmPersonFaker.Generate(1));

        var personAccountFaker = new Faker<PersonAccount>()
            .RuleFor(u => u.EffectiveDate, _ => _.Date.Past());

        return new GraphQLResponse<CrmResponse>
        {
            Data = new CrmResponse
            {
                UserPerson = userPersonsFaker.Generate(1),
                PersonAccounts = personAccountFaker.Generate(1)
            },
            Errors = new GraphQLError[]
            {
                new(),
                new()
            }
        };
    }
}
