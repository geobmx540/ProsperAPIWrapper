using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProsperAPIWrapper;

namespace ProsperAPIWrapperTest
{
    [TestClass]
    public class ProsperApiWrapperTests
    {
        
        //Staging Credentials
        private const string Username = "lender01@1.stg";
        private const string Password = "Password23";

        // Staging baseUrl: https://api.stg.circleone.com/v1/
        // Production baseUrl: https://api.prosper.com/v1/

        private readonly ProsperApi _api = new ProsperApi(Username, Password, "https://api.stg.circleone.com/v1/");

        [TestMethod]
        public void Contructor_Null_Username()
        {
            ArgumentException e = null;

            try
            {
                var a = new ProsperApi("", null, "");
            }
            catch (ArgumentException ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e, "An exception should have been thrown for null / empty credentials");
        }

        [TestMethod]
        public void Authenticate_Success()
        {
            var success = _api.AuthenticateAsync().Result;

            Assert.IsTrue(success, "Credentials are invalid");
        }

        [TestMethod]
        public void Authenticate_Failure()
        {
            var api = new ProsperApi("wrong", "credentials", "https://api.stg.circleone.com/v1/");
            var success = api.AuthenticateAsync().Result;

            Assert.IsFalse(success, "Authentication should have failed, but it was successful");
        }

        [TestMethod]
        public void GetNotes()
        {
            var notes = _api.GetNotesAsync().Result;

            Assert.IsTrue(notes.Count > 2, "Notes are empty");
        }

        [TestMethod]
        public void GetAccount()
        {
            var account = _api.GetAccountAsync().Result;

            Assert.IsTrue(account.TotalAccountValue > 40000, "You seem poor");
        }

        [TestMethod]
        public void GetListings()
        {
            var listings = _api.GetListingsAsync().Result;

            Assert.IsTrue(listings.Count > 0, "No listings, that's possible, but unlikely");
        }

        [TestMethod]
        public void GetPendingInvestments()
        {
            var pendingInvestments = _api.GetListingsAsync().Result;

            Assert.IsTrue(pendingInvestments.Count > 0, "Zero Pending Investments");
        }

        [TestMethod]
        public void Invest()
        {
            // Note this will fail in either test or production - so just in 
            // case credentials are set to prod, we won't invest anything.
            var result = _api.InvestAsync("23443", "20").Result;

            Assert.IsNotNull(result, "Invest returned null result");
        }
    }
}
