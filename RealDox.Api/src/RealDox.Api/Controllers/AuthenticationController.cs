using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealDox.Api.Filters;
using RealDox.Api.Models.AccountViewModels;
using RealDox.Api.Security;
using RealDox.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace RealDox.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly JwtValidator _jwtValidator;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IPasswordHasher<ApplicationUser> passwordHasher,
            ILoggerFactory loggerFactory,
            IOptions<JwtOptions> jwtOptions,
            JwtValidator jwtValidator)
            : base()
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtOptions.Value;
            _passwordHasher = passwordHasher;
            _jwtValidator = jwtValidator;
        }

        [AllowAnonymous]
        [ValidateModel]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            try
            {
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    FirstName = "Alexander",
                    LastName = "Martinez",
                    Title = "Mr.",
                    Email = "alexmtnezf@gmail.com",
                    EmailConfirmed = true,
                    ImageProfile = null,
                    Active = true,
                    PhoneNumber = "0000000000"
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("error", error.Description);
                }

                return BadRequest(result.Errors.Select(x => x.Description).ToList());
            }
            catch (Exception ex)
            {
                //Logger.Error($"error while creating user: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error while creating user: " + ex.Message);
            }
        }

        private JsonResult Errors(IdentityResult result)
        {
            var items = result.Errors
                .Select(x => x.Description)
                .ToArray();
            return new JsonResult(items) { StatusCode = 400 };
        }

        private JsonResult Error(string message)
        {
            return new JsonResult(message) { StatusCode = 400 };
        }

        private bool IsValidUserAndPasswordCombination(string username, string password)
        {
            return !string.IsNullOrEmpty(username) && username == password;
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateModel]
        [Route("token")]
        public async Task<IActionResult> CreateToken([FromBody]LoginViewModel model)
        {
            try
            {
                //var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                var user = await _userManager.FindByNameAsync(model.UserName);
                /*if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {

                        var claims = new[]
                        {
              new Claim(JwtRegisteredClaimNames.Sub, user.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                          _config["Tokens:Issuer"],
                          claims,
                          expires: DateTime.Now.AddDays(30),
                          signingCredentials: creds);

                        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                    }
                }*/
                if (user == null)
                {
                    await _userManager.AccessFailedAsync(user);
                    return Unauthorized();
                }

                if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) != PasswordVerificationResult.Success)
                {
                    await _userManager.AccessFailedAsync(user);
                    return Unauthorized();
                }

                var userClaims = await _userManager.GetClaimsAsync(user);
                var validTo = model.RememberMe == true
                    ? new DateTimeOffset(DateTime.Now.AddDays(_jwtOptions.RememberMeExpireInDays)).ToUnixTimeSeconds().ToString()
                    : new DateTimeOffset(DateTime.Now.AddMinutes(_jwtOptions.ExpireInMinutes)).ToUnixTimeSeconds().ToString();


                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
                    new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                    new Claim(ClaimTypes.Name, user.UserName),
                    
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                    // Token ID. We need this to maintain a blacklist for Logout purpose.
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, validTo),
                }.Union(userClaims);

                foreach(var c  in claims)
                {
                    Console.WriteLine(c.Issuer +"--"+c.Type+"--"+c.Value);
                }
                

                /*var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
                var creds = new SigningCredentials(key, _jwtOptions.SigningAlgorithm);
                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    expires: model.RememberMe == true
                        ? DateTime.Now.AddDays(_jwtOptions.RememberMeExpireInDays)
                        : DateTime.Now.AddMinutes(_jwtOptions.ExpireInMinutes),
                    signingCredentials: creds
                );*/

                var jwt = _jwtValidator.WriteToken(_jwtValidator.CreateToken(claims, _jwtOptions));

                if (_jwtOptions.UseCookie)
                {
                    SetTokenInCookie(jwt);
                }
                // Returns token as JSON.
                return Ok(new
                {
                    token = jwt,
                    expiration = validTo
                });
            }
            catch (Exception ex)
            {

                //Logger.Error($"error while creating token: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while creating token: " + ex.Message);
            }
        }

        // Authentication by filter required since we don't validate token in the method.
        [HttpPost]
        [Route("renew")]
        public IActionResult RenewToken()
        {
            try
            {
                var protectedText = GetTokenFromRequest();

                if (string.IsNullOrEmpty(protectedText))
                {
                    return Unauthorized();
                }

                var newToken = _jwtValidator.RenewToken(protectedText, _jwtOptions);

                var jwt = _jwtValidator.WriteToken(newToken);

                if (_jwtOptions.UseCookie)
                {
                    SetTokenInCookie(jwt);
                }

                return Ok(new
                {
                    token = jwt,
                    expiration = newToken.ValidTo
                });
            }
            catch (Exception ex)
            {
                //Logger.Error($"error while renew token: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error while renew token:" + ex.Message);
            }
        }

        // Authentication by filter required since we don't validate token in the method.
        [HttpPost]
        [Route("revoke")]
        public IActionResult RevokeToken()
        {
            try
            {
                var protectedText = GetTokenFromRequest();

                if (string.IsNullOrEmpty(protectedText))
                {
                    return Unauthorized();
                }

                //_jwtValidator.RevokeToken(protectedText);
                if (_jwtOptions.UseCookie)
                {
                    DeleteCookie();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                //Logger.Error($"error while revoke token: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error while revoke token: " + ex.Message);
            }
        }

        #region Private Methods

        private void SetTokenInCookie(string token)
        {
            Response.Cookies.Append(_jwtOptions.CookieName, token, new CookieOptions
            {
                HttpOnly = true,
                //Secure = true, // Todo: Enable this in production to ensure cookie only send over HTTPS.
                Path = _jwtOptions.CookiePath,
                Expires = new DateTimeOffset(DateTime.Now.AddDays(_jwtOptions.RememberMeExpireInDays)), //DateTimeOffset.Now.AddMinutes(_jwtOptions.ExpireInMinutes), //Check this
                SameSite = SameSiteMode.Strict,
                Domain = _jwtOptions.DomainName
            });
        }

        private void DeleteCookie()
        {
            Response.Cookies.Delete(_jwtOptions.CookieName);
        }

        private string GetTokenFromRequest()
        {
            string token;

            if (_jwtOptions.UseCookie)
            {
                Request.Cookies.TryGetValue(_jwtOptions.CookieName, out token);
            }
            else
            {
                Request.Headers.TryGetValue("Authorization", out var stringValues);
                var array = stringValues.FirstOrDefault()?.Split(" ");

                token = array?.Length == 2 ? array[1] : null;

            }

            return token;
        }

        #endregion
    }
}