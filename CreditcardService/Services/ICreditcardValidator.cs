using CreditcardService.Models;

namespace CreditcardService.Services
{
    public interface ICreditcardValidator
    {
        bool IsValid(CreditcardTransaction transaction);
    }
}
