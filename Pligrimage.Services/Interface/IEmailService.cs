using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pligrimage.Services.Interface
{
    public interface IEmailService
    {
     
        Task CareerPeformSendEamil(string emailTo, string evaluation, string emailType);

        Task<bool> SendEmailToAllofficers();

        Task FeedbackSendEmail(string emailTo);

        
    }
}
