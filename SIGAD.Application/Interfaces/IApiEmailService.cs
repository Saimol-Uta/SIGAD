using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.Interfaces
{
    public interface IApiEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
