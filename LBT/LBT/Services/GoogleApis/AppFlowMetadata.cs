// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-29-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-29-2014
// ***********************************************************************
// <copyright file="AppFlowMetadata.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using LBT.Helpers;
using System;
using System.Web.Mvc;

namespace LBT.Services.GoogleApis
{
    public class AppFlowMetadata : FlowMetadata, IDisposable
    {
        #region Consts

        private const string FileDataStoreFolder = "Calendar.Api.Auth.Store";

        #endregion

        #region Properties

        public bool Valid { get; private set; }

        public override IAuthorizationCodeFlow Flow
        {
            get { return _flow; }
        }

        public bool AccessDenied { get; set; }

        #endregion

        #region Fields

        private readonly string _googleUserId;
        private IAuthorizationCodeFlow _flow;

        #endregion

        #region Constructors & Destructors

        public AppFlowMetadata(string applicationPath, string googleCredentialsJson)
        {
            try
            {
                GoogleClientJson googleClientJson = GoogleClientJson.Load(googleCredentialsJson);

                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                                      {
                                          ClientSecrets = new ClientSecrets
                                                              {
                                                                  ClientId = googleClientJson.Web.ClientId,
                                                                  ClientSecret = googleClientJson.Web.ClientSecret
                                                              },
                                          Scopes = new[] {CalendarService.Scope.Calendar},
                                          DataStore = new FileDataStore(applicationPath, FileDataStoreFolder)
                                      };
                _flow = new GoogleAuthorizationCodeFlow(initializer);
                _googleUserId = googleClientJson.Web.ClientEmail;

                Valid = true;
            }
            catch (Exception e)
            {
                Logger.SetLog(e);
                Valid = false;
            }
        }

        ~AppFlowMetadata()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_flow != null)
            {
                _flow.Dispose();
                _flow = null;
            }
        }

        #endregion

        public override string GetUserId(Controller controller)
        {
            return _googleUserId;
        }

        public string GetUserId()
        {
            return _googleUserId;
        }
    }
}