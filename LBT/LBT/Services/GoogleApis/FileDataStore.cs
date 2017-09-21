// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 02-11-2014
//
// Last Modified By : zmikeska
// Last Modified On : 02-11-2014
// ***********************************************************************
// <copyright file="FileDataStore.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Google.Apis.Json;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LBT.Services.GoogleApis
{
    /// <summary>
    /// Class FileDataStore
    /// </summary>
    public class FileDataStore : IDataStore
    {
        private const string GoogleApisCacheFolder = "GoogleApisCache";

        public string FolderPath
        {
            get { return _folderPath; }
        }

        // Fields
        /// <summary>
        /// The _folder path
        /// </summary>
        private readonly string _folderPath;

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataStore" /> class.
        /// </summary>
        /// <param name="applicationPath">The application path.</param>
        /// <param name="folder">The folder.</param>
        public FileDataStore(string applicationPath, string folder)
        {
            _folderPath = Path.Combine(applicationPath, GoogleApisCacheFolder, folder);
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
        }

        /// <summary>
        /// Generates the stored key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="t">The t.</param>
        /// <returns>System.String.</returns>
        public static string GenerateStoredKey(string key, Type t)
        {
            return string.Format("{0}-{1}", t.FullName, key);
        }

        /// <summary>
        /// Asynchronously clears all values in the data store.
        /// </summary>
        /// <returns>Task.</returns>
        public Task ClearAsync()
        {
            if (Directory.Exists(_folderPath))
            {
                Directory.Delete(_folderPath, true);
                Directory.CreateDirectory(_folderPath);
            }

            return Task.Delay(0);
        }

        /// <summary>
        /// Deletes the async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentException">Key MUST have a value</exception>
        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key MUST have a value");

            string path = Path.Combine(_folderPath, GenerateStoredKey(key, typeof (T)));
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Task.Delay(0);
        }

        /// <summary>
        /// Gets the async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns>Task{``0}.</returns>
        /// <exception cref="System.ArgumentException">Key MUST have a value</exception>
        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            var source = new TaskCompletionSource<T>();
            string path = Path.Combine(_folderPath, GenerateStoredKey(key, typeof (T)));
            if (File.Exists(path))
            {
                try
                {
                    string input = File.ReadAllText(path);
                    source.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(input));
                }
                catch (Exception exception)
                {
                    source.SetException(exception);
                }
            }
            else
            {
                source.SetResult(default(T));
            }
            return source.Task;
        }

        /// <summary>
        /// Stores the async.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentException">Key MUST have a value</exception>
        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key MUST have a value");

            string contents = NewtonsoftJsonSerializer.Instance.Serialize(value);
            File.WriteAllText(Path.Combine(_folderPath, GenerateStoredKey(key, typeof (T))), contents);
            
            return Task.Delay(0);
        }
    }
}