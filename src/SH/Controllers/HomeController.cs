using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using SH.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;

namespace SH
{

    public class HomeController : Controller
    {
        public smartdbContext lc = null;

        public HomeController(smartdbContext abc)
        {

            lc = abc;
        }

        public IActionResult Index()
        {
            return Redirect("/Account/Login");

        }



        [HttpGet]
        public IActionResult Room()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Room(Room c)
        {

            try
            {

                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userId = claim.Value;



                lc.Room.Add(c);

                c.UserId = userId;

                var foo = lc.Room.Where(v => v.Name == c.Name);
                var foocount = foo.Count();
                if (foocount == 0)
                {
                    lc.SaveChanges();
                    ViewBag.Message = "Successfully Saved";
                    return View();
                }

                else
                {
                    ViewBag.Message = "room already entered please select another one";
                }


                return View();
            }
            catch
            {

                return Redirect("/Account/Login");
            }

        }



        [HttpGet]
        public IActionResult Appliances(int roomid)
        {
            ViewBag.id = roomid;
            return View();
        }

        [HttpPost]
        public IActionResult Appliances(Appliances c, int rid, string appliances)
        {

            try
            {

                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userId = claim.Value;



                c.Name = appliances;


                lc.Appliances.Add(c);



                lc.SaveChanges();


                Room b_obj = lc.Room.Where(u => u.Id == rid).SingleOrDefault<Room>();
                c.RoomId = b_obj.Id;


                b_obj.NoOfAppliances++;


                lc.SaveChanges();
                ViewBag.Message = "Successfully Saved";
                return View();
            }
            catch
            {

                return Redirect("/Account/Login");
            }

        }


        public IActionResult ShowAppliances(int roomid)
        {

            IList<Appliances> list = lc.Appliances.Where(u => u.RoomId == roomid).ToList<Appliances>();

            return View(list);
        }



        public IActionResult Chart()
        {

            string a = null;
            string b = null;
            string c = null;
            string d = null;

            int count = 0;
            int counti = 0;
            IList<Transaction> toff = lc.Transaction.Where(u => u.Action == "1" && (u.Day == "Wednesday" || u.Day == "Thursday")).ToList<Transaction>();
            foreach (var item in toff)
            {



                if (count == 0)
                {
                    a = item.Permission;
                    DateTime onDate = Convert.ToDateTime(a);
                    ViewBag.one = onDate;


                }
                if (count == 1)
                {
                    b = item.Permission;
                    DateTime onDate = Convert.ToDateTime(b);
                    ViewBag.two = onDate;

                }
                count++;

            }
            TimeSpan t = ViewBag.two - ViewBag.one;
            ViewBag.wed = t;

            //a = item.Permission;
            //DateTime ofDate = Convert.ToDateTime(a);
            //ViewBag.off = ofDate;

            IList<Transaction> ton = lc.Transaction.Where(u => u.Action == "0" && (u.Day == "Wednesday" || u.Day == "Thursday")).ToList<Transaction>();

            foreach (var items in ton)
            {


                if (counti == 0)
                {
                    c = items.Permission;
                    DateTime onDate = Convert.ToDateTime(c);
                    ViewBag.onec = onDate;


                }
                if (counti == 1)
                {
                    d = items.Permission;
                    DateTime onDate = Convert.ToDateTime(d);
                    ViewBag.twod = onDate;

                }
                counti++;

            }


            TimeSpan t2 = ViewBag.twod - ViewBag.onec;
            ViewBag.thu = t2;







            IList<Transaction> listi = lc.Transaction.ToList<Transaction>();
            for (var i = 0; i < listi.Count; i++)
            {

                if (i == 0)
                {
                    ViewBag.a = listi[i].Day;

                    ViewBag.wed = t;

                }
                if (i == 1 && listi[i].Day != ViewBag.a)
                {

                    ViewBag.b = listi[i].Day;


                }

                if (i == 2 && listi[i].Day != ViewBag.a)
                {

                    ViewBag.c = listi[i].Day;
                    ViewBag.thu = t2;

                }




            }


            return View();



        }
       

        public IActionResult Showrooms()
        {
            IList<Room> list = lc.Room.ToList<Room>();



            return View(list);
        }


       

       
        public IActionResult dashoboard()
        {
            IList<Room> list = lc.Room.ToList<Room>();
            ViewData["roomCount"] = list.Count;

            IList<Appliances> lista = lc.Appliances.ToList<Appliances>();
            ViewData["appCount"] = lista.Count;

            //IList<AspNetUsers> listu = lc.AspNetUsers.ToList<AspNetUsers>();
            //ViewData["userpCount"] = listu.Count;

            return View();
        }


        public IActionResult seegraph()
        {

            IList<Room> listi = lc.Room.ToList<Room>();
            for (var i = 0; i < listi.Count; i++)
            {



                if (i == 0)
                {
                    ViewBag.a = listi[i].Name;
                    ViewBag.aa = listi[i].NoOfAppliances;
                }
                if (i == 1)
                {
                    ViewBag.b = listi[i].Name;
                    ViewBag.bb = listi[i].NoOfAppliances;
                }
                if (i == 2)
                {
                    ViewBag.c = listi[i].Name;
                    ViewBag.cc = listi[i].NoOfAppliances;
                }
                if (i == 3)
                {
                    ViewBag.d = listi[i].Name;
                    ViewBag.dd = listi[i].NoOfAppliances;
                }
                if (i == 4)
                {
                    ViewBag.e = listi[i].Name;
                    ViewBag.ee = listi[i].NoOfAppliances;
                }
                if (i == 5)
                {
                    ViewBag.f = listi[i].Name;
                    ViewBag.ff = listi[i].NoOfAppliances;
                }


            }
            if (ViewBag.a == null)
            {
                ViewBag.a = 0;
                ViewBag.aa = 0;

            }
            if (ViewBag.b == null)
            {
                ViewBag.b = 0;
                ViewBag.bb = 0;
            }
            if (ViewBag.c == null)
            {
                ViewBag.c = 0;
                ViewBag.cc = 0;
            }
            if (ViewBag.d == null)
            {
                ViewBag.d = 0;
                ViewBag.dd = 0;
            }
            if (ViewBag.e == null)
            {
                ViewBag.e = 0;
                ViewBag.ee = 0;
            }
            if (ViewBag.f == null)
            {
                ViewBag.f = 0;
                ViewBag.ff = 0;
            }


            return View();


        }


        public IActionResult AddTransaction(int id)
        {
            //step 1 
            // data came from applainces table on click action 
            Appliances app = new Appliances();
            Transaction t1 = new Transaction();
            t1.Request = System.DateTime.Now;
            t1.AppId = id;
            t1.Day = DateTime.Now.DayOfWeek.ToString();
            lc.Transaction.Add(t1);
            lc.SaveChanges();


            var obj = lc.Appliances.Where(m => m.Id == id).FirstOrDefault();
            ViewBag.AppStatus = obj.Status;
            return View();

        }



        public IActionResult DATAFORARD()
        {
            //step 2 arduino hit this action


            //TRYCATCH
            Transaction obj = lc.Transaction.Where(u => u.Permission == null && u.Inspect == null).FirstOrDefault<Transaction>();
            Appliances appobj = lc.Appliances.Where(u => u.Id == obj.AppId).SingleOrDefault();
            string requestToArduino = "";

            if (appobj.Status == "OFF")
            {
                //update appliance statu on
                //  appobj.Status = "ON";
                // obj.Action = "ON";
                ///lc.Entry(appobj).State = EntityState.Modified;
                //lc.SaveChanges();

                requestToArduino = obj.AppId + "ON";

            }

            else
            {
                //appobj.Status = "OFF";
                //obj.Action = "OFF";
                //lc.Entry(appobj).State = EntityState.Modified;
                //lc.SaveChanges();
                requestToArduino = obj.AppId + "OFF";
            }

            lc.Entry(obj).State = EntityState.Modified;
            lc.SaveChanges();
            return Json(requestToArduino);
        }

        [HttpGet]
        public IActionResult arduinoPermission()
        {
            //step 3 arduino permission de ga enter data in transaction table where permission was null

            Transaction T1 = new Transaction();
            T1 = lc.Transaction.Where(M => M.Action == null && M.Permission == null && M.Inspect == null).FirstOrDefault<Transaction>();
            Appliances appobj = lc.Appliances.Where(u => u.Id == T1.AppId).SingleOrDefault();
            // ACTION 2 MEAN IS KO ARDUINO SE PERMISSION MILL GAYE HA


            if (appobj.Status == "OFF")
            {
                //update appliance statu on
                appobj.Status = "ON";
                T1.Action = "1";
                T1.Permission = System.DateTime.Now.ToString();
                lc.Entry(appobj).State = EntityState.Modified;
                lc.Entry(T1).State = EntityState.Modified;

                lc.SaveChanges();


            }

            else
            {
                appobj.Status = "OFF";
                T1.Action = "0";
                T1.Permission = System.DateTime.Now.ToString();
                lc.Entry(appobj).State = EntityState.Modified;
                lc.Entry(T1).State = EntityState.Modified;

                lc.SaveChanges();
            }


            return View();
        }


        public IActionResult email()
        {




            IList<Transaction> list = lc.Transaction.Where(u => u.Permission == null && u.Inspect == null).ToList<Transaction>();

            //require changes
            if (list.Count >= 2)
            {
                return RedirectToAction("Sendemail");
            }

            else
            {
                ViewBag.msg = "error";

            }





            return View();
        }


        public IActionResult Sendemail(string to, string subject, string body)

        {



          //  IList<AspNetUsers> listi = lc.AspNetUsers.ToList<AspNetUsers>();



         //   foreach (var items in listi)
            {
              //  to = items.Email;
                body = "something issue with your appliances please contact technical team of smart home";
                subject = "Issue in appliance";

                MailMessage omailmessage = new MailMessage();
                omailmessage.From = new MailAddress("smarthome327@gmail.com");
                omailmessage.To.Add(to);
                omailmessage.Body = body;
                omailmessage.Subject = subject;
                omailmessage.IsBodyHtml = true;
                SmtpClient osmtpclient = new SmtpClient("smtp.gmail.com", 587);
                osmtpclient.EnableSsl = true;
                osmtpclient.UseDefaultCredentials = true;
                osmtpclient.Credentials = new NetworkCredential("smarthome327@gmail.com", "Smart_327");
                osmtpclient.Send(omailmessage);


            }

            return RedirectToAction("Sendemailtotechnician");

        }

        public IActionResult Sendemailtotechnician(string to, string subject, string body)
        {

            string a = "shan4924@gmail.com";


            to = "smarthome327@gmail.com";
            body = "something issue with appliances of " + a;
            subject = "Issue in appliance";

            MailMessage omailmessage = new MailMessage();
            omailmessage.From = new MailAddress("smarthome327@gmail.com");
            omailmessage.To.Add(to);
            omailmessage.Body = body;
            omailmessage.Subject = subject;
            omailmessage.IsBodyHtml = true;
            SmtpClient osmtpclient = new SmtpClient("smtp.gmail.com", 587);
            osmtpclient.EnableSsl = true;
            osmtpclient.UseDefaultCredentials = true;
            osmtpclient.Credentials = new NetworkCredential("smarthome327@gmail.com", "Smart_327");
            osmtpclient.Send(omailmessage);

            return View();

        }

        public IActionResult minustest(int id)
        {

            string a = null;
            string on = null;



            IList<Transaction> toff = lc.Transaction.Where(u => u.Action == "1" && u.AppId == id && u.Day == "Sunday").ToList<Transaction>();
            foreach (var item in toff)
            {
                a = item.Permission;
                DateTime ofDate = Convert.ToDateTime(a);
                ViewBag.off = ofDate;
            }
            IList<Transaction> ton = lc.Transaction.Where(u => u.Action == "0" && u.AppId == id && u.Day == "Sunday").ToList<Transaction>();
            foreach (var item in ton)
            {
                on = item.Permission;
                DateTime onDate = Convert.ToDateTime(on);
                ViewBag.on = onDate;
            }


            TimeSpan final = ViewBag.on - ViewBag.off;

            ViewBag.msg = final;





            return View();
        }

        public IActionResult Contact()
        {
            try
            {

                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var userId = claim.Value;
            }
            catch
            {
                return Redirect("/Account/Login");
            }

                return View();
        }



        public IActionResult Checkaurd()
        {
            IList<Appliances> list = lc.Appliances.Where(u => u.Status == "ON" || u.Status == "OFF").ToList<Appliances>();


            return View();
        }


        public IActionResult ardchk(string to, string subject, string body)
        {

            to = "smarthome327@gmail.com";
            body = "something issue with appliances of ";
            subject = "Issue in appliance";

            MailMessage omailmessage = new MailMessage();
            omailmessage.From = new MailAddress("smarthome327@gmail.com");
            omailmessage.To.Add(to);
            omailmessage.Body = body;
            omailmessage.Subject = subject;
            omailmessage.IsBodyHtml = true;
            SmtpClient osmtpclient = new SmtpClient("smtp.gmail.com", 587);
            osmtpclient.EnableSsl = true;
            osmtpclient.UseDefaultCredentials = true;
            osmtpclient.Credentials = new NetworkCredential("smarthome327@gmail.com", "Smart_327");
            osmtpclient.Send(omailmessage);

            return View();


        }


        public IActionResult highchrt()
        {


            return View();
        }
    }
}

















