using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace UserControlUI.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        //private readonly IUnitOfWork _uof;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }
        //protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        //{
        //if (!Request.Headers.ContainsKey("Authorization"))
        //{
        //    return AuthenticateResult.Fail("Authorization was not found");
        //}
        //try
        //{
        //    var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        //    string[] credentials = authenticationHeaderValue.Parameter.Split(":");
        //    string emailAddress = credentials[0];
        //    string password = credentials[1];
        //    IEnumerable<User> users = await _uof.UserRepository.Get();
        //    if ((users != null) && (users.Count() > 0))
        //    {
        //        foreach (User user in users)
        //        {
        //            if ((user.EmailAddress == emailAddress) && (user.Password == password))
        //            {
        //                var claims = new[] { new Claim(ClaimTypes.Name, user.EmailAddress)};
        //                var identity = new ClaimsIdentity(claims, Scheme.Name);
        //                var principal = new ClaimsPrincipal(identity);
        //                var ticker = new AuthenticationTicket(principal, Scheme.Name);
        //                return AuthenticateResult.Success(ticker);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return AuthenticateResult.Fail("Invalid username or password");
        //    }
        //    return AuthenticateResult.Fail("Invalid username or password");
        //}

        //catch(Exception)
        //{
        //    return AuthenticateResult.Fail("Error has occured");
        //}
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return AuthenticateResult.Fail("Mot implemented");
        }

    }
}
