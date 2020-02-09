using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Proglet.Core.Data
{

    public class User : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        /*public string Error => throw new NotImplementedException();

        public string this[string columnName] => throw new NotImplementedException();

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        private uint userId;
        public uint UserId {
            get => userId;
            set => SetField(ref userId, value);
        }

        private string username;
        public string Username
        {
            get => username;
            set => SetField(ref username, value);
        }


        private string email;
        public string Email
        {
            get => email;
            set => SetField(ref email, value);
        }*/

        [Key]
        public int UserId { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Username { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Email { get; set; }
        
        public string OrganizationIdentifier { get; set; }

        public UserRoles UserRole { get; set; }

        public DateTime RegistrationDate { get; set; }

        public ICollection<Submission> Submissions { get; set; }
        
        public ICollection<CourseRegistration> CourseRegistrations { get; set; }

        public OauthLogin OauthLogin { get; set; }

        public override string ToString()
        {
            return Username;
        }
    }
}
