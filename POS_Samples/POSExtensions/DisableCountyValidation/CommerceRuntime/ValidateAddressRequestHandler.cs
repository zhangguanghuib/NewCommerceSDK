using Microsoft.Dynamics.Commerce.Runtime.Messages;
using Microsoft.Dynamics.Commerce.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Dynamics.Commerce.Runtime.DataServices.Messages;
using System.Threading.Tasks;
using Microsoft.Dynamics.Commerce.Runtime.DataAccess.SqlServer;
using Microsoft.Dynamics.Commerce.Runtime.DataModel;
using Microsoft.Dynamics.Retail.Diagnostics;
using System.Globalization;

namespace Contoso.GasStationSample.CommerceRuntime
{
    public class ValidateAddressRequestHandler : SingleAsyncRequestHandler<ValidateAddressDataRequest>
    {
        private const string ValidateAddressSprocName = "[ext].[VALIDATEADDRESSExt]";

        protected override Task<Response> Process(ValidateAddressDataRequest validateAddressDataRequest)
        {
            ThrowIf.Null(validateAddressDataRequest, nameof(validateAddressDataRequest));

            return this.ValidateAddress(validateAddressDataRequest);
        }

        private async Task<Response> ValidateAddress(ValidateAddressDataRequest request)
        {
            ThrowIf.Null(request.Address, "address");

            if (string.IsNullOrWhiteSpace(request.Address.ThreeLetterISORegionName))
            {
                return CreateFailedValidateAddressDataResponse(DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidCountryRegion, AddressServiceConstants.ThreeLetterISORegionName);
            }

            Address address = request.Address;

            ParameterSet parameters = new ParameterSet();

            parameters[AddressServiceConstants.CountryRegionId] = address.ThreeLetterISORegionName;

            if (!string.IsNullOrWhiteSpace(address.State))
            {
                parameters[AddressServiceConstants.StateProvinceId] = address.State;
            }

            if (!string.IsNullOrWhiteSpace(address.County))
            {
                parameters[AddressServiceConstants.CountyId] = address.County;
            }

            if (!string.IsNullOrWhiteSpace(address.City))
            {
                parameters[AddressServiceConstants.CityComponentName] = address.City;
            }

            if (!string.IsNullOrWhiteSpace(address.DistrictName))
            {
                parameters[AddressServiceConstants.DistrictId] = address.DistrictName;
            }

            if (!string.IsNullOrWhiteSpace(address.ZipCode))
            {
                parameters[AddressServiceConstants.ZipPostalCodeComponentName] = address.ZipCode;
            }

            int result;
            using (var databaseContext = new SqlServerDatabaseContext(request.RequestContext))
            {
                result = await databaseContext.ExecuteStoredProcedureNonQueryAsync(ValidateAddressSprocName, parameters, request.QueryResultSettings).ConfigureAwait(false);
            }

            if (result == 0)
            {
                return new ValidateAddressDataResponse(true);
            }

            Tuple<DataValidationErrors, string> faultAddressComponent = HandleNonZeroResult(result);

            return CreateFailedValidateAddressDataResponse(faultAddressComponent.Item1, faultAddressComponent.Item2);
        }

        private static ValidateAddressDataResponse CreateFailedValidateAddressDataResponse(DataValidationErrors errorCode, string faultAddressComponent)
        {
            // If address is not valid, tell the user/client code : which component is the faulty one
            var message = string.Format(CultureInfo.InvariantCulture, @"Incorrect address provided: validate {0} property.", faultAddressComponent);
            RetailLogger.Log.AddressSqlServerDataServiceCreateFailedValidateAddressDataResponse(message);

            // create the response object and return
            return new ValidateAddressDataResponse(isAddressValid: false, invalidAddressComponentName: faultAddressComponent, errorCode: errorCode, errorMessage: message);
        }

        private static Tuple<DataValidationErrors, string> HandleNonZeroResult(int result)
        {
            Tuple<DataValidationErrors, string> faultAddressComponent;
            switch (result)
            {
                case 1:
                    faultAddressComponent = new Tuple<DataValidationErrors, string>(
                DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidCountryRegion,
                AddressServiceConstants.CountryRegionId);
                    break;

                case 2:
                    faultAddressComponent = new Tuple<DataValidationErrors, string>(
                DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidStateProvince,
                AddressServiceConstants.StateProvinceId);
                    break;

                case 3:
                    faultAddressComponent = new Tuple<DataValidationErrors, string>(
                DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidCounty,
                AddressServiceConstants.CountyId);
                    break;

                case 4:
                    faultAddressComponent = new Tuple<DataValidationErrors, string>(
                DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidCity,
                AddressServiceConstants.CityComponentName);
                    break;

                case 5:
                    faultAddressComponent = new Tuple<DataValidationErrors, string>(
                DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidDistrict,
                AddressServiceConstants.DistrictId);
                    break;

                case 6:
                    faultAddressComponent = new Tuple<DataValidationErrors, string>(
                DataValidationErrors.Microsoft_Dynamics_Commerce_Runtime_InvalidZipPostalCode,
                AddressServiceConstants.ZipPostalCodeComponentName);
                    break;

                default:
                    throw new StorageException(StorageErrors.Microsoft_Dynamics_Commerce_Runtime_CriticalStorageError, result);
            }

            return faultAddressComponent;
        }

    }
}
