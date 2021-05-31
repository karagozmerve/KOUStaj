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
    public class StudentController : Controller
    {
        projebEntities1 db = new projebEntities1();
        // GET: Student
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string mail = form["mail"].ToString();
            string sifre = form["sifre"].ToString();
            var kontrol = db.ogrenci.FirstOrDefault(m => m.mail == mail && m.sifre == sifre);
            if (kontrol != null)
            {
                Session.Add("kmail", form["mail"].ToString());
                foreach(var i in db.ogrenci.ToList())
                {
                    if (i.mail == mail)
                    {
                        Session.Add("kadi", i.adsoyad);
                        Session.Add("kid", i.id);

                    }
                }
                return RedirectToAction("Index", "Student");


            }
            else
            {
                ViewBag.Message = "Mail Adresi veya Şifre Hatalı.Lütfen Tekrar deneyin. Denemeniz Başarısız Olmaya Devam Ederse Mail Atabilirsiniz.";
                return View();
            }

        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(FormCollection form)
        {
            string mail = form["mail"].ToString();
            var model = db.ogrenci.Where(x => x.mail == mail).FirstOrDefault();
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
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.hata = "Sistemde böyle bir mail adresi mevcut değildir.";
                return View();
            }
        }
        public ActionResult Ozgecmis()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Ozgecmis(cv cv, FormCollection form)
        {
            if (Request.Files.Count > 0)
            {
                //Guid nesnesini benzersiz dosya adı oluşturmak için tanımladık ve Replace ile aradaki “-” işaretini atıp yan yana yazma işlemi yaptık.
                string DosyaAdi = Guid.NewGuid().ToString().Replace("-", "");
                //Kaydetceğimiz resmin uzantısını aldık.
                string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string TamYolYeri = "~/Content/ozgecmis/" + DosyaAdi + uzanti;
                //Eklediğimiz Resmi Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                //Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.

                cv.dosyadi= DosyaAdi + uzanti;
                db.cv.Add(cv);
                db.SaveChanges();
                foreach(var i in db.ogrenci.ToList())
                {
                    if (i.mail == Session["kmail"].ToString())
                    {
                        cvogrenci cvogr = new cvogrenci();
                        cvogr.ogrid = i.id;
                        cvogr.cvid = cv.id;
                        db.cvogrenci.Add(cvogr);
                        db.SaveChanges();
                    }
                }


                return View();
            }
     
            return View();
        }
        public ActionResult OzgecmisGoruntule()
        {
            foreach(var goruntule in db.cvogrenci.ToList())
            {
                foreach(var i in db.ogrenci.ToList())
                {
                    if (i.mail == Session["kmail"].ToString())
                    {
                        if (goruntule.ogrid == i.id)
                        {
                            foreach(var c in db.cv.ToList())
                            {
                                if (goruntule.cvid == c.id)
                                {
                                    ViewBag.mesaj = c.dosyadi;
                                    return View();
                                }
                            }
                        }
                    }
                }
            }
            return View();
        }
        public ActionResult CVSil()
        {
            foreach (var i in db.ogrenci.ToList())
            {
                if (i.mail == Session["kmail"].ToString())
                {
                    foreach (var goruntule in db.cvogrenci.ToList())
                    {
                        if (goruntule.ogrid == i.id)
                        {
                            foreach(var c in db.cv.ToList())
                            {
                                if (c.id == goruntule.cvid)
                                {
                                    cv varmi = (from a in db.cv where a.id == c.id select a).FirstOrDefault();
                                    cvogrenci varmis= (from x in db.cvogrenci where x.id == goruntule.id select x).FirstOrDefault();
                                    db.cvogrenci.Remove(varmis);
                                    db.cv.Remove(varmi);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("OzgecmisGoruntule");
        }
        public ActionResult Cikis()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login", "Student");
        }
        public ActionResult IlanlariGoruntule()
        {
       
                    return View(db.ilann.ToList());
        }
        [HttpPost]
        public ActionResult IlanlariGoruntule(string search)
        {
           
          var resultList = db.ilann.Where(t => t.ilanaciklama.Contains(search))
                                                 .ToList();
        

            return View(resultList);
        }
        public ActionResult Basvur(int id)
        {
            int kid = Convert.ToInt32(Session["kid"]);
         basvuru basvur= (from t1 in db.basvuru
                          where t1.ogrenci_id== kid && t1.ilan_id == id
                          select t1).FirstOrDefault();
            if (basvur == null)
            {
                basvuru basvuru = new basvuru();
                basvuru.ogrenci_id = Convert.ToInt32(Session["kid"]);
                basvuru.ilan_id = id;
                db.basvuru.Add(basvuru);
                db.SaveChanges();
            }


            return RedirectToAction("IlanlariGoruntule");
        }
        public ActionResult Hesabim()
        {
            foreach (var e in db.ogrenci.ToList())
            {
                if (e.mail.Trim() == Session["kmail"].ToString())
                {
                    var b = db.ogrenci.Where(s => s.id == e.id).FirstOrDefault();
                    return View(b);
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Hesabim(ogrenci k)
        {
            ogrenci updated = (from c in db.ogrenci
                                    where c.id == k.id
                                    select c).FirstOrDefault();


            updated.adsoyad = k.adsoyad;
            updated.tckimlik = k.tckimlik;
            updated.ogrno = k.ogrno;
            updated.mail = k.mail;
            updated.sifre = k.sifre;
            db.SaveChanges();
            return View();
        }
        public ActionResult KabulFormu()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult KabulFormu(kabulformu kf, FormCollection form)
        {
            if (Request.Files.Count > 0)
            {
                //Guid nesnesini benzersiz dosya adı oluşturmak için tanımladık ve Replace ile aradaki “-” işaretini atıp yan yana yazma işlemi yaptık.
                string DosyaAdi = Guid.NewGuid().ToString().Replace("-", "");
                //Kaydetceğimiz resmin uzantısını aldık.
                string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string TamYolYeri = "~/Content/kabulformu/" + DosyaAdi + uzanti;
                //Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                //Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.

                kf.dosyaadi = DosyaAdi + uzanti;
                db.kabulformu.Add(kf);
                db.SaveChanges();
                foreach (var i in db.ogrenci.ToList())
                {
                    if (i.mail == Session["kmail"].ToString())
                    {
                        kabulformuogrenci kfogr = new kabulformuogrenci();
                        kfogr.ogrid = i.id;
                        kfogr.kfid = kf.id;
                        kfogr.durum = "gonderilmedi";
                        kfogr.onaydurumu = "onaylanmadi";
                        db.kabulformuogrenci.Add(kfogr);
                        db.SaveChanges();
                    }
                }
                return View();
            }
            return View();
        }
        public ActionResult KabulFormuGoruntule()
        {
            foreach (var goruntule in db.kabulformuogrenci.ToList())
            {
                foreach (var i in db.ogrenci.ToList())
                {
                    if (i.mail == Session["kmail"].ToString())
                    {
                        if (goruntule.ogrid == i.id)
                        {
                            foreach (var c in db.kabulformu.ToList())
                            {
                                if (goruntule.kfid == c.id)
                                {
                                    ViewBag.mesaj = c.dosyaadi;
                                    if (goruntule.onaydurumu == "onaylandi") { ViewBag.onay = "Kabul Formunuz Onaylanmıştır.";  }

                                    return View();
                                }
                            }
                        }
                    }
                }
            }
            return View();
        }
        public ActionResult KFSil()
        {
            foreach (var i in db.ogrenci.ToList())
            {
                if (i.mail == Session["kmail"].ToString())
                {
                    foreach (var goruntule in db.kabulformuogrenci.ToList())
                    {
                        if (goruntule.ogrid == i.id)
                        {
                            foreach (var c in db.kabulformu.ToList())
                            {
                                if (c.id == goruntule.kfid)
                                {
                                    kabulformu varmi = (from a in db.kabulformu where a.id == c.id select a).FirstOrDefault();
                                    kabulformuogrenci varmis = (from x in db.kabulformuogrenci where x.id == goruntule.id select x).FirstOrDefault();
                                    db.kabulformuogrenci.Remove(varmis);
                                    db.kabulformu.Remove(varmi);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("KabulFormuGoruntule");
        }
        public ActionResult KFGonder()
        {

            foreach (var i in db.ogrenci.ToList())
            {
                if (i.mail == Session["kmail"].ToString())
                {
                    foreach (var goruntule in db.kabulformuogrenci.ToList())
                    {
                        if (goruntule.ogrid == i.id)
                        {
                            
                            kabulformuogrenci varmis = (from x in db.kabulformuogrenci where x.id == goruntule.id select x).FirstOrDefault();
                            varmis.durum = "gonderildi";
                            db.SaveChanges();
                               
                        }
                    }
                }
            }
            return RedirectToAction("KabulFormuGoruntule");

        }
        public ActionResult StajDefterim()
        {
           
            foreach (var goruntule in db.stajdefogrenci.ToList())
            {
                foreach (var i in db.ogrenci.ToList())
                {
                    if (i.mail == Session["kmail"].ToString())
                    {
                        if (goruntule.ogrid == i.id)
                        {
                            foreach (var c in db.stajdef.ToList())
                            {
                                if (goruntule.stajdefid == c.id)
                                {
                                    ViewBag.mesaj = c.dosyaadi;
                                    if (goruntule.onaydurumu != null) { ViewBag.not = "Notunuz : " + goruntule.onaydurumu; }

                                    return View();
                                }
                            }
                        }
                    }
                }
            }
            return View();

        }
        public ActionResult StajDefteriYukle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StajDefteriYukle(stajdef kf, FormCollection form)
        {
            if (Request.Files.Count > 0)
            {
                //Guid nesnesini benzersiz dosya adı oluşturmak için tanımladık ve Replace ile aradaki “-” işaretini atıp yan yana yazma işlemi yaptık.
                string DosyaAdi = Guid.NewGuid().ToString().Replace("-", "");
                //Kaydetceğimiz resmin uzantısını aldık.
                string uzanti = System.IO.Path.GetExtension(Request.Files[0].FileName);
                string TamYolYeri = "~/Content/stajdef/" + DosyaAdi + uzanti;
                //Eklediğimiz Resni Server.MapPath methodu ile Dosya Adıyla birlikte kaydettik.
                Request.Files[0].SaveAs(Server.MapPath(TamYolYeri));
                //Ve veritabanına kayıt için dosya adımızı değişkene aktarıyoruz.

                kf.dosyaadi = DosyaAdi + uzanti;
                db.stajdef.Add(kf);
                db.SaveChanges();
                foreach (var i in db.ogrenci.ToList())
                {
                    if (i.mail == Session["kmail"].ToString())
                    {
                        stajdefogrenci kfogr = new stajdefogrenci();
                        kfogr.ogrid = i.id;
                        kfogr.stajdefid = kf.id;
                        kfogr.durum = "gonderilmedi";
                        kfogr.onaydurumu = "onaylanmadi";
                        db.stajdefogrenci.Add(kfogr);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("StajDefterim");
            }
            return View();
        }
        public ActionResult StajDefSil()
        {
            foreach (var i in db.ogrenci.ToList())
            {
                if (i.mail == Session["kmail"].ToString())
                {
                    foreach (var goruntule in db.stajdefogrenci.ToList())
                    {
                        if (goruntule.ogrid == i.id)
                        {
                            foreach (var c in db.stajdef.ToList())
                            {
                                if (c.id == goruntule.stajdefid)
                                {
                                    stajdef varmi = (from a in db.stajdef where a.id == c.id select a).FirstOrDefault();
                                    stajdefogrenci varmis = (from x in db.stajdefogrenci where x.id == goruntule.id select x).FirstOrDefault();
                                    db.stajdefogrenci.Remove(varmis);
                                    db.stajdef.Remove(varmi);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("StajDefterim");
        }
        public ActionResult SDGonder()
        {

            foreach (var i in db.ogrenci.ToList())
            {
                if (i.mail == Session["kmail"].ToString())
                {
                    foreach (var goruntule in db.stajdefogrenci.ToList())
                    {
                        if (goruntule.ogrid == i.id)
                        {

                            stajdefogrenci varmis = (from x in db.stajdefogrenci where x.id == goruntule.id select x).FirstOrDefault();
                            varmis.durum = "gonderildi";
                            db.SaveChanges();

                        }
                    }
                }
            }
            return RedirectToAction("StajDefterim");

        }
        public ActionResult Basvurularim()
        {
            List<ilann> ObjCustomer = new List<ilann>();

            foreach (var k in db.ogrenci.ToList())
            {
                if (k.mail == Session["kmail"].ToString())
                {
                    foreach (var e in db.basvuru.ToList())
                    {
                        if (k.id == e.ogrenci_id)
                        {
                            foreach(var c in db.ilann.ToList())
                            {
                                if (e.ilan_id == c.id)
                                {
                                    ilann Obj = new ilann();
                                    Obj.id = c.id;
                                    Obj.sirketadi = c.sirketadi.ToString();
                                    Obj.ilanaciklama = c.ilanaciklama.ToString();
                                
                                    ObjCustomer.Add(Obj);
                                }
                            }
                        }
                    }
                }
            }
            if (ObjCustomer.ToList().Count() != 0)
            {
                return View(ObjCustomer.ToList());
            }
            else
            {
                ViewBag.Message = "Kullanıcı Hesabınıza Ait Başvuru Bulunamadı.";
                return View();
            }

        }
        public ActionResult Mesajlarim()
        {
            int id = Convert.ToInt32(Session["kid"]);
            List<mesaj> ObjCustomer = new List<mesaj>();
            foreach (var i in db.mesaj.ToList())
            {
                if (i.ogrid == id)
                {
                    mesaj Obj = new mesaj();
                    Obj.ogrtadi = i.ogrtadi;
                    Obj.konu = i.konu;
                    Obj.mesaj1 = i.mesaj1;
                    ObjCustomer.Add(Obj);

                }
            }
            return View(ObjCustomer.ToList());
        }

    }
}