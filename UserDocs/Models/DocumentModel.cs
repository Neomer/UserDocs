using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Criterion;
using UserDocs.Core;

namespace UserDocs.Models
{
    public class DocumentModel
    {
        public virtual int ID { get; set; }
        public virtual int AuthorID { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual string Data { get; set; }
        public virtual string Filename { get; set; }
        public virtual string ContentType { get; set; }
        public virtual long Size { get; set; }

        public static IList<DocumentModel> Load(int AuthorID)
        {
            ISession session = Context.Instance.Session;
            return session.CreateCriteria<DocumentModel>()
                        .Add(Restrictions.Eq("AuthorID", AuthorID))
                        .AddOrder(Order.Desc("CreationDate"))
                        .List<DocumentModel>();
        }

        public static string SizeToPrintable(long Size)
        {
            string ret;
            double size = Size;
            int n = 0;
            string [] names = { "байт", "Кб", "Мб", "Гб" };

            while (size > 1024 && n < 4)
            {
                n++;
                size /= 1024;
            }

            return Math.Round(size, 1).ToString() + " " + names[n];
        }
    }
}