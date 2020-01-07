using OnlineExam.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace OnlineExam.Controllers
{

    public class HomeController : Controller
    {
        ExamEntities db = new ExamEntities();
        int c = 0;
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["ad_id"] != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(Admin a, Student s)
        {
            string hash = "";
            hash = Crypto.Hash(a.Password);
            Admin ad = db.Admins.Where(x => x.Email == a.Email && x.Password == hash).SingleOrDefault();
            Student std = db.Students.Where(x => x.Email == s.Email).SingleOrDefault();

            if (ad != null && ad.Email != "Student")
            {
                Session["ad_id"] = ad.Id;
                Session["ad_name"] = ad.Email;
                return RedirectToAction("Dashboard");
            }

            else if (std != null)
            {
                //  List<Admin> li = db.Admins.Where(x => x.Password == a.Password).ToList();
                var ps = db.Admins.Where(x => x.Password == hash).FirstOrDefault();
                if (ps.Id == 2)
                {
                    Session["st_id"] = std.Id;
                    Session["st_Fname"] = std.FirstName;
                    Session["st_Lname"] = std.LastName;
                    Session["st_Email"] = std.Email;
                    return RedirectToAction("Instructions");
                }
                else
                {
                    ViewBag.msg = "Invalid Username Or Password!..";
                }
            }
            else
            {
                ViewBag.msg = "Invalid Username Or Password!..";
            }
            return View();
        }

        [HttpGet]
        public ActionResult AddAdmin()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddAdmin(Admin a, string R)
        {
            Admin add = new Admin();
            if (Session["ad_id"].Equals(1))
            {
                if (R == "Remove")
                {
                    try
                    {
                        Admin del = db.Admins.Where(x => x.Email == a.Email).SingleOrDefault();
                        db.Admins.Remove(del);
                        db.SaveChanges();
                        ViewBag.msg = "Removed Successfully!";
                    }
                    catch
                    {
                        ViewBag.msg = "No such Admin Exist!";
                    }
                }
                else
                {

                    add.Email = a.Email;
                    add.Password = Crypto.Hash(a.Password);
                    db.Admins.Add(add);
                    db.SaveChanges();
                    ViewBag.msg = "Added Successfully!";
                }
            }
            else
            {
                ViewBag.msg = "Don't have Access";
            }
            return View();
        }
        public ActionResult GetData()
        {
            List<Admin> lst = db.Admins.ToList();
            //db.Configuration.ProxyCreationEnabled = false;
            var data = lst.Select(S => new {
                Id = S.Id,
             
                Email = S.Email,
                Password=S.Password

            });
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult Reset()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Reset(Admin a, string NewPassword)
        {
            var k = Session["ad_name"];
            if (Session["ad_id"].Equals(1))
            {
                var re = db.Admins.FirstOrDefault(u => u.Email == a.Email);
                if (NewPassword == a.Password)
                {
                    re.Password = Crypto.Hash(a.Password);
                    db.SaveChanges();
                    ViewBag.msg = "Password Updated!";
                }
                else
                {
                    ViewBag.msg = "Password did not match!";
                }
            }
            else
            {
                var re = db.Admins.FirstOrDefault(u => u.Email == k.ToString());
                if (NewPassword == a.Password)
                {
                    re.Password = Crypto.Hash(a.Password);
                    db.SaveChanges();
                    ViewBag.msg = "Password Updated!";
                }
                else
                {
                    ViewBag.msg = "Password did not match!";
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult SReset()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult SReset(Admin a, string NewPassword)
        {
            var re = db.Admins.FirstOrDefault(u => u.Email == "Student");
            if (NewPassword == a.Password)
            {
                re.Password = Crypto.Hash(a.Password);
                db.SaveChanges();
                ViewBag.msg = "Password Updated!";
            }
            else
            {
                ViewBag.msg = "Password did not match!";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Sregister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Sregister(Student svw)
        {
            try
            {
                Student s = new Student();
                s.FirstName = svw.FirstName;
                s.LastName = svw.LastName;
                s.PhoneNumber = svw.PhoneNumber;
                s.Email = svw.Email;
                s.CollegeName = svw.CollegeName;
                s.CollegeRollNo = svw.CollegeRollNo;
                s.Branch = svw.Branch;
                db.Students.Add(s);
                db.SaveChanges();
                TempData["msg"] = "Registration Successfull !...";
                TempData.Keep();

            }
            catch (Exception)
            {
                ViewBag.msg = "Data not inserted!";
            }
            return View();
            // return RedirectToAction("Sregister");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");
        }


        public ActionResult Instructions()
        {
            var prf = db.Profiles.Where(x => x.Status == 1).FirstOrDefault();
            TempData["profile"] = prf.Name;
            TempData.Keep();
            if (Session["st_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Instructions(string check)
        {
            if (Session["st_id"] == null)
            {
                return RedirectToAction("Index");
            }
            TempData["totalQues"] = 0;
            List <QuesCategory> lis = db.QuesCategories.ToList();
            var prf = db.Profiles.Where(x => x.Status == 1).FirstOrDefault();
            List<Exam> ls = db.Exams.Where(x => x.ProfileId == prf.Id).ToList();

            if (check != null)
            {
                Queue<Question> queue = new Queue<Question>();

                foreach (var quesCat in ls)
                {
                    if (quesCat.CheckBox == true)
                    {
                        int k = Convert.ToInt32(quesCat.NoOfQues);
                        int i = 1;
                        //   List<Question> list = db.Questions.Where(x => x.QuesCategoryId == quesCat.Id).ToList();
                        foreach (var cat in lis)
                        {

                            if (cat.Name == quesCat.Category)
                            {
                                using (var ctx = new ExamEntities())
                                {

                                    var questionList = ctx.Questions.SqlQuery("Select * from Questions where QuesCategoryId=" + cat.Id + " ").Distinct().OrderBy(x => Guid.NewGuid()).ToList();


                                    foreach (Question a in questionList)
                                    {
                                        if (i <= k)
                                        {
                                            queue.Enqueue(a);
                                            i++;
                                            TempData["totalQues"] = Convert.ToInt32(TempData["totalQues"]) + 1;
                                        }
                                        else
                                            break;

                                    }

                                }
                            }
                        }
                    }
                }
                TempData["questions"] = queue;
                TempData["score"] = 0;
                TempData["Indi"] = "";
                TempData["i"] = 0;
                TempData["sc1"] = null;
                TempData["sc2"] = null;
                TempData["sc3"] = null;
                TempData["attempt"] = 0;
                TempData["ExamName"] = prf.Name;
               // for(int i = 1; i < queue.Count; i++)
               // {
                //    TempData["sl"] = i;
                //}
                TempData.Keep();


                return RedirectToAction("Ncat");
            }
            else
            {
                ViewBag.error = "Kindly Check the box";
            }
            return View();
        }
        public ActionResult Ncat()
        {
           
            var prf = db.Profiles.Where(x => x.Status == 1).FirstOrDefault();
            float dt = (float)prf.Duration;
            if (Session["st_id"] == null)
            {
                return RedirectToAction("Index");
            }
            if (Session["Rem_Time"] == null)
            {
                Session["Rem_Time"] = DateTime.Now.AddMinutes(dt).ToString("dd-MM-yyyy h:mm:ss tt");

            }

            TempData["sl"] = Convert.ToInt32(TempData["sl"]) + 1;

            Question q = null;
            if (TempData["questions"] != null)
            {

                Queue<Question> qlist = (Queue<Question>)TempData["questions"];
                //   Session["test"] = c++;

                //  TempData["sl"] = c;
                //   TempData.Keep();

               // TempData["sl"] = qlist.First();
                //   TempData["sl"] = Convert.ToInt32(qlist.Distinct());
                //TempData["sl"] = qlist.ToArray();
                if (qlist.Count > 0)
                {
                    q = qlist.Peek();
                    qlist.Dequeue();
                    TempData["questions"] = qlist;
                    TempData.Keep();

                }
                else
                {
                    return RedirectToAction("EndExam");
                }
            }
            else
            {
                return RedirectToAction("StudentExam");
            }




            return View(q);
        }

        [HttpPost]
        public ActionResult Ncat(Question q, string save, string submit, string option)
        {

            var que = db.Questions.Where(x => x.Id == q.Id).FirstOrDefault();

            var cat = db.QuesCategories.Where(x => x.Id == que.QuesCategoryId).FirstOrDefault();
            if (Convert.ToString(TempData["Indi"]) != cat.Name)
            {
                TempData["sc"] = 0;
                TempData["Indi"] = cat.Name;
                TempData["i"] = Convert.ToInt32(TempData["i"]) + 1;
                TempData["1st"] = false;
                TempData["2nd"] = false;
                TempData["3rd"] = false;
            }

            string correctans = null;
            if (submit == null)
            {
                if (save != null)
                {

                    if (option != null)
                    {
                        correctans = option;
                    }

                    if (correctans == que.COP && correctans != null)
                    {
                        TempData["score"] = Convert.ToInt32(TempData["score"]) + 4;
                        TempData["attempt"] = Convert.ToInt32(TempData["attempt"]) + 1;
                        TempData["sc"] = Convert.ToInt32(TempData["sc"]) + 4;

                    }
                    else if (correctans != que.COP && correctans != null)
                    {
                        TempData["score"] = Convert.ToInt32(TempData["score"]) + -1;
                        TempData["attempt"] = Convert.ToInt32(TempData["attempt"]) + 1;
                        TempData["sc"] = Convert.ToInt32(TempData["sc"]) + -1;

                    }

                    else
                    {
                        TempData["score"] = Convert.ToInt32(TempData["score"]) + 0;
                    }
                }
                else
                {
                    TempData["score"] = Convert.ToInt32(TempData["score"]) + 0;
                }

                if (Convert.ToInt32(TempData["i"]) == 1)
                {
                    TempData["sc1"] = TempData["sc"];
                    TempData["id1"] = TempData["Indi"];

                }
                else if (Convert.ToInt32(TempData["i"]) == 2)
                {
                    TempData["sc2"] = TempData["sc"];
                    TempData["id2"] = TempData["Indi"];

                }
                else if (Convert.ToInt32(TempData["i"]) == 3)
                {
                    TempData["sc3"] = TempData["sc"];
                    TempData["id3"] = TempData["Indi"];

                }
                else if (Convert.ToInt32(TempData["i"]) > 3)
                {
                    if (Convert.ToBoolean(TempData["1st"]) == true)
                    {
                        TempData["sc1"] = TempData["sc"];
                        TempData["id1"] = TempData["Indi"];
                    }
                    else if (Convert.ToBoolean(TempData["2nd"]) == true)
                    {
                        TempData["sc2"] = TempData["sc"];
                        TempData["id2"] = TempData["Indi"];
                    }
                    else if (Convert.ToBoolean(TempData["1st"]) == true)
                    {
                        TempData["sc3"] = TempData["sc"];
                        TempData["id3"] = TempData["Indi"];
                    }
                    else if (Convert.ToInt32(TempData["sc1"]) <= Convert.ToInt32(TempData["sc2"]) && Convert.ToInt32(TempData["sc1"]) <= Convert.ToInt32(TempData["sc3"]))
                    {
                        TempData["1st"] = true;
                        TempData["sc1"] = TempData["sc"];
                        TempData["id1"] = TempData["Indi"];
                    }
                    else if (Convert.ToInt32(TempData["sc2"]) <= Convert.ToInt32(TempData["sc3"]) && Convert.ToInt32(TempData["sc2"]) <= Convert.ToInt32(TempData["sc1"]))
                    {
                        TempData["2nd"] = true;
                        TempData["sc2"] = TempData["sc"];
                        TempData["id2"] = TempData["Indi"];
                    }
                    else if (Convert.ToInt32(TempData["sc3"]) <= Convert.ToInt32(TempData["sc1"]) && Convert.ToInt32(TempData["sc3"]) <= Convert.ToInt32(TempData["sc2"]))
                    {
                        TempData["3rd"] = true;
                        TempData["sc3"] = TempData["sc"];
                        TempData["id3"] = TempData["Indi"];

                    }
                }


                TempData.Keep();
                return RedirectToAction("Ncat");
            }
            else
            {
                if (option != null)
                {
                    correctans = option;
                }

                if (correctans == que.COP && correctans != null)
                {
                    TempData["score"] = Convert.ToInt32(TempData["score"]) + 4;
                    TempData["attempt"] = Convert.ToInt32(TempData["attempt"]) + 1;
                    TempData["sc"] = Convert.ToInt32(TempData["sc"]) + 4;


                }
                else if (correctans != que.COP && correctans != null)
                {
                    TempData["score"] = Convert.ToInt32(TempData["score"]) + -1;
                    TempData["attempt"] = Convert.ToInt32(TempData["attempt"]) + 1;
                    TempData["sc"] = Convert.ToInt32(TempData["sc"]) + -1;


                }

                else
                {
                    TempData["score"] = Convert.ToInt32(TempData["score"]) + 0;
                }
                if (Convert.ToInt32(TempData["i"]) == 1)
                {
                    TempData["sc1"] = TempData["sc"];
                    TempData["id1"] = TempData["Indi"];

                }
                else if (Convert.ToInt32(TempData["i"]) == 2)
                {
                    TempData["sc2"] = TempData["sc"];
                    TempData["id2"] = TempData["Indi"];

                }
                else if (Convert.ToInt32(TempData["i"]) == 3)
                {
                    TempData["sc3"] = TempData["sc"];
                    TempData["id3"] = TempData["Indi"];

                }
                else if (Convert.ToInt32(TempData["i"]) > 3)
                {
                    if (Convert.ToBoolean(TempData["1st"]) == true)
                    {
                        TempData["sc1"] = TempData["sc"];
                        TempData["id1"] = TempData["Indi"];
                    }
                    else if (Convert.ToBoolean(TempData["2nd"]) == true)
                    {
                        TempData["sc2"] = TempData["sc"];
                        TempData["id2"] = TempData["Indi"];
                    }
                    else if (Convert.ToBoolean(TempData["1st"]) == true)
                    {
                        TempData["sc3"] = TempData["sc"];
                        TempData["id3"] = TempData["Indi"];
                    }
                    else if (Convert.ToInt32(TempData["sc1"]) <= Convert.ToInt32(TempData["sc2"]) && Convert.ToInt32(TempData["sc1"]) <= Convert.ToInt32(TempData["sc3"]))
                    {
                        TempData["1st"] = true;
                        TempData["sc1"] = TempData["sc"];
                        TempData["id1"] = TempData["Indi"];
                    }
                    else if (Convert.ToInt32(TempData["sc2"]) <= Convert.ToInt32(TempData["sc3"]) && Convert.ToInt32(TempData["sc2"]) <= Convert.ToInt32(TempData["sc1"]))
                    {
                        TempData["2nd"] = true;
                        TempData["sc2"] = TempData["sc"];
                        TempData["id2"] = TempData["Indi"];
                    }
                    else if (Convert.ToInt32(TempData["sc3"]) <= Convert.ToInt32(TempData["sc1"]) && Convert.ToInt32(TempData["sc3"]) <= Convert.ToInt32(TempData["sc2"]))
                    {
                        TempData["3rd"] = true;
                        TempData["sc3"] = TempData["sc"];
                        TempData["id3"] = TempData["Indi"];
                    }
                }

                return RedirectToAction("EndExam");
            }
        }

        public ActionResult EndExam()
        {
            if (Session["st_id"] == null)
            {
                return RedirectToAction("Index");
            }

           
            if (Convert.ToInt32(TempData["score"]) >= 0)
            {
                // TempData["percentage"] = (Convert.ToInt32(TempData["score"]) / ((ct-1 )* 4)) * 100;
                TempData["percentage"] = (Math.Round((Convert.ToDouble(TempData["score"]) / (Convert.ToDouble(TempData["totalQues"]) * 4)) * 100));
                TempData.Keep();
            }
            else
            {
                TempData["percentage"] = 0;
                TempData.Keep();
            }

            Report ex = new Report();
            ex.Date = System.DateTime.Now;
            ex.ExamName = Convert.ToString(TempData["ExamName"]);
            ex.Score = Convert.ToInt32(TempData["score"]);
            ex.StudentId = Convert.ToInt32(Session["st_id"]);
            ex.StudentName = Convert.ToString(Session["st_Fname"]) + " " + Convert.ToString(Session["st_Lname"]);
            ex.Email = Convert.ToString(Session["st_Email"]);

            int[] arr = new int[5];
            string[] brr = new string[5];
            arr[1] = Convert.ToInt32(TempData["sc1"]);
            arr[2] = Convert.ToInt32(TempData["sc2"]);
            arr[3] = Convert.ToInt32(TempData["sc3"]);

            brr[1] = Convert.ToString(TempData["id1"]);
            brr[2] = Convert.ToString(TempData["id2"]);
            brr[3] = Convert.ToString(TempData["id3"]);

            for (int i = 1; i < 3; i++)
            {
                for (int j = i + 1; j <= 3; j++)
                {
                    if (arr[j] >= arr[i])
                    {
                        int x = arr[j];
                        arr[j] = arr[i];
                        arr[i] = x;

                        string y = brr[j];
                        brr[j] = brr[i];
                        brr[i] = y;
                    }
                }
            }

            ex.Subject1 = brr[1];
            ex.Marks1 = arr[1];


            ex.Subject2 = brr[2];
            ex.Marks2 = arr[2];

            ex.Subject3 = brr[3];
            ex.Marks3 = arr[3];

            ex.Percentage = Convert.ToInt32(TempData["percentage"]);
            db.Reports.Add(ex);
            db.SaveChanges();
            TempData["st_Fname"] = Convert.ToString(Session["st_Fname"]) + " " + Convert.ToString(Session["st_Lname"]);
            TempData["st_Email"] = Convert.ToString(Session["st_Email"]);

            TempData["Sub1"] = brr[1];
            TempData["mar1"] = arr[1];

            TempData["Sub2"] = brr[2];
            TempData["mar2"] = arr[2]; ;

            TempData["Sub3"] = brr[3];
            TempData["mar2"] = arr[3]; ;
            Session.RemoveAll();
            return View();
        }
        public ActionResult Dashboard()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            //SetExam sc = new SetExam();
            //  List<SetExam> li = sc.getDashboard(Convert.ToInt32(Session["ad_id"]));
            return View();

        }
        public ActionResult Result()
        {
            List<Report> lst = db.Reports.ToList();
            //db.Configuration.ProxyCreationEnabled = false;
            var data = lst.Select(S => new {
                StudentId = S.StudentId,
                StudentName = S.StudentName,
                Email = S.Email,
                Date = S.Date.ToString("dd/MM/yyyy"),
                ExamName = S.ExamName,
                Subject1 = S.Subject1,
                Marks1 = S.Marks1,
                Subject2 = S.Subject2,
                Marks2 = S.Marks2,
                Subject3 = S.Subject3,
                Marks3 = S.Marks3,
                Score = S.Score,
                Percentage = S.Percentage

            });
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            //Session["ad_id"] = 2;

            return View();
        }
        [HttpPost]
        public ActionResult AddCategory(QuesCategory cat)
        {

            QuesCategory c = new QuesCategory();
            c.Name = cat.Name;
            c.TotalQuestion = 0;
            c.AdminId = Convert.ToInt32(Session["ad_id"].ToString());
            db.QuesCategories.Add(c);
            db.SaveChanges();

            List<Profile> lis = db.Profiles.ToList();
            foreach (var item in lis)
            {
                Exam ex = new Exam();
                ex.Category = cat.Name;
                ex.NoOfQues = 0;
                ex.CheckBox = false;
                ex.ProfileId = item.Id;
                db.Exams.Add(ex);
                db.SaveChanges();
            }
            TempData["msg"] = "Category Added Successfully!";
            return RedirectToAction("AddCategory");
        }
        public ActionResult ViewCategory()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            //Session["ad_id"] = 2;
            // int adid = Convert.ToInt32(Session["ad_id"].ToString());
            List<QuesCategory> li = db.QuesCategories.ToList();
            ViewData["List"] = li;
            return View();
        }


        [HttpGet]
        public ActionResult SetExam()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }

            List<Profile> li = db.Profiles.ToList();
            //ViewData["Lis"] = li;
            foreach (var st in li)
            {
                if (st.Status == 1)
                    ViewBag.list = new SelectList(li, "Id", "Name", st.Id);
            }
            List<QuesCategory> qc = db.QuesCategories.ToList();
            ViewData["QuesCat"] = qc;

            foreach (var item in li)
            {
                if (item.Status == 1)
                {

                    /* List<Exam> lis = db.Exams.Where(x => x.ProfileId == item.Id).ToList();
                     ViewData["Liss"] = lis;*/
                    var data = (from s1 in db.Profiles
                                join s2 in db.Exams on s1.Id equals s2.ProfileId
                                where s1.Id == item.Id
                                select new ProfileExam { Profile = s1, Exam = s2 }).ToList();
                    return View(data);
                }
            }

            ViewBag.list = new SelectList(li, "Id", "Name");

            return View();

        }

        [HttpPost]
        public ActionResult SetExam(FormCollection c, string ApplyProfile, string AddProfile, string sub, string num, Profile pf)
        {

            if (ApplyProfile != null)
            {
                //pf.Status = 1;
                List<Profile> li = db.Profiles.ToList();
                foreach (var item in li)
                {
                    item.Status = 0;
                }
                //  List<ProfileExam> profiles=db.Profiles.
                try
                {
                    var pro = db.Profiles.Where(x => x.Id == pf.Id).FirstOrDefault();
                    pro.Status = 1;
                    db.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("SetExam");
                }


            }

            if (AddProfile != "")
            {
                Profile pro = new Profile();
                pro.Name = AddProfile;
                pro.Status = 0;
                db.Profiles.Add(pro);
                db.SaveChanges();

                List<QuesCategory> lis = db.QuesCategories.ToList();
                Exam ex = new Exam();
                foreach (var item in lis)
                {
                    ex.Category = item.Name;
                    ex.NoOfQues = 0;
                    ex.CheckBox = false;
                    ex.ProfileId = pro.Id;
                    db.Exams.Add(ex);
                    db.SaveChanges();
                }
            }

            int i = 0;
            if (sub != null)
            {
                int t = 0;
                string[] Check = new string[100];
                var ID = c.GetValues("item.Exam.Id");
                var CATEGORY = c.GetValues("item.Exam.Category");
                var CHECKBOX = c.GetValues("item.Exam.CheckBox");
                var Ques = c.GetValues("item.Exam.NoOfQues");
                
                for (int k = 0; k < CHECKBOX.Length; k++)
                {
                    if (t < CHECKBOX.Length)
                    {
                        if (Convert.ToBoolean(CHECKBOX[t]).Equals(true))
                        {
                            Check[i] = CHECKBOX[t];
                            t = t + 2;
                            i++;
                        }
                        else
                        {
                            Check[i] = CHECKBOX[t];
                            t = t + 1;
                            i++;
                        }
                    }
                    else if (t == CHECKBOX.Length)
                    {
                        Check[i] = CHECKBOX[k];
                    }
                }
                t = 0;
                for (i = 0; i < ID.Count(); i++)
                {
                    Exam ex = db.Exams.Find(Convert.ToInt32(ID[i]));
                    ex.CheckBox = Convert.ToBoolean(Check[i]);
                    if (Check[i] == "true")
                    {
                        ex.NoOfQues = Convert.ToInt32(Ques[i]);
                        t = t + Convert.ToInt32(Ques[i]);
                    }
                    db.Entry(ex).State = EntityState.Modified;

                }
                TempData["selectedQues"] = t;
                TempData.Keep();
                if (num == Convert.ToString(t))
                {
                    db.SaveChanges();
                    TempData["msg"] = "Saved successfully!";
                }
                else
                {
                    TempData["msg"] = "Set total is not equal to number of question!";
                }
            }



            return RedirectToAction("SetExam");
        }

        public ActionResult ViewAllQuestions(int? id, int? page)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return RedirectToAction("Dashboard");
            }
            int pagesize = 10, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            List<Question> li = db.Questions.Where(x => x.QuesCategoryId == id).ToList();
            IPagedList<Question> stu = li.ToPagedList(pageindex, pagesize);

            return View(stu);
        }

        [HttpGet]
        public ActionResult AddQuestion()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            // int sid= Convert.ToInt32(Session["ad_id"]);
            List<QuesCategory> li = db.QuesCategories.ToList();
            ViewBag.list = new SelectList(li, "Id", "Name");
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddQuestion(Question q)
        {
            // int sid = Convert.ToInt32(Session["ad_id"]);
            List<QuesCategory> li = db.QuesCategories.ToList();
            ViewBag.list = new SelectList(li, "Id", "Name");
            Question QA = new Question();
            QA.Text = q.Text;
            QA.OPA = q.OPA;
            QA.OPB = q.OPB;
            QA.OPC = q.OPC;
            QA.OPD = q.OPD;
            QA.COP = q.COP;
            QA.QuesCategoryId = q.QuesCategoryId;
            try
            {
                db.Questions.Add(QA);
                db.SaveChanges();
                TempData["msg"] = "Question added successfully....";
                TempData.Keep();

            }
            catch (Exception e)
            {
                TempData["msg"] = e.Message;
            }

            var total = db.Questions.SqlQuery(" SELECT * FROM Questions Where QuesCategoryId=" + q.QuesCategoryId + " ").Count();
            var qc = db.QuesCategories.FirstOrDefault(u => u.Id == q.QuesCategoryId);
            qc.TotalQuestion = total;
            db.SaveChanges();


            return RedirectToAction("AddQuestion");

        }
        [HttpGet]
        public ActionResult EditQuestion(int? id)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question m = db.Questions.Find(id);
            if (m == null)
            {
                return HttpNotFound();
            }
            return View(m);
        }

        [HttpPost]
        public ActionResult EditQuestion(Question m)
        {
            var qs = db.Questions.Where(x => x.Id == m.Id).FirstOrDefault();
            qs.Text = m.Text;
            qs.OPA = m.OPA;
            qs.OPB = m.OPB;
            qs.OPC = m.OPC;
            qs.OPD = m.OPD;
            qs.COP = m.COP;
            db.SaveChanges();
            /* if (ModelState.IsValid)
             {
                 db.Entry(m).State = EntityState.Modified;
                 db.SaveChanges();
                 return RedirectToAction("Index");
             }*/
            return View(m);
        }

        //QuestionDeletion
        public ActionResult DeleteQuestion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question subject_tbl = db.Questions.Find(id);
            db.Questions.Remove(subject_tbl);
            db.SaveChanges();

            var total = db.Questions.SqlQuery(" SELECT * FROM Questions Where QuesCategoryId=" + subject_tbl.QuesCategoryId + " ").Count();
            var qc = db.QuesCategories.FirstOrDefault(u => u.Id == subject_tbl.QuesCategoryId);
            qc.TotalQuestion = total;
            db.SaveChanges();

            return RedirectToAction("ViewCategory");
        }
        public ActionResult About()
        {
            List<Profile> li = db.Profiles.ToList();
            //ViewData["Lis"] = li;
            ViewBag.list = new SelectList(li, "Status", "Name", 1);

            foreach (var item in li)
            {
                if (item.Status == 1)
                {

                    /* List<Exam> lis = db.Exams.Where(x => x.ProfileId == item.Id).ToList();
                     ViewData["Liss"] = lis;*/
                    var data = (from s1 in db.Profiles
                                join s2 in db.Exams on s1.Id equals s2.ProfileId
                                where s1.Id == item.Id
                                select new ProfileExam { Profile = s1, Exam = s2 }).ToList();
                    return View(data);
                }
            }
            return View();
            /* using (var db=new ExamEntities())
             {

                 var data = (from s1 in db.Profiles join s2 in db.Exams on s1.Id equals s2.ProfileId where s1.Id == k
                             select new ProfileExam {Profile=s1, Exam=s2 }).ToList();
                 return View(data);*/

            // ExamEntities DbCompany = new ExamEntities();
            /* var data = from item in db.Exams
                        where item.ProfileId == 1
                        orderby item.Id
                        select item;
             return View(data.ToList());*/
        }

        [HttpPost]
        public ActionResult About(FormCollection c, string ApplyProfile, string AddProfile, string sub, string num, Profile pf)
        {
            if (ApplyProfile != null)
            {
                List<Profile> li = db.Profiles.ToList();
                foreach (var item in li)
                {
                    item.Status = 0;
                }
                var pro = db.Profiles.Where(x => x.Status == pf.Status).FirstOrDefault();
                pro.Status = 1;
                db.SaveChanges();

            }
            if (AddProfile != "")
            {
                Profile pro = new Profile();
                pro.Name = AddProfile;
                pro.Status = 0;
                db.Profiles.Add(pro);
                db.SaveChanges();
            }

            int i = 0;
            if (sub != null)
            {
                int t = 0;
                string[] Check = new string[100];
                var ID = c.GetValues("item.Exam.Id");
                var CATEGORY = c.GetValues("item.Exam.Category");
                var CHECKBOX = c.GetValues("item.Exam.CheckBox");
                var Ques = c.GetValues("item.Exam.NoOfQues");
                for (int k = 0; k < CHECKBOX.Length; k++)
                {
                    if (t < CHECKBOX.Length)
                    {
                        if (Convert.ToBoolean(CHECKBOX[t]).Equals(true))
                        {
                            Check[i] = CHECKBOX[t];
                            t = t + 2;
                            i++;
                        }
                        else
                        {
                            Check[i] = CHECKBOX[t];
                            t = t + 1;
                            i++;
                        }
                    }
                    else if (t == CHECKBOX.Length)
                    {
                        Check[i] = CHECKBOX[k];
                    }
                }
                t = 0;
                for (i = 0; i < ID.Count(); i++)
                {
                    Exam ex = db.Exams.Find(Convert.ToInt32(ID[i]));
                    ex.CheckBox = Convert.ToBoolean(Check[i]);
                    if (Check[i] == "true")
                    {
                        ex.NoOfQues = Convert.ToInt32(Ques[i]);
                        t = t + Convert.ToInt32(Ques[i]);
                    }
                    db.Entry(ex).State = EntityState.Modified;

                }
                if (num == Convert.ToString(t))
                {
                    db.SaveChanges();
                    TempData["msg"] = "Saved successfully!";
                }
                else
                {
                    TempData["msg"] = "Set total is not equal to number of question!";
                }
            }
            return RedirectToAction("About");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();


        }

    }
}