// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 12-20-2013
//
// Last Modified By : zmikeska
// Last Modified On : 12-20-2013
// ***********************************************************************
// <copyright file="InitializeSimpleMembershipAttribute.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using LBT.DAL;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;

namespace LBT.Filters
{
    /// <summary>
    /// Class InitializeSimpleMembershipAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The _initializer
        /// </summary>
        private static SimpleMembershipInitializer _initializer;
        /// <summary>
        /// The _initializer lock
        /// </summary>
        private static object _initializerLock = new object();
        /// <summary>
        /// The _is initialized
        /// </summary>
        private static bool _isInitialized;

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        /// <summary>
        /// Class SimpleMembershipInitializer
        /// </summary>
        // ReSharper disable ClassNeverInstantiated.Local
        private class SimpleMembershipInitializer
        // ReSharper restore ClassNeverInstantiated.Local
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SimpleMembershipInitializer"/> class.
            /// </summary>
            /// <exception cref="System.InvalidOperationException">The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588</exception>
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<DefaultContext>(null);

                try
                {
                    using (var context = new DefaultContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
