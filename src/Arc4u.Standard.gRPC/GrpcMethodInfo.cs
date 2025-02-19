﻿using Arc4u.Dependency.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arc4u.gRPC
{
    [Export, Shared]
    public class GrpcMethodInfo
    {
        private readonly Dictionary<String, ServiceAspectAttribute> RightsOnMethod;

        public GrpcMethodInfo()
        {
            RightsOnMethod = new Dictionary<string, ServiceAspectAttribute>();
        }

        /// <summary>
        /// Register the service aspect attached to the method and cache it for performance.
        /// </summary>
        /// <param name="method">The gRPC method called.</param>
        /// <param name="serviceType">The service implementing the method.</param>
        /// <returns>A service aspect, empty one if no service aspect is defined.</returns>
        public ServiceAspectAttribute GetAttributeFor(String method, Type serviceType)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException(nameof(method));

            if (null == serviceType)
                throw new ArgumentException(nameof(serviceType));

            if (RightsOnMethod.ContainsKey(method))
                return RightsOnMethod[method];

            var methodSplit = method.Split('/');
            var requestedMethodName = methodSplit.Last();

            // find the attribute in the selected method.
            var methodInfo = serviceType.GetMethod(requestedMethodName);

            // should not be possible!
            if (null == methodInfo)
                return RegisterEmptyAspectForMethod(method);

            var serviceAspects = methodInfo.GetCustomAttributes(typeof(ServiceAspectAttribute), true).Cast<ServiceAspectAttribute>();

            // Allow multiple has been set to false => should not be possible to have more than one by design.
            var serviceAspect = serviceAspects.FirstOrDefault();

            if (null == serviceType)
                return RegisterEmptyAspectForMethod(method);

            RightsOnMethod[method] = serviceAspect;

            return serviceAspect;

        }

        private ServiceAspectAttribute RegisterEmptyAspectForMethod(string method)
        {
            var empty = ServiceAspectAttribute.Empty();
            RightsOnMethod[method] = empty;
            return empty;
        }
    }
}
