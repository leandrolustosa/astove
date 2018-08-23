using AInBox.Astove.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astove.BlurAdmin.Model
{
    public class PessoaAddResultModel : BaseResultModel
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
    }

    public class ForgotPasswordResultModel : BaseResultModel
    {
        public int UserId { get; set; }
        public string Token { get; set; }
    }

    public class ResetPasswordResultModel : BaseResultModel
    {
    }

    public class SendVerificationCodeResultModel : BaseResultModel
    {
        public int UserId { get; set; }
    }
}
