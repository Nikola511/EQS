using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EQS.Models;

namespace EQS.Controllers
{
    public class WindowSetsController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: WindowSets
        public async Task<ActionResult> Index()
        {
            return View(await db.WindowSet.ToListAsync());
        }

        // GET: WindowSets/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WindowSet windowSet = await db.WindowSet.FindAsync(id);
            if (windowSet == null)
            {
                return HttpNotFound();
            }
            return View(windowSet);
        }

        // GET: WindowSets/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,UnavailableOperation,CurrentClient,RestOfTime")] WindowSet windowSet)
        {
            if (ModelState.IsValid)
            {
                db.WindowSet.Add(windowSet);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(windowSet);
        }

        // GET: WindowSets/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            WindowSet windowSet = await db.WindowSet.FindAsync(id);
            if (windowSet == null) { return HttpNotFound(); }
            return View(windowSet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,UnavailableOperation,CurrentClient,RestOfTime")] WindowSet windowSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(windowSet).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(windowSet);
        }

        // GET: WindowSets/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WindowSet windowSet = await db.WindowSet.FindAsync(id);
            if (windowSet == null)
            {
                return HttpNotFound();
            }
            return View(windowSet);
        }

        // POST: WindowSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            WindowSet windowSet = await db.WindowSet.FindAsync(id);
            db.WindowSet.Remove(windowSet);
            await db.SaveChangesAsync();
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
    }
}
