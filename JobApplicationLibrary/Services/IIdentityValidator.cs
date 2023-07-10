using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary.Services
{
    public interface IIdentityValidator
    {
        bool IsValid(string identityNumber);

        //bool CheckConnectionToRemoteServer();

        ICountryDataProvider CountryDataProvider { get; }

        public ValidationMode ValidationMode { get; set; }

    }

    public enum ValidationMode
    {
        Detailed,
        Quick
    }


    public interface ICountryData
    {
        string Country { get; }

    }

    public interface ICountryDataProvider
    {
        ICountryData CountryData { get; }

    }





}



