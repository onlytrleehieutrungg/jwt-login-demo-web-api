using DataAccess.Models;

namespace DataAccess.Repository.Login;

public interface ILoginRepository
{
    PetShopMember? Login(string email, string password);
}