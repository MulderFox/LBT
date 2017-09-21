// ***********************************************************************
// Assembly         : LBT.Tests
// Author           : zmikeska
// Created          : 02-06-2014
//
// Last Modified By : zmikeska
// Last Modified On : 02-06-2014
// ***********************************************************************
// <copyright file="CalendarTests.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Text;
using Google.Apis.Calendar.v3.Data;
using NUnit.Framework;
using System;
using System.IO;
using Calendar = LBT.Services.GoogleApis.Calendar;

namespace LBT.Tests.Services.GoogleApis
{
    /// <summary>
    /// Class CalendarTests
    /// </summary>
    [TestFixture]
    public class CalendarTests
    {
        /// <summary>
        /// Tests the save events.
        /// </summary>
        [Test]
        public void TestSaveEvents()
        {
            var eEvent = new Event
                             {
                                 Description = "Description",
                                 End = new EventDateTime { Date = DateTime.Today.AddDays(1).ToString("dd.MM.yyyy") },
                                 Start = new EventDateTime { Date = DateTime.Today.ToString("dd.MM.yyyy") },
                                 Summary = "Summary"
                             };

            var calendar = new Calendar();
            bool success = calendar.InsertEvents(new[] { eEvent });

            Assert.True(success);
        }

        [Test]
        public void Test()
        {
            string _folderPath = Path.Combine("..", "GoogleApisCache", "folder");
            _folderPath = Path.GetFullPath(_folderPath);

            _folderPath = _folderPath.ToUpper();
        }
    }
}
