using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment_Gateway.BLL.Infrastructure.Security
{
    public enum CacheKeyPrefix
    {
        /// <summary>
        /// Key prefix for otp code records
        /// </summary>
        OtpCode = 1,
        /// <summary>
        /// Key prefix for account lockout records
        /// </summary>
        AccountLockout = 1
    }
}
