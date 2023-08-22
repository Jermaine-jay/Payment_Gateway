using Payment_Gateway.Shared.DataTransferObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment_Gateway.BLL.Interfaces
{
    public interface IPaymentServiceExtension
    {
        Task<object> CreatePayment(string userId, PaymentResponse response);
    }
}
