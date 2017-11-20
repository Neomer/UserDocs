using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Criterion;
using NHibernate;
using UserDocs.Core;

namespace UserDocs.Models
{
    public class UserModel
    {
        public virtual int ID { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }

        public static UserModel Load(string username)
        {
            return Context.Instance.Session.CreateCriteria<UserModel>()
                .Add(Restrictions.Eq("Username", username))
                .UniqueResult<UserModel>();
        }

        public static UserModel Load(string username, string password)
        {
            return Context.Instance.Session.CreateCriteria<UserModel>()
                .Add(Restrictions.Eq("Username", username))
                .Add(Restrictions.Eq("Password", password))
                .UniqueResult<UserModel>();
        }
    }
}