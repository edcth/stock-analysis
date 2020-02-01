using System;
using System.Threading;
using System.Threading.Tasks;
using core.Adapters.Emails;

namespace core.Account
{
    public class PasswordRequestHandler : MediatR.INotificationHandler<UserPasswordResetRequested>
    {
        private IAccountStorage _storage;
        private IEmailService _email;

        public PasswordRequestHandler(IAccountStorage storage, IEmailService email)
        {
            _storage = storage;
            _email = email;
        }

        public async Task Handle(UserPasswordResetRequested e, CancellationToken cancellationToken)
        {
            // generate random guid that maps back to aggregate id
            // store it in the storage
            // send email

            Console.WriteLine("Issuing password reset");

            var u = await _storage.GetUser(e.AggregateId.ToString());
            if (u == null)
            {
                return;
            }

            var request = new PasswordResetRequest(e.AggregateId, e.When);

            await _storage.SavePasswordResetRequest(request);

            var reseturl = $"{EmailSettings.PasswordResetUrl}/{request.Id}";

            await _email.Send(
                u.State.Email,
                EmailSettings.TemplatePasswordReset,
                new {reseturl}
            );
        }
    }
}