using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SMSManager
    {
        private string _token { get; set; }
        public SMSManager(string userApiKey, string secretKey)
        {
            SmsIrRestful.Token tk = new SmsIrRestful.Token();
            string result = tk.GetToken(userApiKey, secretKey);
            _token = result;
        }

        public async Task<string> GetToken(string userApiKey, string secretKey)
        {
            SmsIrRestful.Token tk = new SmsIrRestful.Token();
            string result = tk.GetToken(userApiKey, secretKey);
            return  result;
        }
        public async Task<bool>  VerificationCodeByThemplate(SmsIrRestful.UltraFastSend dto)
        {

            var tk = new SmsIrRestful.UltraFast();
            var res = tk.Send(_token, dto);
            return res.IsSuccessful;
        }
        public async Task<bool> VerificationCode( SmsIrRestful.RestVerificationCode dto)
        {
            var tk = new SmsIrRestful.VerificationCode();
            var res = tk.Send(_token, dto);
            return res.IsSuccessful;
        }
    }
}
