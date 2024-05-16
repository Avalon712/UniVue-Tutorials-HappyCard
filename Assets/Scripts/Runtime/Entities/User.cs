
using System;

namespace HappyCard.Entities
{
    public sealed class User
    {
        public string id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public DateTime lastLoginDate { get; set; }

        public DateTime registerDate { get; set; }

        public string password { get; set; }
    }
}
