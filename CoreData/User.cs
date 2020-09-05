using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Proglet.Core.Data
{

    public class User
    {

        [Key]
        public int UserId { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Username { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string FullName { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Email { get; set; }
        
        [Column(TypeName = "varchar(64)")]
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
