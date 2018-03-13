using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hanna80_ABCMusic_Auth.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
