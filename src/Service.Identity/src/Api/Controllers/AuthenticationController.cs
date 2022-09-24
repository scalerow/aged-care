using Giantnodes.Infrastructure.Extensions;
using Giantnodes.Service.Identity.Domain.Identity;
using Giantnodes.Service.Identity.Persistence;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Collections.Immutable;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Giantnodes.Service.Identity.Api.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOpenIddictScopeManager _scopeManager;

        public AuthenticationController(ApplicationDbContext database, SignInManager<ApplicationUser> signinManager, UserManager<ApplicationUser> userManager, IOpenIddictScopeManager scopeManager)
        {
            this._signinManager = signinManager;
            this._userManager = userManager;
            this._scopeManager = scopeManager;
        }

        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        public async Task<IActionResult> TokenExchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByEmailAsync(request.Username);
                if (user == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username and or password is invalid."
                        }));
                }

                // validate the username and password parameters and ensure the account is not locked out
                var result = await _signinManager.CheckPasswordSignInAsync(user, request.Password, true);
                if (!result.Succeeded)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username and or password is invalid."
                        }));
                }

                // create a claims-based identity that will be used by OpenIddict to generate tokens
                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

                identity
                    .AddClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                    .AddClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                    .AddClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                    .AddClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

                identity.SetScopes(request.GetScopes());
                identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
                identity.SetDestinations(GetDestinations);

                // returning a SignInResult will ask OpenIddict to issue the appropriate access / identity tokens
                return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsRefreshTokenGrantType())
            {
                // retrieve the claims principal stored in the authorization refresh token
                var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
                if (principal == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ExpiredToken,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                        }));
                }

                // retrieve the user profile corresponding to the authorization code/refresh token
                var user = await _userManager.FindByIdAsync(principal.GetClaim(Claims.Subject));
                if (user == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ExpiredToken,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                        }));
                }

                // ensure the user is still allowed to sign in
                if (!await _signinManager.CanSignInAsync(user))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                        }));
                }

                principal.SetDestinations(GetDestinations);

                // returning a SignInResult will ask OpenIddict to issue the appropriate access / identity tokens
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });
        }

        private IEnumerable<string> GetDestinations(Claim claim)
        {
            // note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.
            switch (claim.Type)
            {

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (claim.Subject?.HasScope(Scopes.Email) ?? false)
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (claim.Subject?.HasScope(Scopes.Profile) ?? false)
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (claim.Subject?.HasScope(Scopes.Roles) ?? false)
                        yield return Destinations.IdentityToken;

                    yield break;

                // never include the security stamp in the access and identity tokens as it's a secret value
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}
