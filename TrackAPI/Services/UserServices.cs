using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrackAPI.Entities;
using TrackAPI.Exceptions;
using TrackAPI.Models;

namespace TrackAPI.Services
{
    public interface IUserServices
    {
        IEnumerable<UserDto> GetAccounts();
        UserDto GetAccountById(int id);
    }

    public class UserServices : IUserServices
    {
        private readonly TrackDbContext _db;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserServices(TrackDbContext db, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public IEnumerable<UserDto> GetAccounts()
        {
            var accounts = _db.Users
                .Include(r => r.Role)
                .ToList();

            var accountsMaps = _mapper.Map<List<UserDto>>(accounts);

            return accountsMaps;
        }

        public UserDto GetAccountById(int id)
        {
            var account = _db.Users
                .Include(r => r.Role)
                .FirstOrDefault(a => a.Id == id);

            if (account == null)
            {
                throw new NotUserFoundException("User in this id dosen't exists");
            }

            var accountMap = _mapper.Map<UserDto>(account);

            return accountMap;
        }


    }
}
