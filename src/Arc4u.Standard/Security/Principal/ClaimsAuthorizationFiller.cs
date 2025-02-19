﻿using Arc4u.Diagnostics;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Claims;

namespace Arc4u.Security.Principal
{
    [Export(typeof(IClaimAuthorizationFiller)), Shared]
    public class ClaimsAuthorizationFiller : IClaimAuthorizationFiller
    {
        public Authorization GetAuthorization(System.Security.Principal.IIdentity identity)
        {
            if (null == identity)
                throw new ArgumentNullException("identity");

            if (!(identity is ClaimsIdentity))
                throw new NotSupportedException("Only identity from ClaimsIdentity are allowed.");

            var claimsIdentity = (ClaimsIdentity)identity;

            // Create a UserProfile based on the identity received.
            var claimAuthorization = ExtractClaimValue(IdentityModel.Claims.ClaimTypes.Authorization, claimsIdentity.Claims);

            if (!String.IsNullOrWhiteSpace(claimAuthorization))
            {
                return GetAuthorization(claimAuthorization);
            }

            return new Authorization();

        }

        private static Authorization GetAuthorization(string claimAuthorization)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(Authorization));
                return serializer.ReadObject<Authorization>(claimAuthorization);
            }
            catch (Exception ex)
            {
                Logger.Technical.From(typeof(ClaimsAuthorizationFiller)).Exception(ex).Log();
            }

            return new Authorization();
        }

        private String ExtractClaimValue(string claimType, IEnumerable<Claim> claims)
        {
            var claim = claims.SingleOrDefault(c => c.Type.Equals(claimType, StringComparison.CurrentCultureIgnoreCase));
            try
            {
                if (null != claim)
                    return claim.Value;

                return String.Empty;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }


    }
}
