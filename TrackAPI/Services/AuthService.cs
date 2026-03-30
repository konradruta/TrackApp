using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrackAPI;
using TrackAPI.Entities;
using TrackAPI.Exceptions;
using TrackAPI.Models;

namespace VueApp1.Server.Services
{
    public interface IAuthService
    {
        string LoginUser(LoginUserDto dto);
        void Register(RegisterUserDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly TrackDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly ILogger<AuthService> _logger;
        public AuthService(TrackDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings, ILogger<AuthService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _logger = logger;
        }

        public string LoginUser(LoginUserDto dto)
        {
            var user = _dbContext.Users
                .Include(r => r.Role)
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
            {
                throw new WrongLoginException("Wrong e-mail or passowrd");
            }

            var verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (verifyPassword == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning($"Nieudane logowanie dla użytkownika: {user.Email}");
                throw new WrongLoginException("Wrong e-mail or passowrd");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, dto.Email),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JWTKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JWTExpiresDays);

            var token = new JwtSecurityToken(_authenticationSettings.JWTIssuer,
                _authenticationSettings.JWTIssuer,
                claims: claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            _logger.LogWarning($"Udane logowanie dla użytkownika: {user.Email}");

            return tokenHandler.WriteToken(token);
        }

        public void Register(RegisterUserDto dto)
        {
            var newAccount = _mapper.Map<User>(dto);

            var password = _passwordHasher.HashPassword(newAccount, dto.Password);
            newAccount.PasswordHash = password;

            _dbContext.Users.Add(newAccount);
            _dbContext.SaveChanges();
        }
    }
}
