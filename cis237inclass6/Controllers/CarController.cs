using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237inclass6.Models;

namespace cis237inclass6.Controllers
{
    [Authorize]
    public class CarController : Controller
    {
        private CarsAStaffenEntities db = new CarsAStaffenEntities();

        // GET: /Car/
        public ActionResult Index()
        {
            // Set up a variable to hold the cars data
            DbSet<Car> CarsToFilter = db.Cars;

            // Set up some strings to hold the data that might be in the sessions.
            // If ther eis nothing in the session we can still use these variables
            // as a default value.
            string filterMake = "";
            string filterMin = "";
            string filterMax = "";

            // Define a min and a max for the cylinders
            int min = 0;
            int max = 16;

            // Check to see if there is a value in the session and if there is, assign it
            // to the variable that we set up to hold the value. 
            if (Session["make"] != null && !String.IsNullOrWhiteSpace((string)Session["make"]))
            {
                filterMake = (string)Session["make"];
            }

            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                min = Int32.Parse(filterMin);
            }

            if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMax = (string)Session["max"];
                max = Int32.Parse(filterMax);
            }

            // Do the filter on the CarsToFilter dataset. Use the where that we used before
            // when doing EF work, only this time send in more lambda expressions to narrow it
            // down further. Since we set up default values for each of the filter parameters,
            // min, max, and FilterMake, we can count on this always running with no errors.
            IEnumerable<Car> filtered = CarsToFilter.Where(car => car.cylinders >= min &&
                                                                  car.cylinders <= max &&
                                                                  car.make.Contains(filterMake));

            // Convert the database set to a list now that the query work is done on it.
            // The view is expecting a list, so we convert the database set to a list. 
            IEnumerable<Car> finalFiltered = filtered.ToList();

            // Controller to view, viewbag
            // Place the string representation of the values that are in the session into 
            // the viewbag so that they can be retrieved and displayed on the view. 
            ViewBag.filterMake = filterMake;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;


            // Return the view with a filtered seection of the cars.
            return View(finalFiltered);

            // This is what used to be returned before a filter was set up. 
            //return View(db.Cars.ToList());
        }

        // GET: /Car/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // GET: /Car/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Car/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,year,make,model,type,horsepower,cylinders")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Cars.Add(car);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(car);
        }

        // GET: /Car/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: /Car/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,year,make,model,type,horsepower,cylinders")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(car);
        }

        // GET: /Car/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: /Car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Car car = db.Cars.Find(id);
            db.Cars.Remove(car);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // HttpPost is saing can only get there from a form request
        // We need to add the HttpPost so it limits the type of
        // requests it will handle to only POST. We can also specify
        // an action name if we don't want it to use the method name 
        // as the action name. 
        [HttpPost, ActionName("Filter")]
        public ActionResult Filter()
        {
            // Get the form data that we sent out of the Request object.
            // the string that is used as a key to get the data matches the
            // name property of the form control. 
            string make = Request.Form.Get("make");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");

            // Store the form data into the session so that it can be retrieved later
            // on to filter the data. Session lets us go controller to controller
            Session["make"] = make;
            Session["min"] = min;
            Session["max"] = max;

            // Redirect the user to the index page. We will do the work of actually
            // filtering the list in the index method. 
            return RedirectToAction("Index");
        }
    }
}
