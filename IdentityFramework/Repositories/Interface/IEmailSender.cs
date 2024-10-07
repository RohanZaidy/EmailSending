﻿namespace IdentityFramework.Repositories.Interface
{
    public interface IEmailSender
    {
        Task<bool> EmailSendAsync(string email, string subject, string message);
    }
}
