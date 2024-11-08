using DataBaseAccess;
using ModelClasses;

namespace ECommerce.Repository.UserRepo
{
    public class UserRepository:IUserRepository
    {
        private readonly AppDBContext context;

        public UserRepository(AppDBContext _context)
        {
            context = _context;
        }

        public List<ApplicationUser> GetAllUser()
        {
            List<ApplicationUser> user = context.AppUser.ToList();
            return user;
        }
        public ApplicationUser? UserById(string id)
        {
            ApplicationUser? user = context.AppUser.FirstOrDefault(c => c.Id == id);

            if (user == null)
            {
                return null; // or you can return a specific value like -1 to indicate not found
            }

            return user;
        }

        public void RemoveUser(string id)
        {
            ApplicationUser user = context.AppUser.FirstOrDefault(c => c.Id == id);
            context.AppUser.Remove(user);
            context.SaveChanges();
        }
        public void UpdateUser(ApplicationUser user, string id)
        {
            if (user == null || string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Invalid user or ID");
            }

            var oldUser = context.AppUser.FirstOrDefault(c => c.Id == id);
            if (oldUser == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Editable fields
            oldUser.FirstName = user.FirstName;
            oldUser.LastName = user.LastName;
            oldUser.Email = user.Email;  // only if admins have permission to edit email
            oldUser.Address = user.Address;
            oldUser.City = user.City;
            oldUser.PhoneNumber = user.PhoneNumber;

            context.SaveChanges();
        }






    }
}
