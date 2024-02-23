using SendGrid;
using SSD_Major_Web_Project.Models;

namespace SSD_Major_Web_Project.Data.Services
{
    public interface IEmailService
    {
        Task<Response> SendSingleEmail(ComposeEmailModel payload);
    }

}
