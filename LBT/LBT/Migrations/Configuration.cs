// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-26-2014
//
// Last Modified By : zmikeska
// Last Modified On : 05-09-2014
// ***********************************************************************
// <copyright file="Configuration.cs" company="Zdenìk Mikeska">
//     Copyright (c) Zdenìk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace LBT.Migrations
{
    using System.Data.Entity.Migrations;

    /// <summary>
    /// Class Configuration
    /// </summary>
    internal sealed class Configuration : DbMigrationsConfiguration<DAL.DefaultContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        /// <summary>
        /// Seeds the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void Seed(DAL.DefaultContext context)
        {
            ConfigurationSeed.SeedContext(context);
        }
    }
}
