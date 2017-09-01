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
    public class LogSetsController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: LogSets
        public async Task<ActionResult> Index()
        {
            return View(await db.LogSet.ToListAsync());
        }

        // GET: LogSets/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogSet logSet = await db.LogSet.FindAsync(id);
            if (logSet == null)
            {
                return HttpNotFound();
            }
            return View(logSet);
        }

        // GET: LogSets/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Time,Client,Operation,TimeNeeded,Event,Window")] LogSet logSet)
        {
            if (ModelState.IsValid)
            {
                db.LogSet.Add(logSet);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(logSet);
        }

        // GET: LogSets/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogSet logSet = await db.LogSet.FindAsync(id);
            if (logSet == null)
            {
                return HttpNotFound();
            }
            return View(logSet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Time,Client,Operation,TimeNeeded,Event,Window")] LogSet logSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(logSet).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(logSet);
        }

        // GET: LogSets/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogSet logSet = await db.LogSet.FindAsync(id);
            if (logSet == null)
            {
                return HttpNotFound();
            }
            return View(logSet);
        }

        // POST: LogSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LogSet logSet = await db.LogSet.FindAsync(id);
            db.LogSet.Remove(logSet);
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
