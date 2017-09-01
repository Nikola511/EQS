using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using EQS.Models;
using EntityFramework.Extensions;

namespace EQS.Controllers
{
    public class HomeController : Controller
    {
        private Database1Entities db = new Database1Entities();
        private static int currentTime;
        private int endOfTime = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("EndOfTime"));

        public ActionResult Index() { return View(); }

        [HttpPost]
        public ActionResult SystemPreparation()
        {
            currentTime = 0;

            db.WindowSet.Where(a => a.CurrentClient != "-")
                .Update(a => new WindowSet { CurrentClient = "-", RestOfTime = 0 });
            db.Customer.Delete();
            db.QueueSet.Delete();
            db.LogSet.Delete();

            return null;
        }

        [HttpGet]
        public JsonResult GetWindowState()
        {
            ++currentTime;
            DecreaseRestOfTime();
            var jsonData = db.WindowSet;         
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        private void DecreaseRestOfTime()
        {
            LogSet log = new LogSet { Time = currentTime };
            IEnumerable<WindowSet> windows = db.WindowSet.Where(c => c.RestOfTime > 0)
                .AsEnumerable().Select(c => {
                    --c.RestOfTime;
                    if (c.RestOfTime == 0 && c.CurrentClient != "-")
                    {
                        int index = c.CurrentClient.IndexOf("(");
                        log.Client = c.CurrentClient.Substring(0, index);
                        log.Operation = c.CurrentClient.Substring(index + 1, 2);
                        log.TimeNeeded = Convert.ToInt32(c.CurrentClient.Substring(index + 4, 1));
                        log.Event = "Ушёл обслуженный";
                        db.LogSet.Add(log);

                        c.CurrentClient = "-";
                    }
                    return c;
                });
            foreach (WindowSet window in windows)
                db.Entry(window).State = EntityState.Modified;
            db.SaveChanges();

            QueueTimeOut();            
        }

        private void QueueTimeOut()
        {
            LogSet log = new LogSet();
            IEnumerable<QueueSet> queueSets = db.QueueSet.Where(
                c => c.TimeNeeded + currentTime > endOfTime)
                .AsEnumerable().Select(c => {
                    log.Time = currentTime;
                    log.Client = c.Client;
                    log.Operation = c.Operation;
                    log.TimeNeeded = c.TimeNeeded;
                    log.Event = "Отказано в обслуживании";
                    db.LogSet.Add(log);
                    return c;
                });
            foreach (QueueSet queueSet in queueSets)
                db.QueueSet.Remove(queueSet);
            db.SaveChanges();

            FillWindows();
        }

        [HttpPost]
        public ActionResult Queuing(int queueTime, string customer, string operation, int timeNeeded)
        {
            db.Customer.Add(new Customer
            {
                Name = customer,
                Operation = operation,
                QueueTime = queueTime,
                TimeNeeded = timeNeeded
            });
            db.QueueSet.Add(new QueueSet
            {
                QueueTime = queueTime,
                Client = customer,
                Operation = operation,
                TimeNeeded = timeNeeded
            });
            db.LogSet.Add(new LogSet
            {
                Time = queueTime,
                Client = customer,
                Operation = operation,
                TimeNeeded = timeNeeded,
                Event = "Пришёл клиент"
            });
            db.SaveChanges();
            return null;
        }

        private void FillWindows()
        {
            LogSet log = new LogSet { Time = currentTime, Event = "Вызван в окно" };

            WindowSet window = db.WindowSet.Where(a => a.Name == "A" && a.CurrentClient == "-").FirstOrDefault();
            if (window != null)
            {
                string unavailableOperation = window.UnavailableOperation;
                QueueSet queueSet = db.QueueSet.Where(b => b.Operation != unavailableOperation).FirstOrDefault();
                if (queueSet != null)
                {
                    log.Client = queueSet.Client;
                    log.Operation = queueSet.Operation;
                    log.TimeNeeded = queueSet.TimeNeeded;
                    log.Window = window.Name;
                    window.CurrentClient = queueSet.Client + "(" + queueSet.Operation + "-" + queueSet.TimeNeeded + ")";
                    window.RestOfTime = queueSet.TimeNeeded;
                    db.Entry(window).State = EntityState.Modified;
                    db.QueueSet.Remove(queueSet);
                    db.LogSet.Add(log);
                    db.SaveChanges();
                }
            }

            window = db.WindowSet.Where(a => a.Name == "B" && a.CurrentClient == "-").FirstOrDefault();
            if (window != null)
            {
                string unavailableOperation = window.UnavailableOperation;
                QueueSet queueSet = db.QueueSet.Where(b => b.Operation != unavailableOperation).FirstOrDefault();
                if (queueSet != null)
                {
                    log.Client = queueSet.Client;
                    log.Operation = queueSet.Operation;
                    log.TimeNeeded = queueSet.TimeNeeded;
                    log.Window = window.Name;
                    window.CurrentClient = queueSet.Client + "(" + queueSet.Operation + "-" + queueSet.TimeNeeded + ")";
                    window.RestOfTime = queueSet.TimeNeeded;
                    db.Entry(window).State = EntityState.Modified;
                    db.QueueSet.Remove(queueSet);
                    db.LogSet.Add(log);
                    db.SaveChanges();
                }
            }

            window = db.WindowSet.Where(a => a.Name == "C" && a.CurrentClient == "-").FirstOrDefault();
            if (window != null)
            {
                string unavailableOperation = window.UnavailableOperation;
                QueueSet queueSet = db.QueueSet.Where(b => b.Operation != unavailableOperation).FirstOrDefault();
                if (queueSet != null)
                {
                    log.Client = queueSet.Client;
                    log.Operation = queueSet.Operation;
                    log.TimeNeeded = queueSet.TimeNeeded;
                    log.Window = window.Name;
                    window.CurrentClient = queueSet.Client + "(" + queueSet.Operation + "-" + queueSet.TimeNeeded + ")";
                    window.RestOfTime = queueSet.TimeNeeded;
                    db.Entry(window).State = EntityState.Modified;
                    db.QueueSet.Remove(queueSet);
                    db.LogSet.Add(log);
                    db.SaveChanges();
                }
            }
        }

        public ActionResult Statistics()
        {
            IEnumerable<LogSet> logs;
            ViewBag.nIncomingCustomers = db.LogSet.Where(a => a.Event == "Пришёл клиент").Count();
            ViewBag.nDeniedService = db.LogSet.Where(a => a.Event == "Отказано в обслуживании").Count();
            ViewBag.sumOperationsO1 = db.LogSet.Where(a => a.Operation == "O1" && a.Window != null).Count();
            ViewBag.sumOperationsO2 = db.LogSet.Where(a => a.Operation == "O2" && a.Window != null).Count();
            ViewBag.sumOperationsO3 = db.LogSet.Where(a => a.Operation == "O3" && a.Window != null).Count();

            double nSumDiff = 0, numberOperations = 0;
            logs = db.LogSet.Where(a => a.Event == "Пришёл клиент").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
            {
                nSumDiff += log.TimeNeeded;
                ++numberOperations;
            }
            ViewBag.nAverageDiff = Math.Round(nSumDiff / numberOperations, 2);

            int sumDifficultyO1 = 0;
            logs = db.LogSet.Where(a => a.Operation == "O1" && a.Window != null)
                .AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO1 += log.TimeNeeded;
            ViewBag.sumDifficultyO1 = sumDifficultyO1;

            int sumDifficultyO2 = 0;
            logs = db.LogSet.Where(a => a.Operation == "O2" && a.Window != null)
                .AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO2 += log.TimeNeeded;
            ViewBag.sumDifficultyO2 = sumDifficultyO2;

            int sumDifficultyO3 = 0;
            logs = db.LogSet.Where(a => a.Operation == "O3" && a.Window != null)
                .AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO3 += log.TimeNeeded;
            ViewBag.sumDifficultyO3 = sumDifficultyO3;

            //Окно A
            ViewBag.nServedCustomersA = db.LogSet.Where(a => a.Event == "Вызван в окно" && a.Window == "A").Count();
            ViewBag.sumOperationsO1A = db.LogSet.Where(a => a.Operation == "O1" && a.Window == "A").Count();
            ViewBag.sumOperationsO2A = db.LogSet.Where(a => a.Operation == "O2" && a.Window == "A").Count();

            double nSumDiffA = 0, numberOperationsA = 0;
            logs = db.LogSet.Where(a => a.Event == "Вызван в окно" && a.Window == "A").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
            {
                nSumDiffA += log.TimeNeeded;
                ++numberOperationsA;
            }
            ViewBag.nAverageDiffA = Math.Round(nSumDiffA / numberOperationsA, 2);

            int sumDifficultyO1A = 0;
            logs = db.LogSet.Where(a => a.Operation == "O1" && a.Window == "A").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO1A += log.TimeNeeded;
            ViewBag.sumDifficultyO1A = sumDifficultyO1A;

            int sumDifficultyO2A = 0;
            logs = db.LogSet.Where(a => a.Operation == "O2" && a.Window == "A").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO2A += log.TimeNeeded;
            ViewBag.sumDifficultyO2A = sumDifficultyO2A;

            //Окно B
            ViewBag.nServedCustomersB = db.LogSet.Where(a => a.Event == "Вызван в окно" && a.Window == "B").Count();
            ViewBag.sumOperationsO2B = db.LogSet.Where(a => a.Operation == "O2" && a.Window == "B").Count();
            ViewBag.sumOperationsO3B = db.LogSet.Where(a => a.Operation == "O3" && a.Window == "B").Count();

            double nSumDiffB = 0, numberOperationsB = 0;
            logs = db.LogSet.Where(a => a.Event == "Вызван в окно" && a.Window == "B").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
            {
                nSumDiffB += log.TimeNeeded;
                ++numberOperationsB;
            }
            ViewBag.nAverageDiffB = Math.Round(nSumDiffB / numberOperationsB, 2);

            int sumDifficultyO2B = 0;
            logs = db.LogSet.Where(a => a.Operation == "O2" && a.Window == "B").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO2B += log.TimeNeeded;
            ViewBag.sumDifficultyO2B = sumDifficultyO2B;

            int sumDifficultyO3B = 0;
            logs = db.LogSet.Where(a => a.Operation == "O3" && a.Window == "B").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO3B += log.TimeNeeded;
            ViewBag.sumDifficultyO3B = sumDifficultyO3B;

            //Окно C
            ViewBag.nServedCustomersC = db.LogSet.Where(a => a.Event == "Вызван в окно" && a.Window == "C").Count();
            ViewBag.sumOperationsO1C = db.LogSet.Where(a => a.Operation == "O1" && a.Window == "C").Count();
            ViewBag.sumOperationsO3C = db.LogSet.Where(a => a.Operation == "O3" && a.Window == "C").Count();

            double nSumDiffC = 0, numberOperationsC = 0;
            logs = db.LogSet.Where(a => a.Event == "Вызван в окно" && a.Window == "C").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
            {
                nSumDiffC += log.TimeNeeded;
                ++numberOperationsC;
            }
            ViewBag.nAverageDiffC = Math.Round(nSumDiffC / numberOperationsC, 2);

            int sumDifficultyO1C = 0;
            logs = db.LogSet.Where(a => a.Operation == "O1" && a.Window == "C").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO1C += log.TimeNeeded;
            ViewBag.sumDifficultyO1C = sumDifficultyO1C;

            int sumDifficultyO3C = 0;
            logs = db.LogSet.Where(a => a.Operation == "O3" && a.Window == "C").AsEnumerable().Select(a => a);
            foreach (LogSet log in logs)
                sumDifficultyO3C += log.TimeNeeded;
            ViewBag.sumDifficultyO3C = sumDifficultyO3C;

            return View();
        }
    }
}