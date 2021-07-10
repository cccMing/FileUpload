using System.Collections.Generic;

namespace FileUpload.Models
{
    public class AccountModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
    }
}
