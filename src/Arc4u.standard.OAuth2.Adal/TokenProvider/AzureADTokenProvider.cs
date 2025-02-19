﻿using Arc4u.Dependency;
using Arc4u.OAuth2.Configuration;
using Arc4u.OAuth2.Token;
using Arc4u.OAuth2.Token.Adal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Arc4u.OAuth2.TokenProvider
{
    [System.Composition.Export(AdalTokenProvider.ProviderName, typeof(ITokenProvider))]
    class AzureADTokenProvider : AdalTokenProvider
    {
        [System.Composition.ImportingConstructor]
        public AzureADTokenProvider(OAuthConfig oAuthConfig, ILogger logger, IContainerResolve container) : base(oAuthConfig, logger, container) { }


        protected override AuthenticationContext CreateAuthenticationContext(string authority, string cacheIdentifier)
        {
            return new AuthenticationContext(authority, true, new Cache(Logger, Container, cacheIdentifier));
        }
    }
}
