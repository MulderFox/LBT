// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-14-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-18-2014
// ***********************************************************************
// <copyright file="RegexHelper.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace LBT.Helpers
{
    /// <summary>
    /// Class RegexHelper
    /// </summary>
    public class Regex
    {
        /// <summary>
        /// The only numbers
        /// </summary>
        public const string OnlyNumberCharacters = @"^\d*$";

        public const string OnlyCzechNumbers = @"^-?(?:\d+|\d{1,3}(?:[\s,]\d{3})+)(?:[,]\d+)?$";
        /// <summary>
        /// The email
        /// </summary>
        public const string Email = @"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?$";

        /// <summary>
        /// The password characters
        /// </summary>
        public const string PasswordCharacters = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";

        /// <summary>
        /// The lyoness id
        /// </summary>
        public const string LyonessId = @"^\d{3}\.\d{3}\.\d{3}\.\d{3}$";

        /// <summary>
        /// The time
        /// </summary>
        public const string Time = @"^([01][0-9]|2[0-3]):[0-5][0-9]$";
    }
}