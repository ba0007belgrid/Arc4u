﻿using Microsoft.AspNetCore.Builder;
using System;

namespace Arc4u.Standard.AspNetCore.Middleware
{
    public static class LogGrpcMonitoringTimeElapsedMiddlewareExtension
    {
        public static IApplicationBuilder AddGrpcMonitoringTimeElapsed(this IApplicationBuilder app, Action<Type, TimeSpan> extraLog = null)
        {
            if (null == app)
                throw new ArgumentNullException(nameof(app));

            if (null != extraLog)
                return app.UseMiddleware<LogGrpcMonitoringTimeElapsedMiddleware>(extraLog);

            return app.UseMiddleware<LogGrpcMonitoringTimeElapsedMiddleware>();
        }
    }
}
