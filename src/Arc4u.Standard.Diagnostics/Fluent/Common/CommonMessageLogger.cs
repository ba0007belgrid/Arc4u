﻿using Microsoft.Extensions.Logging;
using System;

namespace Arc4u.Diagnostics
{
    public class CommonMessageLogger
    {
        private LoggerMessage MessageLogger { get; set; }

        internal CommonMessageLogger(ILogger logger, MessageCategory category, string typeClass, string methodName)
        {
            MessageLogger = new LoggerMessage(logger, category, methodName, typeClass);
        }

        private CommonLoggerProperties AddEntry(LogLevel logLevel, string message, Exception exception = null, object[] args = null)
        {
            MessageLogger.LogLevel = logLevel;
            MessageLogger.Text = message;
            MessageLogger.Args = args;
            MessageLogger.Exception = exception;


            return new CommonLoggerProperties(MessageLogger);
        }

        public CommonLoggerProperties Debug(string message, params object[] args)
        {
            return AddEntry(LogLevel.Debug, message, null, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            var buildMessage = AddEntry(LogLevel.Debug, message, null, args);
            buildMessage.Log();
        }

        public CommonLoggerProperties Information(string message, params object[] args)
        {
            return AddEntry(LogLevel.Information, message, null, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            var buildMessage = AddEntry(LogLevel.Information, message, null, args);
            buildMessage.Log();
        }

        public CommonLoggerProperties Warning(string message, params object[] args)
        {
            return AddEntry(LogLevel.Warning, message, null, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            var buildMessage = AddEntry(LogLevel.Warning, message, null, args);
            buildMessage.Log();
        }

        public CommonLoggerProperties Fatal(string message, params object[] args)
        {
            return AddEntry(LogLevel.Critical, message, null, args);
        }

        public void LogFatal(string message, params object[] args)
        {
            var buildMessage = AddEntry(LogLevel.Critical, message, null, args);
            buildMessage.Log();
        }

        public CommonLoggerProperties Error(string message, params object[] args)
        {
            return AddEntry(LogLevel.Error, message, null, args);
        }

        public void LogError(string message, params object[] args)
        {
            var buildMessage = AddEntry(LogLevel.Error, message, null, args);
            buildMessage.Log();
        }

        public CommonLoggerProperties Exception(Exception exception)
        {
            var property = AddEntry(LogLevel.Error, exception.Message, exception);
            return property.AddStacktrace(exception.StackTrace ?? Environment.StackTrace);
        }

        public void LogException(Exception exception)
        {
            var property = AddEntry(LogLevel.Error, exception.Message, exception);
            var buildMessage = property.AddStacktrace(exception.StackTrace ?? Environment.StackTrace);
            buildMessage.Log();
        }

        internal CommonLoggerProperties System(string message, params object[] args)
        {
            return AddEntry(LogLevel.Trace, message, null, args);
        }
    }
}
