// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-29-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-29-2014
// ***********************************************************************
// <copyright file="GoogleClientJsonWeb.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Newtonsoft.Json;

namespace LBT.Services.GoogleApis
{
    /// <summary>
    /// Class GoogleClientJsonWeb
    /// </summary>
    public sealed class GoogleClientJsonWeb
    {
        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>The client id.</value>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the client email.
        /// </summary>
        /// <value>The client email.</value>
        [JsonProperty("client_email")]
        public string ClientEmail { get; set; }
    }
}