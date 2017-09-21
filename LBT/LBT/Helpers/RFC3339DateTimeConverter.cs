// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 05-12-2014
//
// Last Modified By : zmikeska
// Last Modified On : 05-12-2014
// ***********************************************************************
// <copyright file="RFC3339DateTimeConverter.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace LBT.Helpers
{
    /// <summary>
    /// A JSON converter which honers RFC 3339 and the serialized date is accepted by Google services.
    /// </summary>
    public class RFC3339DateTimeConverter : JsonConverter
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
        /// </summary>
        /// <value><c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.</value>
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <exception cref="System.NotImplementedException">Unnecessary because CanRead is false.</exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new Exception("Unnecessary because CanRead is false.");
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            var date = (DateTime)value;
            serializer.Serialize(writer, ConvertToRFC3339(date));
        }

        /// <summary>
        /// Converts to RF C3339.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        internal static string ConvertToRFC3339(DateTime date)
        {
            if (date.Kind != DateTimeKind.Utc)
            {
                date = date.ToUniversalTime();
            }
            return date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
