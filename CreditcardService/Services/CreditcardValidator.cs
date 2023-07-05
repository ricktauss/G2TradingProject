using CreditcardService.Models;

namespace CreditcardService.Services
{
    public class CreditcardValidator : ICreditcardValidator
    {
        public bool IsValid(CreditcardTransaction transaction)
        {
            return !String.IsNullOrEmpty(transaction?.CreditcardType) ;
        }
    }
}
