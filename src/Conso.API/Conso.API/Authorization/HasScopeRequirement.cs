using Microsoft.AspNetCore.Authorization;

namespace Conso.API.Authorization
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string Scope { get; }

        public HasScopeRequirement(string scope, string issuer)
        {
            ArgumentNullException.ThrowIfNull(scope);
            ArgumentNullException.ThrowIfNull(issuer);

            Scope = scope;
            Issuer = issuer;
        }
    }
}
