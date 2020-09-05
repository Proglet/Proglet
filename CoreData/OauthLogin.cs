using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Proglet.Core.Data
{
    public class OauthLogin
    {

        [Key]
        public int Id { get; set; }

        public int OauthLoginId { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string LoginService { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }


    }
}
