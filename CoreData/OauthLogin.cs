using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Proglet.Core.Data
{
    public class OauthLogin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [Key]
        public int Id { get; set; }

        public int OauthLoginId { get; set; }
        public string LoginService { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }


    }
}
