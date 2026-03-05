//using ITS.CareerPeform.Core.EmailModel;
//using ITS.CareerPeform.Data;
//using MailKit.Net.Smtp;
//using MailKit.Security;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;
//using MimeKit;
//using MimeKit.Text;
//using MimeKit.Utils;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Pligrimage.Services.Implementation
//{
//    public class EmailService : IEmailService
//    {
//        private readonly EmailConfig ec;

//        private readonly AppDbContext _context;


//        public EmailService(IOptions<EmailConfig> emailConfig, AppDbContext context)
//        {
//            ec = emailConfig.Value;
//            _context = context;
//        }

//        public async Task CareerPeformSendEamil(string emailTo, string evaluation, string emailType)
//        {

//            emailTo = (emailTo + "@mod.saf").Trim();
//            var today = DateTime.Now.Day.ToString() + " " + GetMonth() + " " + DateTime.Now.Year.ToString() + "  بتوقيت  " + string.Format("{0:HH:MM}", DateTime.Now);


//            StringBuilder sbEmailTemplate = new StringBuilder();
//            string strSubject = String.Empty;

//            if (emailType == "ToEvaluation")
//            {
//                strSubject = " تجربة // منظومة الأداء الوظيفي";

//                sbEmailTemplate.Append("<div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br />");
//                sbEmailTemplate.Append("  عزيزي المستخدم،   <br /><br/>");
//                sbEmailTemplate.Append("((تجربة المنظومة )) الموضوع  :  تقييم الأداء الوظيفي   <br /> <br />");
//                sbEmailTemplate.Append(" تم تعيينكم من قبل فرع الضباط لتقييم   " + evaluation + " في منظومة الأداء الوظيفي <br />");
//                sbEmailTemplate.Append(" للدخول على النظام أضغط  :  " + "<a href='http://CareerPerformance:8090/' tiltle='منظومة  الأداء الوظيفي'>" + "هنا </a>" + "   <br /> <br />");
//                sbEmailTemplate.Append("  التاريخ " + today + "  <br />");
//                sbEmailTemplate.Append("  </div>");
//                sbEmailTemplate.Append("<div  style='direction:rtl;font:16px; font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br /><br /><br /><br />");
//                sbEmailTemplate.Append(" ----------------------<br />");
//                sbEmailTemplate.Append(" خدمات تقنية المعلومات<br />");
//                sbEmailTemplate.Append(" منظومة الأداء الوظيفي<br />");
//                sbEmailTemplate.Append("  </div>");


//            }

//            else if (emailType == "ToApproveEvaluation")
//            {

//                strSubject = " تجربة // منظومة الأداء الوظيفي";

//                sbEmailTemplate.Append("<div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br />");
//                sbEmailTemplate.Append("  عزيزي المستخدم،   <br /><br/>");
//                sbEmailTemplate.Append("((تجربة المنظومة )) الموضوع  :  تقييم الأداء الوظيفي   <br /> <br />");
//                sbEmailTemplate.Append(" تم إنشاء التقييم الخاص بكم من قبل الـ" + evaluation + " في منظومة الإداء الوظيفي <br /><br />");
//                sbEmailTemplate.Append(" للتأكيد على التقييم يرجى الدخول على المنظومة أضغط  :  " + "<a href='http://CareerPerformance:8090/' tiltle='منظومة  الأداء الوظيفي'>" + "هنا </a>" + "   <br /> <br />");
//                sbEmailTemplate.Append("  التاريخ " + today + "  <br /><br />");
//                sbEmailTemplate.Append("  </div>");
//                sbEmailTemplate.Append("<div  style='direction:rtl;font:16px; font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br /><br /><br /><br /><br />");
//                sbEmailTemplate.Append(" ----------------------<br />");
//                sbEmailTemplate.Append(" خدمات تقنية المعلومات<br />");
//                sbEmailTemplate.Append(" منظومة الأداء الوظيفي<br />");
//                sbEmailTemplate.Append("  </div>");

//            }

//            else if (emailType == "ToApproveEvaluationReconsider") // يرسل الى الضباط للتصديق على إعادة التقييم 
//            {
//                strSubject = " تجربة // منظومة الأداء الوظيفي";

//                sbEmailTemplate.Append("<div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br />");
//                sbEmailTemplate.Append("  عزيزي المستخدم،   <br /><br/>");
//                sbEmailTemplate.Append("((تجربة المنظومة )) الموضوع  :  تقييم الأداء الوظيفي   <br /> <br />");
//                sbEmailTemplate.Append(" لقد قـام     " + evaluation + " بإعادة النظر في التقييم الخاص بك    <br />");
//                sbEmailTemplate.Append(" لذا يرجى التكرم بالتصديق على التقييم ، علما بأنه لايمكنك إعادة التقييم الى الضابط المقيم ، للدخول الى المنظومة   :  " + "<a href='http://10.20.19.103/' tiltle='منظومة  الأداء الوظيفي'>" + "هنا </a>" + "   <br />");
//                sbEmailTemplate.Append("  التاريخ " + today + "  <br /><br />");
//                sbEmailTemplate.Append("  </div>");
//                sbEmailTemplate.Append("<div  style='direction:rtl;font:16px; font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br /><br /><br /><br /><br />");
//                sbEmailTemplate.Append(" ----------------------<br />");
//                sbEmailTemplate.Append(" خدمات تقنية المعلومات<br />");
//                sbEmailTemplate.Append(" منظومة الأداء الوظيفي<br />");
//                sbEmailTemplate.Append("  </div>");

//            }



//            else if (emailType == "ToApproveEvaluationUndirectOfficer")
//            {
//                strSubject = " تجربة // منظومة الأداء الوظيفي";

//                sbEmailTemplate.Append("<div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br />");
//                sbEmailTemplate.Append("  عزيزي المستخدم،   <br /><br/>");
//                sbEmailTemplate.Append("((تجربة المنظومة )) الموضوع  :  تقييم الأداء الوظيفي   <br /> <br />");
//                sbEmailTemplate.Append(" نفيدكم بأن  التقييم الخاص بـ    " + evaluation + " قد تمت الموافقه عليه من قبله     <br />  <br />");
//                sbEmailTemplate.Append("  للتصديق عليه يرجى الدخول  على منظومة الاداء الوظيفي  أضغط   :  " + "<a href='http://CareerPerformance:8090/' tiltle='منظومة  الأداء الوظيفي'>" + "هنا </a>" + "   <br /> <br />");
//                sbEmailTemplate.Append("  التاريخ " + today + "  <br /><br />");
//                sbEmailTemplate.Append("  </div>");
//                sbEmailTemplate.Append("<div  style='direction:rtl;font:16px; font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br /><br /><br /><br /><br />");
//                sbEmailTemplate.Append(" ----------------------<br />");
//                sbEmailTemplate.Append(" خدمات تقنية المعلومات<br />");
//                sbEmailTemplate.Append(" منظومة الأداء الوظيفي<br />");
//                sbEmailTemplate.Append("  </div>");
//            }


//            else if (emailType == "ToEvaluationReconsider")
//            {
//                strSubject = " تجربة // منظومة الأداء الوظيفي";

//                sbEmailTemplate.Append("<div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br />");
//                sbEmailTemplate.Append("  عزيزي المستخدم،   <br /><br/>");
//                sbEmailTemplate.Append("((تجربة المنظومة )) الموضوع  :  تقييم الأداء الوظيفي   <br /> <br />");
//                sbEmailTemplate.Append(" نفيدكم بأن  التقييم الخاص بـ    " + evaluation + "  تمت إعادته لكم وذلك لإعادة النظر في محتوى التقييم   <br /> <br />");
//                sbEmailTemplate.Append(" للدخول على المنظومة أضغط  :  " + "<a href='http://CareerPerformance:8090/' tiltle='منظومة  الأداء الوظيفي'>" + "هنا </a>" + "   <br />");
//                sbEmailTemplate.Append("  التاريخ " + today + "  <br /><br />");
//                sbEmailTemplate.Append("  </div>");
//                sbEmailTemplate.Append("<div  style='direction:rtl;font:16px; font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br /><br /><br /><br /><br />");
//                sbEmailTemplate.Append(" ----------------------<br />");
//                sbEmailTemplate.Append(" خدمات تقنية المعلومات<br />");
//                sbEmailTemplate.Append(" منظومة الأداء الوظيفي<br />");
//                sbEmailTemplate.Append("  </div>");
//            }

//            else if (emailType == "ToTransferToCommittee")
//            {
//                strSubject = " تجربة // منظومة الأداء الوظيفي";

//                sbEmailTemplate.Append("<div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br />");
//                sbEmailTemplate.Append("  عزيزي المستخدم،   <br /><br/>");
//                sbEmailTemplate.Append("((تجربة المنظومة )) الموضوع  :  تقييم الأداء الوظيفي   <br /> <br />");
//                sbEmailTemplate.Append(" نفيدكم بأن  التقييم الخاص بـ    " + evaluation + "  تم تحويله الى فرع الضباط - لجنة التظلم  بتاريخ   <br /> <br />");
//                sbEmailTemplate.Append("  التاريخ " + today + "  <br /><br />");
//                sbEmailTemplate.Append("  </div>");
//                sbEmailTemplate.Append("<div  style='direction:rtl;font:16px; font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//                sbEmailTemplate.Append("  <br /><br /><br /><br /><br /><br />");
//                sbEmailTemplate.Append(" ----------------------<br />");
//                sbEmailTemplate.Append(" خدمات تقنية المعلومات<br />");
//                sbEmailTemplate.Append(" منظومة الأداء الوظيفي<br />");
//                sbEmailTemplate.Append("  </div>");
//            }


//            // await SendEmailAsync(emailTo, strSubject, sbEmailTemplate);

//            new Thread(async () => await SendEmailAsync(emailTo, strSubject, sbEmailTemplate)).Start();

//        }




//        public async Task FeedbackSendEmail(string emailTo)
//        {

//            emailTo = (emailTo + "@mod.saf").Trim();
//            var today = DateTime.Now.Day.ToString() + " " + GetMonth() + " " + DateTime.Now.Year.ToString() + "  بتوقيت  " + string.Format("{0:HH:MM}", DateTime.Now);


//            StringBuilder sbEmailTemplate = new StringBuilder();
//            string strSubject = String.Empty;

//            strSubject = " منظومة الأداء الوظيفي";

//            sbEmailTemplate.Append("<div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//            sbEmailTemplate.Append("  <br /><br />");
//            sbEmailTemplate.Append("  عزيزي المستخدم،   <br /><br/>");
//            sbEmailTemplate.Append("الموضوع  : منظومة الأداء الوظيفي   <br /> <br />");          
//            sbEmailTemplate.Append(" لقد تم الرد على ملاحظاتكم في المنظومة <br />");
//            sbEmailTemplate.Append(" للدخول على النظام أضغط  :  " + "<a href='http://CareerPerformance:8090/' tiltle='منظومة  الأداء الوظيفي'>" + "هنا </a>" + "   <br /> <br />");
//            sbEmailTemplate.Append("  التاريخ " + today + "  <br />");
//            sbEmailTemplate.Append("  </div>");
//            sbEmailTemplate.Append("<div  style='direction:rtl;font:16px; font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//            sbEmailTemplate.Append("  <br /><br /><br /><br /><br />");
//            sbEmailTemplate.Append(" ----------------------<br />");
//            sbEmailTemplate.Append(" خدمات تقنية المعلومات<br />");
//            sbEmailTemplate.Append(" منظومة الأداء الوظيفي<br />");
//            sbEmailTemplate.Append("  </div>");


//            new Thread(async () => await SendEmailAsync(emailTo, strSubject, sbEmailTemplate)).Start();

//        }



//        public async Task<bool> SendEmailToAllofficers()
//        {


//            string strSubject = String.Empty;
//            strSubject = "  منظومة الأداء الوظيفي";

//            StringBuilder sbEmailTemplate = new StringBuilder();

//            sbEmailTemplate.Append("<div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:right;' >");
//            sbEmailTemplate.Append(" ");
//            sbEmailTemplate.Append("  المستخدم،   <br /><br/>");


//            sbEmailTemplate.Append("  </div>");




//            var builder = new BodyBuilder();
//            var pathName = "//MAMITSPERFAPP01/OfficerImage$/Booster.jpg";
//            var image = builder.LinkedResources.Add(pathName);
//            image.ContentId = MimeUtils.GenerateMessageId();
//            //builder.HtmlBody = string.Format(@" <div  style='direction:rtl;font:23px;font-family: sans-serif, Tahoma, Geneva;text-align:centre;' > <br /><br />  منظومة الاداء الوظيفي  </div> <img src=""cid:{0}"">", image.ContentId);
//            builder.HtmlBody = string.Format(@" <br /><br/> <img src=""cid:{0}"">", image.ContentId);

//            string.Format(@"<img src=""cid:{0}"">", image.ContentId);

//            builder.TextBody = sbEmailTemplate.ToString();


//            //  var _employeList = await _context.EmployeeEvaluations.Include(x=>x.OrganizationUnit).Where(x => x.OrganizationUnit.ServiceId==7000).Select(x=>x.ServiceNumber).ToListAsync();
//            var list = new List<string> { "D1-6064" };
//            foreach (var item in list)
//            {


//                await SendEmailAsyncNew(item + "@MOD.SAF", strSubject, builder);

//            }

//            //await SendEmailAsyncNew("D1-6064@MOD.SAF", strSubject, builder );


//            return true;



//        }

//        public async Task SendEmailAsyncNew(string email, string subject, BodyBuilder builder)
//        {
//            try
//            {
//                var emailMessag = new MimeMessage();
//                emailMessag.From.Add(new MailboxAddress(ec.FromName, ec.fromAddress));
//                emailMessag.To.Add(new MailboxAddress("", email));
//                emailMessag.Subject = subject;

//                emailMessag.Body = builder.ToMessageBody();

//                using (var client = new SmtpClient())
//                {


//                    await client.ConnectAsync(ec.MailServerAddress, Convert.ToInt32(ec.MailServerPort), SecureSocketOptions.Auto).ConfigureAwait(false);
//                    await client.AuthenticateAsync(new NetworkCredential(ec.UserId, ec.UserPassword));
//                    await client.SendAsync(emailMessag).ConfigureAwait(false);

//                    await client.DisconnectAsync(true).ConfigureAwait(false);
//                }
//            }
//            catch (Exception ex)
//            {

//                Console.WriteLine(ex.Message);

//            }
//        }

//        public async Task SendEmailAsync(string email, string subject, StringBuilder message)
//        {
//            try
//            {
//                var emailMessag = new MimeMessage();
//                emailMessag.From.Add(new MailboxAddress(ec.FromName, ec.fromAddress));
//                emailMessag.To.Add(new MailboxAddress("", email));
//                emailMessag.Subject = subject;
//                emailMessag.Body = new TextPart(TextFormat.Html)
//                {
//                    Text = message.ToString()

//                };
//                using (var client = new SmtpClient())
//                {
//                    // client.LocalDomain = ec.LocalDomain;
//                    await client.ConnectAsync(ec.MailServerAddress, Convert.ToInt32(ec.MailServerPort), SecureSocketOptions.Auto).ConfigureAwait(false);
//                    await client.AuthenticateAsync(new NetworkCredential(ec.UserId, ec.UserPassword));
//                    await client.SendAsync(emailMessag).ConfigureAwait(false);

//                    await client.DisconnectAsync(true).ConfigureAwait(false);
//                }
//            }
//            catch (Exception ex)
//            {

//                Console.WriteLine(ex.Message);

//            }
//        }


//        private string GetMonth()
//        {
//            var monthString = "";
//            int month = DateTime.Now.Month;

//            if (month == 1) monthString = "يناير ";
//            if (month == 2) monthString = "فبراير ";
//            if (month == 3) monthString = "مارس ";
//            if (month == 4) monthString = "إبريل ";
//            if (month == 5) monthString = "مايو ";
//            if (month == 6) monthString = "يونيو ";
//            if (month == 7) monthString = "يوليو ";
//            if (month == 8) monthString = "أغسطس ";
//            if (month == 9) monthString = "سبتبمر ";
//            if (month == 10) monthString = "أكتوبر ";
//            if (month == 11) monthString = "نوفمبر ";
//            if (month == 12) monthString = "ديسمبر ";
//            return monthString;
//        }


//    }
//}

