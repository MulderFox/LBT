// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-29-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-29-2014
// ***********************************************************************
// <copyright file="GoogleClientJson.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using Google.Apis.Json;
using Newtonsoft.Json;

namespace LBT.Services.GoogleApis
{
    /// <summary>
    /// Class GoogleClientJson
    /// </summary>
    public sealed class GoogleClientJson
    {
        /// <summary>
        /// Loads the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>GoogleClientJson.</returns>
        public static GoogleClientJson Load(string input)
        {
            var googleClientJson = NewtonsoftJsonSerializer.Instance.Deserialize<GoogleClientJson>(input);

            if (googleClientJson != null && googleClientJson.Web != null && String.IsNullOrEmpty(googleClientJson.Web.ClientEmail))
            {
                googleClientJson.Web.ClientEmail = googleClientJson.Web.ClientId.Replace(".apps.googleusercontent.com", "@developer.gserviceaccount.com");
            }

            return googleClientJson;
        }

        /// <summary>
        /// Gets or sets the web.
        /// </summary>
        /// <value>The web.</value>
        [JsonProperty("web")]
        public GoogleClientJsonWeb Web { get; set; }
    }
}