using ModelClasses;

namespace ECommerce.Repository.UserRepo
{
    public interface IUserRepository
    {
        public List<ApplicationUser> GetAllUser();
        public ApplicationUser? UserById(string id);
      
        public void RemoveUser(string id);
        public void UpdateUser(ApplicationUser User, string id);

    }
}
