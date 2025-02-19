﻿using Arc4u.Diagnostics;
using Arc4u.Security.Principal;
using Arc4u.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arc4u.OAuth2.Aspect
{
    /// <summary>
    /// This attribute checks:
    /// - the security. if user is authenticated at least and if the necessary rihts are assigned to the user.
    /// - Keep the time to complete the call.
    /// - Allow to set or not the culture.
    /// Handle AppException or Exception and return a Bad RequestMessage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public abstract class ServiceAspectBase : ActionFilterAttribute, IAsyncAuthorizationFilter, IExceptionFilter
    {
        /// <summary>
        /// The page used to render when you are unauthorized.
        /// </summary>
        private readonly int[] _operations;
        private readonly String _scope = string.Empty;
        protected readonly ILogger Logger;
        protected readonly IApplicationContext ApplicationContext;

        private static Action<Type, TimeSpan> _log = null;

        public ServiceAspectBase(ILogger logger, IApplicationContext applicationContext, String scope, params int[] operations)
        {
            ApplicationContext = applicationContext;
            Logger = logger;
            _scope = scope;
            _operations = operations;
        }

        public abstract void SetCultureInfo(ActionExecutingContext context);

        public static void SetExtraLogging(Action<Type, TimeSpan> log)
        {
            _log = log;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Thread.CurrentPrincipal = ApplicationContext.Principal;

            SetCultureInfo(context);

            await next().ConfigureAwait(true);
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (null != ApplicationContext.Principal && ApplicationContext.Principal.IsAuthorized(_scope, _operations))
            {
                return Task.CompletedTask;
            }

            context.Result = new UnauthorizedResult();

            return Task.CompletedTask;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
                Logger.Technical().From(descriptor.MethodInfo.DeclaringType, descriptor.MethodInfo.Name).Exception(context.Exception).Log();
            else
                Logger.Technical().From(typeof(ServiceAspectBase)).Exception(context.Exception).Log();

            Messages messages;
            if (context.Exception is AppException appException)
            {
                messages = Messages.FromEnum(appException.Messages);
                messages.LocalizeAll();
            }
            else
            {
                messages = new Messages
                {
                    new Message(Arc4u.ServiceModel.MessageCategory.Technical, Arc4u.ServiceModel.MessageType.Error, "A technical error occured.")
                };
            }

            context.Result = new BadRequestObjectResult(messages);
        }
    }
}
