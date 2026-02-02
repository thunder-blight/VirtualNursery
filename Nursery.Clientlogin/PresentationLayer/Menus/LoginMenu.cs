using Nursery.Clientlogin.Models;
using Nursery.Clientlogin.Services;

namespace  Nursery.Clientlogin.PresentationLayer.Menus
{
    public static class LoginMenu
    {
        public static User? LoginUser()
        {
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine()?.Trim() ?? "";
            
            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine()?.Trim() ?? "";

            try
            {
                User user = AuthServices.Login(username, password);
                Console.WriteLine($"User '{user.Username}' successfully logged in.");
                return user;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                return null;
            }
        }
        
        public static User? RegisterUser()
        {
            Console.WriteLine("=== Register New User ===");
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine()?.Trim() ?? "";

            string password;
            string confirmPassword;

            while (true)
            {
                Console.WriteLine("Enter password: ");
                password = Console.ReadLine()?.Trim() ?? "";
                
                Console.WriteLine("Confirm password: ");
                confirmPassword = Console.ReadLine()?.Trim() ?? "";

                if (password == confirmPassword)
                    break;
                
                Console.WriteLine("Passwords do not match. Please try again.");
            }

            try
            {
                User user = AuthServices.Register(username, password);
                Console.WriteLine($"User '{user.Username}' successfully registered.!");
                return user;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }

        }
    }
}