using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GolfShowCase.Models;
using System.Data.Entity;
using System.Net;

namespace GolfShowCase.Controllers
{
    public class HomeController : Controller
    {

        private GolfVesionsEntities1 db = new GolfVesionsEntities1();

        public ActionResult Index(string SearchString, string YearOfRelease)
        {
            //creating the SelectList for the Year Of Release DropDownList
            List<string> ReleaseYearList = new List<string>();
            var ReleaseQuery = from y in db.GolfVersions
                               orderby y.StartProduction
                               select y.StartProduction;

            ReleaseYearList.AddRange(ReleaseQuery.Distinct());
            ViewBag.YearOfRelease = new SelectList(ReleaseYearList);

            //LINQ query to get all the records from the db
            var GolfModel = from g in db.GolfVersions
                            select g;

            //filtering by year
            if (!String.IsNullOrEmpty(YearOfRelease))
            {
                GolfModel = GolfModel.Where(x => x.StartProduction.Contains(YearOfRelease));
            }

            //searching by Version Name
            if (!String.IsNullOrEmpty(SearchString))
            {
                GolfModel = GolfModel.Where(x => x.VersionName.Contains(SearchString));
            }

            return View(GolfModel);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(GolfVersion Golf )
        {
            if(Golf.Image == null)
            {
                Golf.Image = "http://members.yline.com/~whyline/Gesamt/rally_golf.jpg";
            }

            if (ModelState.IsValid)
            {
                db.GolfVersions.Add(Golf);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Golf);
        }

        public ActionResult Edit(int id)
        {
            GolfVersion Golf = db.GolfVersions.Find(id);
            return View(Golf);
        }

        [HttpPost]
        public ActionResult Edit(GolfVersion Golf)
        {
            if (Golf.Image == null)
            {
                Golf.Image = "http://members.yline.com/~whyline/Gesamt/rally_golf.jpg";
            }

            if (ModelState.IsValid)
            {
                db.Entry(Golf).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Golf);
        }

        public ActionResult Details(int? id)
        {
            //if a null id is passed in, display an HTML error page
            //ad using System.Net to get rid of the error on HttpStatusCode
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get the record from the database using its id
            GolfVersion Golf = db.GolfVersions.Find(id);

            //if an invalid id was passed in and the record doesn't not exist in the database, dispay HTML error page
            if (Golf == null)
            {
                return HttpNotFound();
            }
            //pass the data to the Details views to be displayed
            return View(Golf);
        }

        public ActionResult Delete(int id)
        {
            GolfVersion Golf = db.GolfVersions.Find(id);
            return View(Golf);
        }

        [HttpPost, ActionName("Delete") ]
            public ActionResult DeleteConfirmed(int id)
        {
            GolfVersion Golf = db.GolfVersions.Find(id);
            db.GolfVersions.Remove(Golf);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}