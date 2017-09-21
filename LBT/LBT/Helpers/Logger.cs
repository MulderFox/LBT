// ***********************************************************************
// Assembly         : LBT
// Author           : zmikeska
// Created          : 01-25-2014
//
// Last Modified By : zmikeska
// Last Modified On : 01-25-2014
// ***********************************************************************
// <copyright file="Logger.cs" company="Zdeněk Mikeska">
//     Copyright (c) Zdeněk Mikeska. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace LBT.Helpers
{
    /// <summary>
    /// Logger level.
    /// </summary>
    public enum LoggerLevel
    {
        /// <summary>
        /// The error
        /// </summary>
        Error,
        /// <summary>
        /// The fatal
        /// </summary>
        Fatal,
        /// <summary>
        /// The info
        /// </summary>
        Info,
        /// <summary>
        /// The warn
        /// </summary>
        Warn
    }

    /// <summary>
    /// Application logger.
    /// Provides static methods for logging to avoid defining logger field in each class.
    /// Gets configuration from App.config initializing it in a static constructor.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Gets the default name of the logger.
        /// </summary>
        /// <value>The default name of the logger.</value>
        public static string DefaultLoggerName
        {
            get
            {
                lock (LockDefaultLoggerName)
                {
                    if (!String.IsNullOrEmpty(_defaultLoggerName))
                        return _defaultLoggerName;

                    _defaultLoggerName = Properties.Settings.Default.WebTitle;

                    if (String.IsNullOrEmpty(_defaultLoggerName))
                        _defaultLoggerName = "LBT";

                    return _defaultLoggerName;
                }
            }
        }

        /// <summary>
        /// The _default logger name
        /// </summary>
        private static string _defaultLoggerName;
        /// <summary>
        /// The lock default logger name
        /// </summary>
        private static readonly object LockDefaultLoggerName = new object();

        /// <summary>
        /// Gets a value indicating whether [detail debug log enabled].
        /// </summary>
        /// <value><c>true</c> if [detail debug log enabled]; otherwise, <c>false</c>.</value>
        public static bool DetailDebugLogEnabled
        {
            get
            {
                lock (LockDetailDebugLogEnabled)
                {
                    if (_detailDebugLogEnabled.HasValue)
                        return _detailDebugLogEnabled.Value;

                    _detailDebugLogEnabled = Properties.Settings.Default.DetailDebugLogEnabled;
                    return _detailDebugLogEnabled.Value;
                }
            }
        }
        /// <summary>
        /// The _detail debug log enabled
        /// </summary>
        private static bool? _detailDebugLogEnabled = new bool?();
        /// <summary>
        /// The lock detail debug log enabled
        /// </summary>
        private static readonly object LockDetailDebugLogEnabled = new object();

        /// <summary>
        /// Initializes the <see cref="Logger" /> class.
        /// </summary>
        static Logger()
        {
            XmlConfigurator.Configure();
        }

        /// <summary>
        /// Sets detail debug log information with tic.
        /// </summary>
        /// <param name="messages">Messages.</param>
        /// <returns>Invoking time.</returns>
        public static DateTime SetDetailDebugLogWithTic(params string[] messages)
        {
            try
            {
                if (!DetailDebugLogEnabled)
                    return DateTime.Now;

                StackFrame stackFrame = (new StackTrace()).GetFrame(1);
                Type declaringType = stackFrame.GetMethod().DeclaringType;
                if (declaringType != null)
                    DebugFormat(@"000;{0};Invoking {1}.{2}(0ms).", new object[] { DefaultLoggerName, declaringType.Name, stackFrame.GetMethod().Name });

                foreach (string message in messages.Where(message => !String.IsNullOrEmpty(message) && !String.IsNullOrWhiteSpace(message)))
                {
                    DebugFormat(@"000;{0};{1}", new object[] { DefaultLoggerName, message });
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }

            return DateTime.Now;
        }

        /// <summary>
        /// Sets detail debug log information with tac.
        /// </summary>
        /// <param name="tic">Invoking time.</param>
        /// <param name="message">Message.</param>
        public static void SetDetailDebugLogWithTac(DateTime tic, string message = null)
        {
            try
            {
                if (!DetailDebugLogEnabled)
                    return;

                if (!String.IsNullOrEmpty(message))
                    DebugFormat(@"000;{0};{1}", new object[] { DefaultLoggerName, message });

                StackFrame stackFrame = (new StackTrace()).GetFrame(1);
                Type declaringType = stackFrame.GetMethod().DeclaringType;
                if (declaringType != null)
                    DebugFormat(@"000;{0};Finishing {1}.{2}({3}ms).",
                                new object[]
					            {
						            DefaultLoggerName, declaringType.Name, stackFrame.GetMethod().Name,
						            (DateTime.Now - tic).TotalMilliseconds
					            });
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        /// <summary>
        /// Sets the log.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="logParameter">The log parameter.</param>
        public static void SetLog(Exception exception, LogParameter logParameter = null)
        {
            try
            {
                if (logParameter == null)
                    logParameter = new LogParameter();

                logParameter.ExceptionMessage = exception.Message;
                logParameter.StackTrace = exception.StackTrace;
                SetLog(400, logParameter);

                if (exception.InnerException != null)
                    SetLog(exception.InnerException, logParameter);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        /// <summary>
        /// Sets the log.
        /// </summary>
        /// <param name="errNo">The err no.</param>
        /// <param name="logParameter">The log parameter.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">errNo or loggerLevel.</exception>
        public static void SetLog(int errNo, LogParameter logParameter = null)
        {
            try
            {
                if (logParameter == null)
                    logParameter = new LogParameter();

                LoggerLevel loggerLevel;
                switch (errNo)
                {
                    case 1:
                        loggerLevel = LoggerLevel.Info;
                        break;

                    case 300:
                        loggerLevel = LoggerLevel.Error;
                        break;

                    case 400:
                        loggerLevel = LoggerLevel.Fatal;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("errNo");
                }

                var stringBuilder = new StringBuilder();
                if (!String.IsNullOrEmpty(logParameter.ExceptionMessage) || !String.IsNullOrEmpty(logParameter.StackTrace))
                {
                    stringBuilder.AppendLine(logParameter.ExceptionMessage);
                    stringBuilder.AppendLine(logParameter.StackTrace);
                }

                if (!String.IsNullOrEmpty(logParameter.AdditionalMessage))
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine("Additional message:");
                    stringBuilder.AppendLine(logParameter.AdditionalMessage);
                }

                string message = String.Format("{0};{1};Description: {2}",
                                               errNo.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0'), DefaultLoggerName,
                                               stringBuilder);

                switch (loggerLevel)
                {
                    case LoggerLevel.Info:
                        if (logParameter.CreateFileLog)
                            Info(message);

                        break;

                    case LoggerLevel.Warn:
                        if (logParameter.CreateFileLog)
                            Warn(message);

                        break;

                    case LoggerLevel.Error:
                    case LoggerLevel.Fatal:
                        if (logParameter.CreateFileLog)
                            Error(message);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        public static void SetErrorLog(string errorMessage)
        {
            try
            {
                string[] messages = errorMessage.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string message in messages)
                {
                    Error(String.Format("300;{0};{1};;", DefaultLoggerName, message));
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        /// <summary>
        /// Sets the log.
        /// </summary>
        /// <param name="logMessage">The log message.</param>
        public static void SetLog(string logMessage)
        {
            try
            {
                string[] messages = logMessage.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string message in messages)
                {
                    Info(String.Format("001;{0};{1};;", DefaultLoggerName, message));
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        /// <summary>
        /// Debugs format.
        /// </summary>
        /// <param name="format">Format.</param>
        /// <param name="parameters">Parameters.</param>
        private static void DebugFormat(string format, params object[] parameters)
        {
            DebugFormat(DefaultLoggerName, format, parameters);
        }

        /// <summary>
        /// Debugs format.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="format">Format.</param>
        /// <param name="parameters">Parameters.</param>
        private static void DebugFormat(string name, string format, params object[] parameters)
        {
            LogManager.GetLogger(name).DebugFormat(format, parameters);
        }

        /// <summary>
        /// Logs informative message.
        /// </summary>
        /// <param name="message">Message.</param>
        private static void Info(object message)
        {
            Info(DefaultLoggerName, message);
        }

        /// <summary>
        /// Logs informative message.
        /// </summary>
        /// <param name="name">Logger name.</param>
        /// <param name="message">Message.</param>
        private static void Info(string name, object message)
        {
            LogManager.GetLogger(name).Info(message);
        }

        /// <summary>
        /// Logs warning message.
        /// </summary>
        /// <param name="message">Message.</param>
        private static void Warn(object message)
        {
            Warn(DefaultLoggerName, message);
        }

        /// <summary>
        /// Logs warning message.
        /// </summary>
        /// <param name="name">Logger name.</param>
        /// <param name="message">Message.</param>
        private static void Warn(string name, object message)
        {
            LogManager.GetLogger(name).Warn(message);
        }

        /// <summary>
        /// Logs error message.
        /// </summary>
        /// <param name="message">Message.</param>
        private static void Error(object message)
        {
            Error(DefaultLoggerName, message);
        }

        /// <summary>
        /// Logs error message.
        /// </summary>
        /// <param name="name">Logger name.</param>
        /// <param name="message">Message.</param>
        private static void Error(string name, object message)
        {
            LogManager.GetLogger(name).Error(message);
        }

        /// <summary>
        /// Class LogParameter
        /// </summary>
        public class LogParameter
        {
            /// <summary>
            /// The additional message
            /// </summary>
            public string AdditionalMessage;
            /// <summary>
            /// The address
            /// </summary>
            public string Address;
            /// <summary>
            /// The build
            /// </summary>
            public string Build;
            /// <summary>
            /// The create file log
            /// </summary>
            public bool CreateFileLog;
            /// <summary>
            /// The exception message
            /// </summary>
            public string ExceptionMessage;
            /// <summary>
            /// The filename
            /// </summary>
            public string Filename;
            /// <summary>
            /// The GUID
            /// </summary>
            public Guid Guid;
            /// <summary>
            /// The HTTP status code
            /// </summary>
            public HttpStatusCode HttpStatusCode;
            /// <summary>
            /// The model
            /// </summary>
            public string Model;
            /// <summary>
            /// The parameter
            /// </summary>
            public string Parameter;
            /// <summary>
            /// The position
            /// </summary>
            public int Position;
            /// <summary>
            /// The query
            /// </summary>
            public string Query;
            /// <summary>
            /// The scene
            /// </summary>
            public string Scene;
            /// <summary>
            /// The stack trace
            /// </summary>
            public string StackTrace;
            /// <summary>
            /// The URI
            /// </summary>
            public string Uri;
            /// <summary>
            /// The user agent
            /// </summary>
            public string UserAgent;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogParameter"/> class.
            /// </summary>
            public LogParameter()
            {
                CreateFileLog = true;
                HttpStatusCode = HttpStatusCode.InternalServerError;
            }
        }
    }
}
