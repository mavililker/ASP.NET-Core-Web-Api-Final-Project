using EventProjectFinal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EventProjectFinal.Tokens
{
    public class AccessTokenGenerator
    {
        public EVENTS3Context _context { get; set; }
        public IConfiguration _config { get; set; }
        public AppUser _appUser { get; set; }


        public AccessTokenGenerator(EVENTS3Context context,
                                    IConfiguration config,
                                    AppUser appUser)
        {
            _config = config;
            _context = context;
            _appUser = appUser;
        }


        public AppUserTokens GetToken()
        {
            AppUserTokens userTokens = null;
            TokenINFO tokenInfo = null;

            if (_context.AppUserTokens.Count(x => x.UserId == _appUser.Id) > 0)
            {

                userTokens = _context.AppUserTokens.FirstOrDefault(x => x.UserId == _appUser.Id);


                if (userTokens.ExpireDate <= DateTime.Now)
                {

                    tokenInfo = GenerateToken();

                    userTokens.ExpireDate = tokenInfo.ExpireDate;
                    userTokens.Value = tokenInfo.Token;

                    _context.AppUserTokens.Update(userTokens);
                }
            }
            else
            {

                tokenInfo = GenerateToken();

                userTokens = new AppUserTokens();

                userTokens.UserId = _appUser.Id;
                userTokens.LoginProvider = "SystemAPI";
                userTokens.Name = _appUser.Name;
                userTokens.ExpireDate = tokenInfo.ExpireDate;
                userTokens.Value = tokenInfo.Token;

                _context.AppUserTokens.Add(userTokens);
            }

            _context.SaveChangesAsync();

            return userTokens;
        }

        public async Task<bool> DeleteToken()
        {
            bool ret = true;

            try
            {
                if (_context.AppUserTokens.Count(x => x.UserId == _appUser.Id) > 0)
                {
                    AppUserTokens userTokens = userTokens = _context.AppUserTokens.FirstOrDefault(x => x.UserId == _appUser.Id);

                    _context.AppUserTokens.Remove(userTokens);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }

        private TokenINFO GenerateToken()
        {
            DateTime expireDate = DateTime.Now.AddSeconds(600);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Application:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _config["Application:Audience"],
                Issuer = _config["Application:Issuer"],
                Subject = new ClaimsIdentity(new Claim[]
                {

                    new Claim(ClaimTypes.NameIdentifier, _appUser.Id),
                    new Claim(ClaimTypes.Name, _appUser.Name),
                    new Claim(ClaimTypes.Email, _appUser.Email),
                    ///Adding roles in claims.
                    new Claim(ClaimTypes.Role, _appUser.Role)
                }),


                Expires = expireDate,


                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            TokenINFO tokenInfo = new TokenINFO();

            tokenInfo.Token = tokenString;
            tokenInfo.ExpireDate = expireDate;

            return tokenInfo;
        }


    }
}
