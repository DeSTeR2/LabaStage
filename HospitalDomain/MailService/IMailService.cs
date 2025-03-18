using HospitalDomain.Utils;

namespace HospitalDomain.MailService
{
    public interface IMailService {
        void SentMail(Mail mail);
    }
}
