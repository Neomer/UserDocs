using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using NHibernate;
using UserDocs.Models;
using UserDocs.Core;

namespace UserDocs.Controllers
{
    public class DocViewController : Controller
    {
        // GET: DocView
        [Authorize]
        public ActionResult Index()
        {
            var list = new List<DocumentListViewModel>();

            try
            {
                var docs = DocumentModel.Load(Context.Instance.CurrentUser.ID);
                foreach (DocumentModel doc in docs)
                {
                    list.Add(new DocumentListViewModel()
                    {
                        ID = doc.ID,
                        Name = doc.Name,
                        CreationDate = doc.CreationDate,
                        PrintableSize = DocumentModel.SizeToPrintable(doc.Size),
                        Filename = (doc.Filename.Length > 30) ? doc.Filename.Substring(0, 30) + "..." : doc.Filename
                    });
                }
            }
            catch (Exception ex)
            {

            }


            return View(list);
        }

        [Authorize]
        public FileResult DownloadFile(int documentID)
        {
            DocumentModel doc = Context.Instance.Session.Get<DocumentModel>(documentID);
            var path = Path.Combine(Server.MapPath(Context.Instance.UploadDirectory), doc.Data);
            return File(path, doc.ContentType, doc.Filename);
        }

        [Authorize]
        [HttpGet]
        public ActionResult AddDocument()
        {
            return View(new DocumentCreationViewModel());
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddDocument(DocumentCreationViewModel documentViewModel)
        {
            if (string.IsNullOrEmpty(documentViewModel.Name))
            {
                ModelState.AddModelError("Name", "Заполните наименование файла!");
                return View(documentViewModel);
            }

            if (documentViewModel.File == null || documentViewModel.File.ContentLength == 0)
            {
                ModelState.AddModelError("File", "Выберите файл!");
                return View(documentViewModel);
            }
            var fileName = Path.GetRandomFileName();
            var path = Path.Combine(Server.MapPath(Context.Instance.UploadDirectory), fileName);
            try
            {
                if (!Directory.Exists(Server.MapPath(Context.Instance.UploadDirectory)))
                {
                    Directory.CreateDirectory(Server.MapPath(Context.Instance.UploadDirectory));
                }
                documentViewModel.File.SaveAs(path);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("File", "Не удалось сохранить файл!");
                return View(documentViewModel);
            }

            ITransaction tr = Context.Instance.Session.BeginTransaction();
            try
            {
                Context.Instance.Session.CreateSQLQuery("EXEC [dbo].[CreateDocument] @AuthorID=" + Context.Instance.CurrentUser.ID + 
                    ", @Name='" + documentViewModel.Name +
                    "',@CreationDate='" + DateTime.Now + 
                    "',@Data='" + fileName +
                    "',@Filename='" + documentViewModel.File.FileName +
                    "',@ContentType='" + documentViewModel.File.ContentType +
                    "',@Size=" + documentViewModel.File.ContentLength).ExecuteUpdate();
                
                tr.Commit();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                ModelState.AddModelError("File", "Ошибка базы данных! " + ex.Message);
                System.IO.File.Delete(path);
                return View(documentViewModel);
            }

            return RedirectToAction("Index", "DocView");
        }

    }
}