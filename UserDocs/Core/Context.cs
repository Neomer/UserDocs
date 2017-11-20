using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Principal;
using NHibernate;
using NHibernate.Cfg;
using UserDocs.Models;

namespace UserDocs.Core
{
    public sealed class Context
    {
        #region Singleton implementation
        private static volatile Context instance;
        private static object syncRoot = new Object();

        private Context()
        {
            _sessionFactory = new Configuration().Configure().BuildSessionFactory();
            _session = _sessionFactory.OpenSession();
        }

        public static Context Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Context();
                    }
                }

                return instance;
            }
        }

        #endregion  

        private ISessionFactory _sessionFactory;
        private ISession _session;
        public ISession Session
        {
            get
            {
                return _session;
            }
        }

        private UserModel _currentUser;
        public UserModel CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    IIdentity id = HttpContext.Current.User.Identity;
                    if (id.IsAuthenticated)
                    {
                        _currentUser = UserModel.Load(id.Name);
                    }
                }

                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        }

        public string UploadDirectory
        {
            get
            {
                return "~/Upload";
            }
        }

        
    }
}