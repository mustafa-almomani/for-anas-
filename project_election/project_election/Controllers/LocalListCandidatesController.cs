using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project_election.Models;

namespace project_election.Controllers
{
    public class LocalListCandidatesController : Controller
    {
        private electionEntities5 db = new electionEntities5();

        // GET: LocalListCandidates
        public ActionResult Index()
        {
            return View(db.LocalListCandidates.ToList());
        }

        // GET: LocalListCandidates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalListCandidate localListCandidate = db.LocalListCandidates.Find(id);
            if (localListCandidate == null)
            {
                return HttpNotFound();
            }
            return View(localListCandidate);
        }

        // GET: LocalListCandidates/Create
        public ActionResult Create(string listName)
        {
            ViewBag.ListName = listName;
            return View(new List<LocalListCandidate>());
        }

        // POST: LocalListCandidates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<LocalListCandidate> candidates, string ListName, string ElectionArea, string Governorate ,string Image)
        {
            // إنشاء وحفظ قائمة جديدة
            LocalList localList = new LocalList
            {
                
                ListName = ListName,
                ElectionArea = ElectionArea,
                Governorate = Governorate,
                NumberOfVotes = 0,
                image= Image
                
            };
            db.LocalLists.Add(localList);
            db.SaveChanges();

            // حفظ بيانات المرشحين وربطهم بالقائمة
            foreach (var candidate in candidates)
            {
                candidate.listname = localList.ListName; // الربط باستخدام معرف القائمة المحفوظة

                db.LocalListCandidates.Add(candidate);
            }
            db.SaveChanges();

            // إعادة توجيه المستخدم إلى صفحة النجاح أو صفحة أخرى
            return RedirectToAction("index");
        }


        // POST: LocalListCandidates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CandidateID,CandidateName,NationalNumber,Governorate,ElectionArea,NumberOfVotesCandidate,LocalListingID,typeofCandidates")] LocalListCandidate localListCandidate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(localListCandidate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(localListCandidate);
        }

        // GET: LocalListCandidates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalListCandidate localListCandidate = db.LocalListCandidates.Find(id);
            if (localListCandidate == null)
            {
                return HttpNotFound();
            }
            return View(localListCandidate);
        }

        // POST: LocalListCandidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocalListCandidate localListCandidate = db.LocalListCandidates.Find(id);
            db.LocalListCandidates.Remove(localListCandidate);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetUserData(string nationalNumber)
        {
            var user = db.Users.FirstOrDefault(u => u.NationalNumber == nationalNumber);
            if (user != null)
            {
                return Json(new
                {
                    FullName = user.FullName,
                    ElectionArea = user.ElectionArea,
                    Governorate = user.Governorate,
                    Gender = user.Gender,
                   
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
