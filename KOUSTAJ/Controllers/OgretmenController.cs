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
    public class OgretmenController : Controller
    {
        projebEntities1 db = new projebEntities1();
        // GET: Ogretmen
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
            string mail = form["mail"].ToString();
            string sifre = form["sifre"].ToString();
            var kontrol = db.ogretmen.FirstOrDefault(m => m.mail == mail && m.sifre == sifre);
            if (kontrol != null)
            {
                Session.Add("omail", form["mail"].ToString());
                foreach (var i in db.ogretmen.ToList())
                {
                    if (i.mail == mail)
                    {
                        Session.Add("oadi", i.adsoyad);
                        Session.Add("oid", i.id);
                    }
                }
                return RedirectToAction("Index", "Ogretmen");


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
            string mail = form["mail"].ToString();
            var model = db.ogretmen.Where(x => x.mail == mail).FirstOrDefault();
            if (model != null)
            {
                Guid rastgele = Guid.NewGuid();
                model.sifre = rastgele.ToString().Substring(0, 8);
                db.SaveChanges();
                SmtpClient client = new SmtpClient("smtp.yandex.com", 587);
                client.EnableSsl = true;
                MailMessage mailx = new MailMessage();
                mailx.From = new MailAddress("bendevarimuygulama@yandex.com", "Şifre Sıfırlama");
                mailx.To.Add(mail);
                mailx.IsBodyHtml = true;
                mailx.Subject = "Şifre Sıfırlama İsteği";
                mailx.Body += "Merhaba " + model.adsoyad + "<br /> Yeni Şifreniz: " + model.sifre;
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
        public ActionResult Cikis()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Giris", "Ogretmen");
        }
        public ActionResult Hesabim()
        {
            foreach (var e in db.ogretmen.ToList())
            {
                if (e.mail.Trim() == Session["omail"].ToString())
                {
                    var b = db.ogretmen.Where(s => s.id == e.id).FirstOrDefault();
                    return View(b);
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult Hesabim(ogretmen k)
        {
            ogretmen updated = (from c in db.ogretmen
                              where c.id == k.id
                              select c).FirstOrDefault();

            updated.adsoyad = k.adsoyad;
            updated.tckimlik = k.tckimlik;
            updated.mail = k.mail;
            updated.sifre = k.sifre;
            db.SaveChanges();
            return View();
        }
        public ActionResult Ogrenciler()
        {
            List<ogrenci> ObjCustomer = new List<ogrenci>();

            int oid = Convert.ToInt32(Session["oid"]);
            foreach(var c in db.atama.ToList())
            {
                if (oid == c.ogretmenid)
                {
                    foreach(var o in db.ogrenci.ToList())
                    {
                        if (o.id == c.ogrid)
                        {
                            ogrenci Obj = new ogrenci();
                            Obj.id = c.ogrid;
                            Obj.adsoyad = o.adsoyad;
                            Obj.ogrno = o.ogrno;
                            Obj.mail = o.mail;
                            Obj.sinif = o.sinif;
                            ObjCustomer.Add(Obj);
                        }
                    }
                }
            }

            return View(ObjCustomer.ToList());
        }
        public ActionResult KFGor(int id)
        {
            foreach (var goruntule in db.kabulformuogrenci.ToList())
            {
                if (goruntule.ogrid == id)
                {
                    if (goruntule.durum == "gonderildi")
                    {
                        foreach (var c in db.kabulformu.ToList())
                        {
                            if (goruntule.kfid == c.id)
                            {
                                ViewBag.mesaj = c.dosyaadi;
                                ViewBag.idbilgi = c.id;

                                return View();
                            }
                        }
                    }
                    else
                    {
                        ViewBag.msj = "Bu öğrenci tarafından kabul formu gönderimi henüz olmamıştır.";
                        return View();

                    }

                }

            }
            return View();
        }
        public ActionResult KFOnay(int id)
        {
            foreach (var k in db.kabulformuogrenci.ToList())
            {
                if (k.kfid == id)
                {
                   kabulformuogrenci updated = (from c in db.kabulformuogrenci
                                       where c.id == id
                                       select c).FirstOrDefault();
                    updated.onaydurumu = "onaylandi";
                    db.SaveChanges();
                    return RedirectToAction("Ogrenciler");

                }
            }
            return RedirectToAction("Ogrenciler");
        }
        public ActionResult SDOnay(FormCollection form)
        {
            int id = Convert.ToInt32(Session["stajdefid"]);
            foreach (var k in db.stajdefogrenci.ToList())
            {
                if (k.stajdefid == id)
                {
                    stajdefogrenci updated = (from c in db.stajdefogrenci
                                                 where c.id == id
                                                 select c).FirstOrDefault();
                    updated.onaydurumu = form["onaydurumu"].Trim();
                    db.SaveChanges();
                    return RedirectToAction("Ogrenciler");

                }
            }
            return RedirectToAction("Ogrenciler");
        }
        public ActionResult SDGor(int id)
        {
            foreach (var goruntule in db.stajdefogrenci.ToList())
            {
                if (goruntule.ogrid == id)
                {
                    if (goruntule.durum == "gonderildi")
                    {
                        foreach (var c in db.stajdef.ToList())
                        {
                            if (goruntule.stajdefid == c.id)
                            {
                                ViewBag.mesaj = c.dosyaadi;
                                Session.Add("stajdefid", c.id);
                                return View();
                            }
                        }
                    }
                    else
                    {
                        ViewBag.msj = "Bu öğrenci tarafından staj defteri gönderimi henüz olmamıştır.";
                        return View();

                    }

                }

            }
            return View();
        }
        public ActionResult Mesajgonder()
        {
            List<ogrenci> ObjCustomer = new List<ogrenci>();

            int oid = Convert.ToInt32(Session["oid"]);
            foreach (var c in db.atama.ToList())
            {
                if (oid == c.ogretmenid)
                {
                    foreach (var o in db.ogrenci.ToList())
                    {
                        if (o.id == c.ogrid)
                        {
                            ogrenci Obj = new ogrenci();
                            Obj.id = c.ogrid;
                            Obj.adsoyad = o.adsoyad;
                            ObjCustomer.Add(Obj);
                        }
                    }
                }
            }

            return View(ObjCustomer.ToList());
        }
        [HttpPost]
        public ActionResult Mesajgonder(FormCollection form)
        {
            int oid = Convert.ToInt32(Session["oid"]);
            mesaj msj = new mesaj();
            msj.ogrtid = oid;
            msj.ogrid = Convert.ToInt32(form["ogrenciadi"]);
            foreach(var c in db.ogrenci.ToList())
            {
                if (c.id == msj.ogrid)
                {
                    msj.ogradi = c.adsoyad;
                }
            }
            msj.ogrtadi = Session["oadi"].ToString();
            msj.konu = form["konu"];
            msj.mesaj1 = form["mesaj1"];
            db.mesaj.Add(msj);
            db.SaveChanges();
            return RedirectToAction("Mesajgonder");
        }
        public ActionResult Mesajlar()
        {
            List<mesaj> ObjCustomer = new List<mesaj>();

            int oid = Convert.ToInt32(Session["oid"]);
            foreach(var e in db.mesaj.ToList())
            {
                if (e.ogrtid == oid)
                {
                    mesaj Obj = new mesaj();
                    Obj.ogradi = e.ogradi;
                    Obj.konu = e.konu;
                    Obj.mesaj1 = e.mesaj1;
                    ObjCustomer.Add(Obj);

                }
            }
            return View(ObjCustomer.ToList());
        }

    }
}