﻿using System;
using System.Diagnostics;
using System.Drawing;
using LOLAccountManagement.Classes;
using LOLCodeLibrary.ErrorsManagement;
using LOLCodeLibrary.LoggingSystem;

namespace Test_Interface_Console
{
    public sealed class Test_UserSave : TestBase, ITestable
    {
        //UserSave(Guid AccountID, Guid Token)
        //we are testing the following scenarios :
        //1. pass a token which is not authenticated yet        - should return AuthenticationTokenNotLoggedIn
        //2. pass a token which has expired                     - should return AuthenticationTokenExpired
        //3. pass a token which is already logged out           - should return AuthenticationTokenLoggedOut
        //4. pass a token which does not exist in the database  - should return AuthenticationTokenNotFound
        //5. pass an accountID which is not linked to the token - should return AuthenticationTokenDoesNotMatchAccountID
        //6. pass an accountID which is linked to a valid token - should return valid user and no errors

        #region ITestable

        public LOLConnect.LOLConnectClient _ws { get; set; }
        public ILogger Logger { get; set; }        

        public override void RunTests()
        {
            UserSave_TokenNotAuthenticated_ShouldFail();
            UserSave_TokenExpired_ShouldFail();
            UserSave_TokenLoggedOut_ShouldFail();
            UserSave_TokenNotInDatabase_ShouldFail();
            UserSave_AccountIdNotLinkedToToken_ShouldFail();
            UserSave_ValidInput_ShouldSucceed();
        }
        #endregion

        #region Constructor
        public Test_UserSave(LOLConnect.LOLConnectClient ws, ILogger logger) : base()
        {
            this._ws = ws;
            this.Logger = logger;
        }
        #endregion

        #region Test Methods

        private void UserSave_TokenNotAuthenticated_ShouldFail()
        {
            this.Logger.LogMessage("Testing UserSave_TokenNotAuthenticated_ShouldFail ...", true);
            Image profileImage = Image.FromFile(this.ImageFilePath);
            Guid token = _ws.AuthenticationTokenGet(this.RandomDeviceID);
            string newEmail = this.GetRandomEmail();

            LOLConnect.User tmpUserCreate = _ws.UserCreate(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, LOLConnect.AccountOAuth.OAuthTypes.LOL, this.RandomOAuthID, this.RandomOAuthToken, this.RandomFirstName, this.RandomLastName, newEmail, this.RandomPassword, ImageHandler.imageToByteArray(profileImage), new DateTime(1978, 2, 5), token);

            var elapsed = Stopwatch.StartNew();
            LOLConnect.User user = _ws.UserSave(tmpUserCreate, token);
            elapsed.Stop();
            this.Logger.LogMessage(PrepareElapsedTimeOutput(elapsed), true);

            if (user.Errors.Count == 1 && user.Errors[0].ErrorDescription.Equals(SystemTypes.ErrorMessage.AuthenticationTokenNotLoggedIn.ToString()))
                this.Logger.LogMessage(this.TestSuccessMessage, true);
            else
                this.Logger.LogMessage(this.TestFailMessage, true);

            this.Logger.LogMessage(this.Delimiter, true);
            this.CleanAfterTest(this._ws);            
        }

        private void UserSave_TokenExpired_ShouldFail()
        {
            this.Logger.LogMessage("Testing UserSave_TokenExpired_ShouldFail ...", true);
            this.Logger.LogMessage("Not Implemented Yet ...", true);

            this.Logger.LogMessage(this.Delimiter, true);
            this.CleanAfterTest(this._ws);
        }

        private void UserSave_TokenLoggedOut_ShouldFail()
        {
            this.Logger.LogMessage("Testing UserSave_TokenLoggedOut_ShouldFail ...",true);
            Guid token = _ws.AuthenticationTokenGet(this.RandomDeviceID);

            Image profileImage = Image.FromFile(this.ImageFilePath);
            var tmpUser = _ws.UserCreate(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, LOLConnect.AccountOAuth.OAuthTypes.LOL, string.Empty, string.Empty, this.RandomFirstName, this.RandomLastName, this.GetRandomEmail(), this.RandomPassword, ImageHandler.imageToByteArray(profileImage), new DateTime(1978, 2, 5), token);

            var loggedUser = _ws.UserLogin(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, tmpUser.AccountID, string.Empty, string.Empty, LOLConnect.AccountOAuth.OAuthTypes.LOL, string.Empty, string.Empty, token);
            var errors = _ws.UserLogOut(this.RandomDeviceID, tmpUser.AccountID, token);

            var elapsed = Stopwatch.StartNew();
            LOLConnect.User user = _ws.UserSave(tmpUser, token);
            elapsed.Stop();
            this.Logger.LogMessage(PrepareElapsedTimeOutput(elapsed), true);
            
            if (user.Errors.Count == 1 && user.Errors[0].ErrorDescription.Equals(SystemTypes.ErrorMessage.AuthenticationTokenLoggedOut.ToString()))
                this.Logger.LogMessage(this.TestSuccessMessage, true);
            else
                this.Logger.LogMessage(this.TestFailMessage, true);

            this.Logger.LogMessage(this.Delimiter, true);
            this.CleanAfterTest(this._ws);            
        }

        private void UserSave_TokenNotInDatabase_ShouldFail()
        {
            this.Logger.LogMessage("Testing UserSave_TokenNotInDatabase_ShouldFail ...", true);
            Guid token = _ws.AuthenticationTokenGet(this.RandomDeviceID);
            Image profileImage = Image.FromFile(this.ImageFilePath);
            var tmpUser = _ws.UserCreate(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, LOLConnect.AccountOAuth.OAuthTypes.LOL, string.Empty, string.Empty, this.RandomFirstName, this.RandomLastName, this.GetRandomEmail(), this.RandomPassword, ImageHandler.imageToByteArray(profileImage), new DateTime(1978, 2, 5), token);
            var loggedUser = _ws.UserLogin(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, tmpUser.AccountID, string.Empty, string.Empty, LOLConnect.AccountOAuth.OAuthTypes.LOL, string.Empty, string.Empty, token);

            var elapsed = Stopwatch.StartNew();
            LOLConnect.User user = _ws.UserSave(loggedUser, Guid.NewGuid());
            elapsed.Stop();
            this.Logger.LogMessage(PrepareElapsedTimeOutput(elapsed), true);

            if (user.Errors.Count == 1 && user.Errors[0].ErrorDescription.Equals(SystemTypes.ErrorMessage.AuthenticationTokenNotFound.ToString()))
                this.Logger.LogMessage(this.TestSuccessMessage, true);
            else
                this.Logger.LogMessage(this.TestFailMessage, true);

            this.Logger.LogMessage(this.Delimiter, true);
            this.CleanAfterTest(this._ws);            
        }

        private void UserSave_AccountIdNotLinkedToToken_ShouldFail()
        {
            this.Logger.LogMessage("Testing UserSave_AccountIdNotLinkedToToken_ShouldFail ...", true);
            Guid token = _ws.AuthenticationTokenGet(this.RandomDeviceID);

            Image profileImage = Image.FromFile(this.ImageFilePath);
            var tmpUser = _ws.UserCreate(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, LOLConnect.AccountOAuth.OAuthTypes.LOL, string.Empty, string.Empty, this.RandomFirstName, this.RandomLastName, this.GetRandomEmail(), this.RandomPassword, ImageHandler.imageToByteArray(profileImage), new DateTime(1978, 2, 5), token);

            LOLConnect.User userLoggedIn = _ws.UserLogin(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, tmpUser.AccountID, string.Empty, string.Empty, LOLConnect.AccountOAuth.OAuthTypes.LOL, string.Empty, string.Empty, token);
            tmpUser.AccountID = Guid.NewGuid();

            var elapsed = Stopwatch.StartNew();            
            LOLConnect.User user = _ws.UserSave(tmpUser, token);
            elapsed.Stop();
            this.Logger.LogMessage(PrepareElapsedTimeOutput(elapsed), true);
            
            if (user.Errors.Count == 1 && user.Errors[0].ErrorDescription.Equals(SystemTypes.ErrorMessage.AuthenticationTokenDoesNotMatchAccountID.ToString()))
                this.Logger.LogMessage(this.TestSuccessMessage, true);
            else
                this.Logger.LogMessage(this.TestFailMessage, true);

            this.Logger.LogMessage(this.Delimiter, true);
            this.CleanAfterTest(this._ws);            
        }

        private void UserSave_ValidInput_ShouldSucceed()
        {
            this.Logger.LogMessage("Testing UserSave_ShouldSucceed ...", true);
            Guid token = _ws.AuthenticationTokenGet(this.RandomDeviceID);

            Image profileImage = Image.FromFile(this.ImageFilePath);
            var tmpUser = _ws.UserCreate(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, LOLConnect.AccountOAuth.OAuthTypes.LOL, string.Empty, string.Empty, this.RandomFirstName, this.RandomLastName, this.GetRandomEmail(), this.RandomPassword, ImageHandler.imageToByteArray(profileImage), new DateTime(1978, 2, 5), token);

            LOLConnect.User userLoggedIn = _ws.UserLogin(this.RandomDeviceID, LOLConnect.DeviceDeviceTypes.Windows, tmpUser.AccountID, string.Empty, string.Empty, LOLConnect.AccountOAuth.OAuthTypes.LOL, string.Empty, string.Empty, token);
            userLoggedIn.EmailAddress = "emailChanged@Rolo.cem";
            var elapsed = Stopwatch.StartNew();
            LOLConnect.User user = _ws.UserSave(userLoggedIn, token);
            elapsed.Stop();
            this.Logger.LogMessage(PrepareElapsedTimeOutput(elapsed), true);

            if (user.Errors.Count == 0 && user.EmailAddress.Equals("emailChanged@Rolo.cem"))
                this.Logger.LogMessage(this.TestSuccessMessage, true);
            else
                this.Logger.LogMessage(this.TestFailMessage, true);

            this.Logger.LogMessage(this.Delimiter, true);
            this.CleanAfterTest(this._ws);            
        }

        #endregion
    }
}
