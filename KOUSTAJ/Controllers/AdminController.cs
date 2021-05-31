using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using projeb.Models;
using projeb.Models.Entity;

namespace projeb.Controllers
{
    public class AdminController : Controller
    {
        projebEntities1 db = new projebEntities1();
        // GET: Admin
        public ActionResult Index()
        {
            int a = 0;
            foreach (var y in db.ogrenci)
            {
                a = a + 1;

            }
            int b = 0;
            foreach (var x in db.ogretmen)
            {
                b = b + 1;

            }
            int c = 0;
            foreach (var z in db.sirket)
            {
                c = c + 1;

            }
            int q = 0;
            foreach (var x in db.ilann)
            {
                q = q + 1;

            }
            int p = 0;
            foreach (var z in db.basvuru)
            {
                p = p + 1;

            }
            ViewBag.ilansayisi = q;
            ViewBag.basvurusayisi = p;
            ViewBag.sirketsayisi = c;
            ViewBag.ogrencisayisi = a;
            ViewBag.ogretmensayisi = b;

            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string mail = form["mail"].ToString();
            string sifre = form["sifre"].ToString();
            if (mail == "admin" && sifre=="admin123")
            {
                Session.Add("admin", form["mail"].ToString());
                return RedirectToAction("Index", "Admin");


            }
            else
            {
                ViewBag.Message = "Kullanıcı Adı veya Şifre Hatalı. Lütfen Tekrar deneyin. Denemeniz Başarısız Olmaya Devam Ederse Mail Atabilirsiniz.";
                return View();
            }

        }
        public ActionResult OgrenciEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OgrenciEkle(ogrenci ogr,FormCollection form)
        {
            string mail = form["mail"].ToString();
            var kontrol = db.ogrenci.FirstOrDefault(m => m.mail == mail);
              if (kontrol != null)
              {
                  ViewBag.Mesaj = "Bu Mail Adresiyle Daha Önceden Kayıt Yapmışsınız. Lütfen Yeni Bir Mail Adresi Giriniz.";
                  return View();
              }
              else
              {
                    ogr.adsoyad = form["adsoyad"].ToString();
                    ogr.tckimlik = form["tckimlik"].ToString();
                    ogr.ogrno = form["ogrno"].ToString();
                    ogr.mail = form["mail"].ToString();
                    ogr.sifre = form["sifre"].ToString();
                    ogr.bolum = form["bolum"].ToString();
                    ogr.sinif= form["sinif"].ToString();

                    db.ogrenci.Add(ogr);
                    db.SaveChanges();
                    return View();
              }

        }
        public ActionResult OgrenciListele()
        {
            return View(db.ogrenci.ToList());
        }
        public ActionResult OgrenciDuzenle(int id)
        {
            var std = db.ogrenci.Where(s => s.id == id).FirstOrDefault();
            return View(std);
        }
        [HttpPost]
        public ActionResult OgrenciDuzenle(ogrenci ogr)
        {
            ogrenci updated = (from c in db.ogrenci
                                    where c.id == ogr.id
                                    select c).FirstOrDefault();


            updated.adsoyad = ogr.adsoyad;
            updated.tckimlik = ogr.tckimlik;
            updated.ogrno = ogr.ogrno;
            updated.mail = ogr.mail;
            updated.sifre = ogr.sifre;
            updated.bolum = ogr.bolum;
            updated.sinif = ogr.sinif;
            db.SaveChanges();
            return View();
        }
        public ActionResult OgrenciSil(int id)
        {
            ogrenci sil = (from c in db.ogrenci
                                where c.id == id
                                select c).FirstOrDefault();
            db.ogrenci.Remove(sil);
            db.SaveChanges();
            return RedirectToAction("OgrenciListele");
        }
        public ActionResult OgretmenEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OgretmenEkle(ogretmen ogrt, FormCollection form)
        {
            string mail = form["mail"].ToString();
            var kontrol = db.ogretmen.FirstOrDefault(m => m.mail == mail);
            if (kontrol != null)
            {
                ViewBag.Mesaj = "Bu Mail Adresiyle Daha Önceden Kayıt Yapmışsınız. Lütfen Yeni Bir Mail Adresi Giriniz.";
                return View();
            }
            else
            {
                ogrt.adsoyad = form["adsoyad"].ToString();
                ogrt.tckimlik = form["tckimlik"].ToString();
                ogrt.mail = form["mail"].ToString();
                ogrt.sifre = form["sifre"].ToString();
                ogrt.bolum = form["bolum"].ToString();

                db.ogretmen.Add(ogrt);
                db.SaveChanges();
                return View();
            }

        }
        public ActionResult OgretmenListele()
        {
            return View(db.ogretmen.ToList());
        }
        public ActionResult OgretmenDuzenle(int id)
        {
            var std = db.ogretmen.Where(s => s.id == id).FirstOrDefault();
            return View(std);
        }
        [HttpPost]
        public ActionResult OgretmenDuzenle(ogretmen ogrt)
        {                                                                                   
            ogretmen updated = (from c in db.ogretmen
                               where c.id == ogrt.id
                               select c).FirstOrDefault();


            updated.adsoyad = ogrt.adsoyad;
            updated.tckimlik = ogrt.tckimlik;
            updated.mail = ogrt.mail;
            updated.sifre = ogrt.sifre;
            updated.bolum = ogrt.bolum;
            db.SaveChanges();
            return View();
        }
        public ActionResult OgretmenSil(int id)
        {
            ogretmen sil = (from c in db.ogretmen
                           where c.id == id
                           select c).FirstOrDefault();
            db.ogretmen.Remove(sil);
            db.SaveChanges();
            return RedirectToAction("OgretmenListele");
        }
        public ActionResult Atama()
        {
            List<ogrenci> ObjCustomer = new List<ogrenci>();

                foreach(var ogr in db.ogrenci.ToList())
                {
                    if (Session["ogrtbolum"].ToString() == ogr.bolum)
                    {
                        ogrenci Obj = new ogrenci();
                        Obj.id = ogr.id;
                        Obj.adsoyad = ogr.adsoyad;
                        ObjCustomer.Add(Obj);
                }
            }

            return View(ObjCustomer.ToList());
        }
        [HttpPost]
        public ActionResult Atama(atama a,FormCollection form)
        {
            string[] ids = form["duallistbox_demo1[]"].Split(',');
            foreach(var t in ids)
            {
                string i = t.Trim();
                int x = Convert.ToInt32(i);
                atama ata = (from t1 in db.atama
                                  where t1.ogrid == x
                                  select t1).FirstOrDefault();
                if (ata == null) { 
                a.ogretmenid = Convert.ToInt32(Session["atamaogrtmid"]);
                a.ogrid = x;
                db.atama.Add(a);
                db.SaveChanges();
                }
            }
            ViewBag.m = "Atama İşlemi Başarıyla Sonuçlandı.";
            return RedirectToAction("AtamaOncesi");
        }
        public ActionResult AtamaOncesi()
        {
            return View(db.ogretmen.ToList());
        }
        [HttpPost]
        public ActionResult AtamaOncesi(FormCollection form)
        {
            Session.Add("atamaogrtmid", form["bolum"]);
            int ogrtid = Convert.ToInt32(Session["atamaogrtmid"]);
            foreach (var o in db.ogretmen.ToList())
            {
                if (o.id == ogrtid)
                {
                    Session.Add("ogrtbolum", o.bolum);
                }
            }
            return RedirectToAction("Atama");
        }
        public ActionResult SirketEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SirketEkle(sirket srkt, FormCollection form)
        {
            string maili = form["maili"].ToString();
            var kontrol = db.sirket.FirstOrDefault(m => m.maili == maili);
            if (kontrol != null)
            {
                ViewBag.Mesaj = "Bu Mail Adresiyle Daha Önceden Kayıt Yapmışsınız. Lütfen Yeni Bir Mail Adresi Giriniz.";
                return View();
            }
            else
            {
                srkt.adi= form["adi"].ToString();
                srkt.alani = form["alani"].ToString();
                srkt.maili = form["maili"].ToString();
                srkt.sifre = form["sifre"].ToString();

                db.sirket.Add(srkt);
                db.SaveChanges();
                return View();
            }

        }
        public ActionResult SirketListele()
        {
            return View(db.sirket.ToList());
        }
        public ActionResult SirketDuzenle(int id)
        {
            var std = db.sirket.Where(s => s.id == id).FirstOrDefault();
            return View(std);
        }
        [HttpPost]
        public ActionResult SirketDuzenle(sirket srkt)
        {
            sirket updated = (from c in db.sirket
                                where c.id == srkt.id
                                select c).FirstOrDefault();


            updated.adi = srkt.adi;
            updated.alani = srkt.alani;
            updated.maili = srkt.maili;
            updated.sifre = srkt.sifre;
            db.SaveChanges();
            return View();
        }
        public ActionResult SirketSil(int id)
        {
            sirket sil = (from c in db.sirket
                            where c.id == id
                            select c).FirstOrDefault();
            db.sirket.Remove(sil);
            db.SaveChanges();
            return RedirectToAction("SirketListele");
        }
        public ActionResult AtamaList()
        {

            return View(db.atama.ToList());
        }
        public ActionResult AtamaSil(int id)
        {
            atama sil = (from c in db.atama
                          where c.id == id
                          select c).FirstOrDefault();
            db.atama.Remove(sil);
            db.SaveChanges();
            return RedirectToAction("AtamaList");
        }
    }
}
