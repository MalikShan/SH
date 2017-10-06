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
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace SH
{
   
    public class HomeController : Controller
    {
        public smarthomeContext lc = null;

        public HomeController(smarthomeContext abc)
        {

            lc = abc;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");

        }


        #region register and login logout

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(Residents u, string[] box )
        {
            if (ModelState.IsValid)
            {
                if (u.Usertype == "Other")
                {

                    u.Permission = String.Join(",", box);
                    if (string.IsNullOrEmpty(u.Permission))
                    {
                        lc.Residents.Remove(u);

                        ModelState.Clear();
                        ModelState.AddModelError("", "you need to select permission for Other user");

                    }
                    else
                    {
                        u.Permission = String.Join(",", box);

                        string email = u.Email;
                        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                        Match match = regex.Match(email);
                        if (match.Success)
                        {

                            u.Email = match.ToString();


                            string strmsg = string.Empty;
                            byte[] encode = new byte[u.Password.Length];
                            encode = Encoding.UTF8.GetBytes(u.Password);
                            strmsg = Convert.ToBase64String(encode);
                            u.Password = strmsg;



                            lc.Residents.Add(u);

                            var foo = lc.Residents.Where(v => v.Username == u.Username).ToList();
                            var foocount = foo.Count();
                            if (foocount == 0)
                            {
                                lc.SaveChanges();
                                ModelState.Clear();
                                ViewBag.Message = "User is successfully registered.";
                            }

                            else
                            {
                                ModelState.Clear();
                                ViewBag.Message = "user already exists please select another one";
                            }


                        }
                        else
                        {
                            ModelState.AddModelError("", "incorrect email format correct format is abc@domain.com");
                        }
                    }
                }
                




            

            else
            {


                u.Permission = String.Join(",", box);

                string email = u.Email;
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email);
                if (match.Success)
                {

                    u.Email = match.ToString();


                    string strmsg = string.Empty;
                    byte[] encode = new byte[u.Password.Length];
                    encode = Encoding.UTF8.GetBytes(u.Password);
                    strmsg = Convert.ToBase64String(encode);
                    u.Password = strmsg;



                    lc.Residents.Add(u);

                    var foo = lc.Residents.Where(v => v.Username == u.Username).ToList();
                    var foocount = foo.Count();
                    if (foocount == 0)
                    {
                        lc.SaveChanges();
                        ModelState.Clear();
                        ViewBag.Message = "User is successfully registered.";
                    }

                    else
                    {
                        ModelState.Clear();
                        ViewBag.Message = "user already exists please select another one";
                    }


                }

                else
                {
                    ModelState.AddModelError("", "incorrect email format correct format is abc@domain.com");
                }

            }
                
            }
            //var addr = new System.Net.Mail.MailAddress(u.Email);
            //string aa = addr.Address;


            //string decryptpwd = string.Empty;
            //UTF8Encoding encodepwd = new UTF8Encoding();
            //Decoder Decode = encodepwd.GetDecoder();
            //byte[] todecode_byte = Convert.FromBase64String(strmsg);
            //int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            //char[] decoded_char = new char[charCount];
            //Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            //decryptpwd = new String(decoded_char);

            //TempData["dcr"] = decryptpwd;

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if(HttpContext.Session.GetString("Id") == null)
            {
                return View();
            }
            else
            {
                if (HttpContext.Session.GetString("Usertype") == "Admin")
                { 
                    return View("dashoboard");
                }

                else
                {
                    return RedirectToAction("othershowroom", new { arg = HttpContext.Session.GetString("Id") });
                }

            }

           
        }

        [HttpPost]
        public IActionResult Login(Residents user)
        {

            string strmsg = string.Empty;
            byte[] encode = new byte[user.Password.Length];
            encode = Encoding.UTF8.GetBytes(user.Password);
            strmsg = Convert.ToBase64String(encode);


            var account = lc.Residents.Where(u => u.Username == user.Username && u.Password == strmsg).FirstOrDefault();
            if (account != null)
            {

                HttpContext.Session.SetString("Id", account.Id.ToString());
                HttpContext.Session.SetString("Username", account.Username);
                HttpContext.Session.SetString("Usertype", account.Usertype);
                if (account.Usertype == "Admin")
                {
                   
                    return RedirectToAction("dashoboard");
                }
                else
                {
                    return RedirectToAction("othershowroom", new { arg = account.Id });
                }
            }
            else
            {
                ModelState.AddModelError("", "Username or password is incorrect");
            }
            return View();
        }


        
        public IActionResult logout()
        {
            if(HttpContext.Session.GetString("Id") != null)
            {

                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }



        #endregion


        #region all users edit and delete
        public IActionResult allusers()
        {
            if (HttpContext.Session.GetString("Id") != null)
            {

                IList<Residents> list = lc.Residents.Where(u => u.Usertype == "Other").ToList<Residents>();
                return View(list);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public IActionResult Edituser(int Id)
        {
            if (HttpContext.Session.GetString("Id") != null)
            {
                Residents obj = lc.Residents.Where(c => c.Id == Id).SingleOrDefault();
                return View(obj);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult Edituser(Residents r, string[] box)
        {

            if (ModelState.IsValid)
            {
                r.Permission = String.Join(",", box);
                string strmsg = string.Empty;
                byte[] encode = new byte[r.Password.Length];
                encode = Encoding.UTF8.GetBytes(r.Password);
                strmsg = Convert.ToBase64String(encode);
                r.Password = strmsg;
                lc.Entry(r).State = EntityState.Modified;
                lc.SaveChanges();
                ModelState.Clear();
                ViewBag.Message = "User is successfully edited.";
            }
            return View(r);
        }


        public IActionResult Deleteuser(int Id)
        {
            if (HttpContext.Session.GetString("Id") != null)
            {
                Residents obj = lc.Residents.Where(s => s.Id == Id).SingleOrDefault();
                lc.Residents.Remove(obj);
                lc.SaveChanges();
                return RedirectToAction("allusers");
            }
            else
            {
                RedirectToAction("Login");
            }
            return View();
        }
        #endregion




        #region add room

        [HttpGet]
        public IActionResult Room()
        {
            if (HttpContext.Session.GetString("Id") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public IActionResult Room(Room c)
        {

            var id = HttpContext.Session.GetString("Id");
            var account = lc.Residents.Where(u => u.Id.ToString() == id).SingleOrDefault();

            if (account.Usertype == "Admin")
                {


                    lc.Room.Add(c);
                    c.UserId = account.Id.ToString();


                    var foo = lc.Room.Where(v => v.Name == c.Name);
                    var foocount = foo.Count();
                    if (foocount == 0)
                    {
                        lc.SaveChanges();
                        ViewBag.Message = "Successfully Saved";
                    ModelState.Clear();
                        return View();
                    }

                    else
                    {
                        ViewBag.Message = "room already entered please select another one";
                    }
                }

             
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        #endregion

        #region showrooms one for admin and second for other users

        public IActionResult Showrooms()
        {
          
            if (HttpContext.Session.GetString("Id") != null)
            {
                ViewBag.msg2 = TempData["message"];
                ViewBag.msg = TempData["messagesuc"];
                ViewBag.msg3 = TempData["del"];
                var id = HttpContext.Session.GetString("Id");
                var account = lc.Residents.Where(u=>u.Id.ToString()==id).SingleOrDefault();
                if (account.Usertype == "Admin")
                {


                    IList<Room> list = lc.Room.ToList<Room>();

                    return View(list);

                }
            }
            else
            {
                return RedirectToAction("Login");
            }
            
            return View();
        }


        public IActionResult othershowroom(int arg)
        {
            if (HttpContext.Session.GetString("Id") != null)
            {

                IList<Room> filtered = new List<Room>();


                Residents user = lc.Residents.Where(u => u.Id == arg).SingleOrDefault();

                if (user.Id == arg)
                {

                    IList<Room> rooms = lc.Room.ToList<Room>();

                    foreach (var items in rooms)
                    {
                        if (user.Permission.Contains(items.Name))
                        {
                            filtered.Add(items);

                        }


                    }
                }
                return View(filtered);
            }
            else
            {
                return RedirectToAction("Login");
            }

            
        }

        #endregion



        #region add and show and delete appliances

        [HttpGet]
        public IActionResult Appliances(int roomid)
        {
            if (HttpContext.Session.GetString("Id") != null)
            {
                ViewBag.id = roomid;
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public IActionResult Appliances(Appliances c, int rid, string appliances)
        {


            string id =   HttpContext.Session.GetString("Id");
           
            var account = lc.Residents.Where(u=>u.Id.ToString()==id).SingleOrDefault();

             

            if (account.Usertype == "Admin")
            {

                c.Name = appliances;

                var foo = lc.Appliances.Where(v => v.Name == c.Name && v.RoomId == rid);
                var foocount = foo.Count();
                if (foocount == 0)
                {



                    lc.Appliances.Add(c);

                    lc.SaveChanges();

                    Room b_obj = lc.Room.Where(u => u.Id == rid).SingleOrDefault<Room>();
                    c.RoomId = b_obj.Id;


                    b_obj.NoOfAppliances++;


                    lc.SaveChanges();
                    
                    ModelState.Clear();
                    TempData["messagesuc"] = "Successfully Saved";
                    return RedirectToAction("Showrooms");

                }

                else
                {

                    TempData["message"] = "Appliance already entered please select another one";

                    // System.Threading.Thread.Sleep(5000);
                    // System.Threading.Tasks.Task.Delay(3000).Wait();
                    return RedirectToAction("Showrooms");
                }   

                
              
            }
            return View();

        }


        public IActionResult ShowAppliances(int roomid)
        {
            if (HttpContext.Session.GetString("Id") != null)
            {
                ViewBag.user = HttpContext.Session.GetString("Usertype");
                IList<Appliances> list = lc.Appliances.Where(u => u.RoomId == roomid).ToList<Appliances>();
 
                return View(list);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        public IActionResult Deleteapp(int id)
        {
            if (HttpContext.Session.GetString("Id") != null)
            {
                Appliances obj = lc.Appliances.Where(s => s.Id == id).SingleOrDefault();
                lc.Appliances.Remove(obj);
                lc.SaveChanges();
                Room objr = lc.Room.Where(s => s.Id == obj.RoomId).SingleOrDefault();
                objr.NoOfAppliances--;
                lc.SaveChanges();
                TempData["del"] = "Successfully deleted";
                return RedirectToAction("Showrooms");
            }
            else
            {
                RedirectToAction("Login");
            }
            return View();
           
        }





        #endregion



        #region Arduino REquests
        // old region
        //public IActionResult AddTransaction(int id)
        //{
        //    // When user hit on app icon,transaction table me request create hoge 
        //    // chk current status of app from appliances table and prepare responce againts according to current tatus(Opposite)

        //    Transaction t1 = new Transaction();
        //    t1.Request = System.DateTime.Now;
        //    t1.AppId = id;
        //    t1.Day = DateTime.Now.DayOfWeek.ToString();

        //    Appliances app = new Appliances();
        //    app = lc.Appliances.Where(m => m.Id == id).SingleOrDefault();
        //    ViewBag.AppStatus = app.Status;
        //    string Action = "";
        //    if (app.Status != "ON")
        //    {
        //        Action = "1";
        //        t1.Action = "ON";
        //    }
        //    else
        //    {
        //        t1.Action = "OFF";
        //        Action = "0";
        //    }
        //    lc.Transaction.Add(t1);
        //    lc.SaveChanges();

        //    try
        //    {

        //        Uri myurl = new Uri("http://192.168.43.101/gpio" + id + "/" + Action);
        //        WebClient wcl = new WebClient();
        //        var content = wcl.DownloadString(myurl);
        //    }

        //    catch
        //    {
        //        return View();
        //    }

        //    return View();
        //}
        //public IActionResult DATAFORARD()
        //{
        //    //step 2 arduino hit this action


        //    //TRYCATCH
        //    Transaction obj = lc.Transaction.Where(u => u.Permission == null && u.Inspect == null).FirstOrDefault<Transaction>();
        //    Appliances appobj = lc.Appliances.Where(u => u.Id == obj.AppId).SingleOrDefault();

        //    obj.Permission = DateTime.Now.ToString();

        //    if (obj.Action == "OFF")
        //    {
        //        IList<Transaction> olist = lc.Transaction.Where(m => m.Action == "ON" && m.AppId == obj.AppId).OrderByDescending(m => m.Id).ToList();
        //        var obj2 = olist.FirstOrDefault();
        //        DateTime startTime= Convert.ToDateTime(obj.Request);
        //        DateTime EndTime = Convert.ToDateTime(obj2.Request);
        //        TimeSpan time =startTime-EndTime;
        //        obj.Timespan = Convert.ToDateTime(time);
        //    }


        //    if (appobj.Status != "ON")
        //    {
        //        appobj.Status = "ON";
        //    }
        //    else
        //    {
        //        appobj.Status = "OFF";
        //    }

        //    lc.Entry(appobj).State = EntityState.Modified;
        //    lc.Entry(obj).State = EntityState.Modified;
        //    lc.SaveChanges();
        //    return Json("Done");
        //}


        #endregion

        #region Arduino REquests
            // new region
        public IActionResult AddTransaction(int id)
        {
            // When user hit on app icon,transaction table me request create hoge 
            // chk current status of app from appliances table and prepare responce againts according to current tatus(Opposite)

            Transaction t1 = new Transaction();
            t1.Request = System.DateTime.Now;
            t1.AppId = id;
            t1.Day = DateTime.Now.DayOfWeek.ToString();


            Appliances app = new Appliances();
            app = lc.Appliances.Where(m => m.Id == id).SingleOrDefault();
            ViewBag.AppStatus = app.Status;
            string Action = "";
            if (app.Status == "OFF")
            {
                Action = "1";
                t1.Action = "ON";
            }
            else
            {
                t1.Action = "OFF";
                Action = "0";
            }
            lc.Transaction.Add(t1);
            lc.SaveChanges();

            try
            {

                Uri myurl = new Uri("http://192.168.43.101/gpio" + id + "/" + Action);
                WebClient wcl = new WebClient();
                var content = wcl.DownloadString(myurl);
            }

            catch
            {
                return View();
            }

            return View();
        }
        public IActionResult DATAFORARD()
        {
            //step 2 arduino hit this action


            //TRYCATCH
            Transaction obj = lc.Transaction.Where(u => u.Permission == null && u.Inspect == null).FirstOrDefault<Transaction>();
            Appliances appobj = lc.Appliances.Where(u => u.Id == obj.AppId).SingleOrDefault();

            obj.Permission = DateTime.Now.ToString();

            if (obj.Action == "OFF")
            {
                IList<Transaction> olist = lc.Transaction.Where(m => m.Action == "ON" && m.AppId == obj.AppId).OrderByDescending(m => m.Id).ToList();
                var obj2 = olist.FirstOrDefault();
                DateTime startTime = Convert.ToDateTime(obj2.Request);
                DateTime EndTime = Convert.ToDateTime(obj.Request);
                TimeSpan time = EndTime - startTime;

               // obj.Timespan = EndTime - startTime;
            }


            if (appobj.Status != "ON")
            {
                appobj.Status = "ON";
            }
            else
            {
                appobj.Status = "OFF";
            }

            lc.Entry(appobj).State = EntityState.Modified;
            lc.Entry(obj).State = EntityState.Modified;
            lc.SaveChanges();
            return Json("Done");
        }


        #endregion

        #region dashboard
        public IActionResult dashoboard()
        {

            if (HttpContext.Session.GetString("Id") != null)
            {

                IList<Room> list = lc.Room.ToList<Room>();
                ViewData["roomCount"] = list.Count;

                IList<Appliances> lista = lc.Appliances.ToList<Appliances>();
                ViewData["appCount"] = lista.Count;

                IList<Residents> listu = lc.Residents.ToList<Residents>();
                ViewData["userpCount"] = listu.Count;

                ViewData["username"] = HttpContext.Session.GetString("Username");


                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #endregion


        #region graph

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

        #endregion

       


        #region email

        public IActionResult email()
        {
            if (HttpContext.Session.GetString("Id") != null)
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
            else
            {
                return RedirectToAction("Login");
            }
        }


        public IActionResult Sendemail(string to, string subject, string body)

        {



            IList<Residents> listi = lc.Residents.ToList<Residents>();



               foreach (var items in listi)
            {
                  to = items.Email;
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


        #endregion




       

        public IActionResult Contact()
        {
            if (HttpContext.Session.GetString("Id")!= null)
            { 
            var id = HttpContext.Session.GetString("Id");
           
                var account = lc.Residents.Where(u => u.Id.ToString() == id).SingleOrDefault();
            if (account.Usertype== "Admin")
            {

                return View();
                
            }
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }

   

        public IActionResult list()
        {
            IList<Room> room = lc.Room.ToList<Room>();
            ViewBag.rm = room;
            return View(ViewBag.rm);

           
        }



         public IActionResult Bill()
        {
           
            if (HttpContext.Session.GetString("Id") != null)
            {
              
                var id = HttpContext.Session.GetString("Id");
                var account = lc.Residents.Where(u => u.Id.ToString() == id).SingleOrDefault();
                if (account.Usertype == "Admin")
                {


                    IList<Room> list = lc.Room.ToList<Room>();

                    return View(list);

                }
            }

            return View();
           
        }


        //public IActionResult Billapp()
        //{

        // //   ViewBag.applist = TempData["app"];
        //   string applist = TempData.Peek("app").ToString();

        //    return View(applist);

        //}

    }
    }

















