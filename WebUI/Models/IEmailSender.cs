namespace WebUI.Models
{
    //bu sayfa zaten genellikle standart bir sayfa dolayısıyla algoritmik şeylere odaklan, her satırı bilmek zorunda değilsin.
    public interface IEmailSender
    {
        //email mail göndereceğimiz mail adresini temsil ediyor.
        Task SendEmailAsync(string SendToEmailAddress, string subject, string message);

    }
}
