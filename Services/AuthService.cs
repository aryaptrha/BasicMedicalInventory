using InitialSetupMVC.Models;
using InitialSetupMVC.Repositories;

namespace InitialSetupMVC.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User? ValidateUser(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }

            if (AuthHelper.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }
    }
}
