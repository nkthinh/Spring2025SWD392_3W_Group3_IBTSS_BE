using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace IBTSS.Service.Services.SMSService
{

    public class SmsService
    {
        private const string AccountSid = "AC4c8412280f811dc09ba2b8cfb84a64f6";
        private const string AuthToken = "861ddb2b2a51fce2f4647a0e2e5e6848";
        private const string FromPhone = "+19712932825";

        public void SendBookingConfirmation(string customerPhone, string customerName, string tripInfo)
        {
            TwilioClient.Init(AccountSid, AuthToken);

            // Ensure the phone number starts with "+84" for Vietnam
            if (!customerPhone.StartsWith("+84"))
            {
                customerPhone = $"+84{customerPhone.TrimStart('0')}";
            }

            var messageBody = $"Chào {customerName}, bạn đã đặt vé thành công cho chuyến đi: {tripInfo}. Cảm ơn bạn!";

            var message = MessageResource.Create(
                body: messageBody,
                from: new PhoneNumber(FromPhone),
                to: new PhoneNumber(customerPhone)
            );

            Console.WriteLine($"Tin nhắn gửi thành công! SID: {message.Sid}");
        }
    }
    }
