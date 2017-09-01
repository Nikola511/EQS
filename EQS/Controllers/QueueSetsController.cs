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
    public class QueueSetsController : Controller
    {
        private Database1Entities db = new Database1Entities();

        // GET: QueueSets
        public async Task<ActionResult> Index()
        {
            return View(await db.QueueSet.ToListAsync());
        }

        // GET: QueueSets/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QueueSet queueSet = await db.QueueSet.FindAsync(id);
            if (queueSet == null)
            {
                return HttpNotFound();
            }
            return View(queueSet);
        }

        // GET: QueueSets/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,QueueTime,Client,Operation,TimeNeeded")] QueueSet queueSet)
        {
            if (ModelState.IsValid)
            {
                db.QueueSet.Add(queueSet);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(queueSet);
        }

        // GET: QueueSets/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            QueueSet queueSet = await db.QueueSet.FindAsync(id);
            if (queueSet == null) { return HttpNotFound(); }
            return View(queueSet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,QueueTime,Client,TimeNeeded")] QueueSet queueSet)
        {
            if (ModelState.IsValid) 
            {
                db.Entry(queueSet).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(queueSet);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            QueueSet queueSet = await db.QueueSet.FindAsync(id);
            if (queueSet == null) { return HttpNotFound(); }
            return View(queueSet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            QueueSet queueSet = await db.QueueSet.FindAsync(id);
            db.QueueSet.Remove(queueSet);
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
