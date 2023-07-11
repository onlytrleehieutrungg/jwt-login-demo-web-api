using DataAccess.Models;

namespace DataAccess.Repository.Login;

public class LoginRepository : ILoginRepository
{
    private readonly PetShop2023DBContext _context;

    public LoginRepository(PetShop2023DBContext context)
    {
        _context = context;
    }

    public PetShopMember? Login(string email, string password)
    {
        var mem = _context.PetShopMembers.FirstOrDefault(
            x => x.EmailAddress.Equals(email) & x.MemberPassword.Equals(password));
        if (mem != null)
        {
            return mem;
        }

        return null;
    }
}