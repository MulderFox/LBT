// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 05-12-2014
//
// Last Modified By : zmikeska
// Last Modified On : 05-12-2014
// ***********************************************************************
// <copyright file="NewtonsonJsonSerializer.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Newtonsoft.Json;
using System;
using System.IO;

namespace LBT.Helpers
{
    /// <summary>
    /// Class for serialization and deserialization of JSON documents using the Newtonsoft Library.
    /// </summary>
    public class NewtonsoftJsonSerializer
    {
        /// <summary>
        /// The newtonsoft serializer
        /// </summary>
        private static readonly JsonSerializer NewtonsoftSerializer;

        /// <summary>
        /// The _instance
        /// </summary>
        private static volatile NewtonsoftJsonSerializer _instance;
        /// <summary>
        /// The lock instance
        /// </summary>
        private static readonly object LockInstance = new object();

        /// <summary>
        /// A singleton instance of the Newtonsoft JSON Serializer.
        /// </summary>
        /// <value>The instance.</value>
        public static NewtonsoftJsonSerializer Instance
        {
            get
            {
                if (_instance == null)
                    lock (LockInstance)
                        if (_instance == null)
                            _instance = new NewtonsoftJsonSerializer();

                return _instance;
            }
        }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        public string Format
        {
            get
            {
                return "json";
            }
        }

        /// <summary>
        /// Initializes static members of the <see cref="NewtonsoftJsonSerializer"/> class.
        /// </summary>
        static NewtonsoftJsonSerializer()
        {
            var settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            settings.Converters.Add(new RFC3339DateTimeConverter());
            NewtonsoftSerializer = JsonSerializer.Create(settings);
        }

        /// <summary>
        /// Serializes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="target">The target.</param>
        public void Serialize(object obj, Stream target)
        {
            using (var writer = new StreamWriter(target))
            {
                if (obj == null)
                {
                    obj = String.Empty;
                }
                NewtonsoftSerializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// Serializes the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>System.String.</returns>
        public string Serialize(object obj)
        {
            string result;
            using (TextWriter tw = new StringWriter())
            {
                if (obj == null)
                {
                    obj = String.Empty;
                }
                NewtonsoftSerializer.Serialize(tw, obj);
                result = tw.ToString();
            }
            return result;
        }

        /// <summary>
        /// Deserializes the specified input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns>``0.</returns>
        public T Deserialize<T>(string input)
        {
            return String.IsNullOrEmpty(input) ? default(T) : JsonConvert.DeserializeObject<T>(input);
        }

        /// <summary>
        /// Deserializes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object Deserialize(string input, Type type)
        {
            return String.IsNullOrEmpty(input) ? null : JsonConvert.DeserializeObject(input, type);
        }

        /// <summary>
        /// Deserializes the specified input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns>``0.</returns>
        public T Deserialize<T>(Stream input)
        {
            T result;
            using (var streamReader = new StreamReader(input))
            {
                result = (T)(NewtonsoftSerializer.Deserialize(streamReader, typeof(T)));
            }
            return result;
        }
    }
}