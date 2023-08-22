using Payment_Gateway.DAL.Paystack.Request;
using Payment_Gateway.DAL.Paystack.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment_Gateway.BLL.Paystack.Interfaces
{
    public interface ICharge
    {
        Task<ChargeResponseModel> ChargeCard(string email, string amount, string pin, string cvv,
            string expiry_month, string expiry_year, string number, string display_name = null, string value = null,
            string variable_name = null);
    }



}
