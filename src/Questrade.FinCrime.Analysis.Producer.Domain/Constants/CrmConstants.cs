namespace Questrade.FinCrime.Analysis.Producer.Domain.Constants;

public static class CrmConstants
{
    public const string GetUserAndPersonAccountsQuery = @"
        query UserPerson($userId: Int, $personIds: [ID]) {
            userPerson(userPersonQueryInput: {userID: $userId}) {
                person {
                    email
                    firstName
                    lastName
                    domesticAddress{
                        city
                        postalCode
                        province
                        provinceCode
                        provinceId
                        streetDirection
                        streetName
                        streetNumber
                        streetSuffix
                        streetType
                        unitNumber
                    }
                    internationalAddress{
                        addressLine1
                        addressLine2
                        addressLine3
                        addressTypeId
                        bpsCountryCode
                        countryCode
                        countryId
                        countryName
                        isIRSTreatyCountry
                        ismCountryCode
                        ismResidenceCode
                        isoCountryCode
                        postalCode
                        provinceState
                    }
                    phoneNumbers{
                        phoneNumber
                    }
                }
            }
            personAccounts(personAccountQueryInput: {personIds: $personIds}) {
                accountNumber
                effectiveDate
                accountStatusId
            }
        }";

    public const string GetPersonIdByUserId = @"
        query UserPerson($userId: Int) {
          userPerson(userPersonQueryInput: {userID: $userId }) {
            person {
                  personId
              }
            }
      }";
}
