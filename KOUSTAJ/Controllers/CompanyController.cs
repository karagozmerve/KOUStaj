using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projeb.Models.Entity;
using System.Net;
using System.Net.Mail;

namespace projeb.Controllers
{
    public class CompanyController : Controller
    {
        projebEntities1 db = new projebEntities1();
        // GET: Company
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Giris()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Giris(FormCollection form)
        {
            string maili = form["maili"].ToString();
            string sifre = form["sifre"].ToString();
            var kontrol = db.sirket.FirstOrDefault(m => m.maili == maili && m.sifre == sifre);
            if (kontrol != null)
            {
                Session.Add("smail", form["maili"].ToString());
                foreach (var i in db.sirket.ToList())
                {
                    if (i.maili == maili)
                    {
                        Session.Add("sadi", i.adi);
                    }
                }
                return RedirectToAction("Index", "Company");


            }
            else
            {
                ViewBag.Message = "Mail Adresi veya Şifre Hatalı.Lütfen Tekrar deneyin. Denemeniz Başarısız Olmaya Devam Ederse Mail Atabilirsiniz.";
                return View();
            }
        }
        public ActionResult SifremiUnuttum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifremiUnuttum(FormCollection form)
        {
            string maili = form["maili"].ToString();
            var model = db.sirket.Where(x => x.maili == maili).FirstOrDefault();
            if (model != null)
            {
                Guid rastgele = Guid.NewGuid();
                model.sifre = rastgele.ToString().Substring(0, 8);
                db.SaveChanges();
                SmtpClient client = new SmtpClient("smtp.yandex.com", 587);
                client.EnableSsl = true;
                MailMessage mailx = new MailMessage();
                mailx.From = new MailAddress("bendevarimuygulama@yandex.com", "Şifre Sıfırlama");
                mailx.To.Add(maili);
                mailx.IsBodyHtml = true;
                mailx.Subject = "Şifre Sıfırlama İsteği";
                mailx.Body += "Merhaba " + model.adi + "<br /> Yeni Şifreniz: " + model.sifre;
                client.Credentials = new NetworkCredential("bendevarimuygulama@yandex.com", "123bendevarim");


                client.Send(mailx);
                return RedirectToAction("Giris");
            }
            else
            {
                ViewBag.hata = "Sistemde böyle bir mail adresi mevcut değildir.";
                return View();
            }
        }
        public ActionResult IlanVer()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IlanVer(ilann ilanver)
        {
            string smail=Session["smail"].ToString();
            string sadi= Session["sadi"].ToString();
            ilanver.sirketadi = sadi;
            db.ilann.Add(ilanver);
            db.SaveChanges();
            foreach(var bul in db.sirket.ToList())
            {
                if (bul.maili == smail)
                {
                    sirket_ilan sirkt = new sirket_ilan();
                    sirkt.ilanid = ilanver.id;
                    sirkt.sirketid = bul.id;
                    db.sirket_ilan.Add(sirkt);
                    db.SaveChanges();
                }
            }
            
            return View();
        }
        public ActionResult Ilanlarim()
        {
            List<ilann> ObjCustomer = new List<ilann>();
            foreach (var i in db.sirket.ToList())
            {
                if (i.maili == Session["smail"].ToString())
                {
                    foreach(var c in db.sirket_ilan.ToList())
                    {
                        if (c.sirketid == i.id)
                        {
                            foreach(var ilan in db.ilann.ToList())
                            {
                                if (ilan.id == c.ilanid)
                                {
                                    ilann Obj = new ilann();
                                    Obj.id = ilan.id;
                                    Obj.ilanaciklama = ilan.ilanaciklama;
                                    ObjCustomer.Add(Obj);

                                }
                            }
                        }
                    }
                }
            }
            return View(ObjCustomer.ToList());
        }
        public ActionResult BasvuruGor(int id)
        {
            List<ogrenci> ObjCustomer = new List<ogrenci>();

            foreach (var e in db.basvuru.ToList())
            {
                if (e.ilan_id == id)
                {
                    foreach(var c in db.ogrenci.ToList())
                    {
                        if (c.id == e.ogrenci_id)
                        {
                            ogrenci Obj = new ogrenci();
                            Obj.id = c.id;
                            Obj.adsoyad = c.adsoyad;
                            Obj.mail = c.mail;
                            Obj.sinif = c.sinif;
                            ObjCustomer.Add(Obj);
                        }
                    }
                }
            }
            return View(ObjCustomer.ToList());
        }
        public ActionResult CVGor(int id)
        {
            foreach (var goruntule in db.cvogrenci.ToList())
            {
                if (goruntule.ogrid == id)
                {
                    foreach (var c in db.cv.ToList())
                    {
                        if (goruntule.cvid == c.id)
                        {
                            ViewBag.mesaj = c.dosyadi;
                            return View();
                        }
                    }
                }
                      
            }
            return View();
        }

        public ActionResult IlanDuzenle(int id)
        {
            var std = db.ilann.Where(s => s.id == id).FirstOrDefault();
            return View(std);
        }
        [HttpPost]
        [ValidateInput(false)]

        public ActionResult IlanDuzenle(ilann ilanim)
        {
            ilann updated = (from c in db.ilann
                               where c.id == ilanim.id
                               select c).FirstOrDefault();

            updated.ilanaciklama= ilanim.ilanaciklama;
            db.SaveChanges();
            return View();
        }
        public ActionResult IlanSil(int id)
        {
            basvuru silm = (from a in db.basvuru where a.ilan_id == id select a).FirstOrDefault();
            db.basvuru.Remove(silm);
            db.SaveChanges();
            sirket_ilan silme = (from a in db.sirket_ilan where a.ilanid == id select a).FirstOrDefault();
            db.sirket_ilan.Remove(silme);
            db.SaveChanges();
            ilann sil = (from c in db.ilann
                           where c.id == id
                           select c).FirstOrDefault();
            db.ilann.Remove(sil);
            db.SaveChanges();
            return RedirectToAction("Ilanlarim");
        }
        public ActionResult Cikis()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Giris", "Company");
        }
        public ActionResult Hesabim()
        {
            foreach (var e in db.sirket.ToList())
            {
                if (e.maili.Trim() == Session["smail"].ToString())
                {
                    var b = db.sirket.Where(s => s.id == e.id).FirstOrDefault();
                    return View(b);
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult Hesabim(sirket k)
        {
            sirket updated = (from c in db.sirket
                               where c.id == k.id
                               select c).FirstOrDefault();


            updated.adi= k.adi;
            updated.alani = k.alani;
            updated.maili = k.maili;
            updated.sifre = k.sifre;
            db.SaveChanges();
            return View();
        }
    }
}