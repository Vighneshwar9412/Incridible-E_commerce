using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wanc.Models;
using System.Security.Cryptography;

namespace Wanc.Controllers
{
    public class homeController : Controller
    {

        // GET: /home/
        wanc_dbEntities db = new wanc_dbEntities();
        public ActionResult Index()
        {
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();
            return View();
        }

        public ActionResult contact()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();
            return View();
        }

        public ActionResult about()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();
            return View();
        }

        public ActionResult Location()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();
            return View();
        }


        public ActionResult login()
        {
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult login(string eamil, string password)
        {
            tbl_registration user = db.tbl_registration.Where(a => a.eamil == eamil.Trim() && a.password == password.Trim()).FirstOrDefault();
            if (user != null)
            {
                Session["name"] = user.name;
                Session["eamil"] = user.eamil;
                Session.Timeout = 10;
                return RedirectToAction("Index");
            }
            TempData["msg"] = "invalid email or password";
            return RedirectToAction("login");
        }
        public ActionResult logout()
        {
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();
            Session["name"] = null;
            Session["eamil"] = null;
            return RedirectToAction("Index");

        }
        public ActionResult registration()
        {
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult registration(tbl_registration model, string pass)
        {

            tbl_registration obj = new tbl_registration();
            obj.name = model.name;
            obj.eamil = model.eamil;
            obj.password = model.password;
            obj.mobile1 = model.mobile1;
            obj.mobile2 = model.mobile2;
            obj.date = DateTime.Now;
            db.tbl_registration.Add(obj);
            db.SaveChanges();
            TempData["msg"] = "registration success";
            return RedirectToAction("registration");
        }

        public ActionResult Editprofile(int id)
        {
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();

            var abc = db.tbl_registration.Where(a => a.id == id).FirstOrDefault();
            return View(abc);
        }

        [HttpPost]

        public ActionResult Editprofile(tbl_registration Add)
        {
            var abc = db.tbl_registration.Where(a => a.id == Add.id).FirstOrDefault();

            abc.name = Add.name;
            //abc.eamil = Add.eamil;
            db.Entry<tbl_registration>(abc).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyProfile");
        }
        public ActionResult EditContact(int id)
        {
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();
            var abcd = db.tbl_registration.Where(a => a.id == id).FirstOrDefault();
            return View(abcd);
        }

        [HttpPost]
        public ActionResult EditContact(tbl_registration Add)
        {
            var abcd = db.tbl_registration.Where(a => a.id == Add.id).FirstOrDefault();

            abcd.mobile1 = Add.mobile1;
            abcd.mobile2 = Add.mobile2;
            db.Entry<tbl_registration>(abcd).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("MyProfile");
        }

        public ActionResult DeleteContact(int id)
        {
            var userdata = db.tbl_registration.Where(a => a.id == id).FirstOrDefault();
            db.tbl_registration.Remove(userdata);
            db.SaveChanges();
            return RedirectToAction("MyProfile");
        }

        #region Change_password
        public ActionResult Change_password()
        {
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();

            if (Session["name"] != null)
            {
                ViewBag.selid = Session["UserID"];
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult Change_password(tbl_registration obj, string npass, string cpass)
        {
            if (Session["name"] != null)
            {
                string name = Session["name"].ToString();
                var data = db.tbl_registration.Where(x => x.name == name).FirstOrDefault();
                if (data.password.Equals(obj.password) == true && npass.Equals(cpass) == true)
                {
                    data.password = cpass;
                    db.Entry<tbl_registration>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "Your Password Has Been Changed";
                    return View();
                }

                TempData["msg"] = "Your Password Is Not Matching";
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        #endregion

        #region GET PRODUCT BY SUB CATEGORY
        public ActionResult GetProductBySubCategory(int id)
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            List<ProductDto> obj2 = new List<ProductDto>();
            var prodata2 = db.Products.Where(a => a.subcatid == id && a.active == true).ToList();
            foreach (var item in prodata2)
            {
                ProductDto add2 = new ProductDto();
                add2.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                add2.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                add2.cashback = item.cashback;
                add2.Description = item.Description;
                add2.Cashbacktype = item.Cashbacktype;
                add2.Discount = item.Discount;
                add2.Unit = item.Unit;
                add2.ID = item.ID;
                add2.subcatid = item.subcatid;
                add2.Price = item.Price;
                add2.Name = item.Name;
                add2.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                add2.Image = db.Products.Where(a => a.ID == item.ID).FirstOrDefault().Image;
                obj2.Add(add2);

            }
            return View(obj2);
        }
        #endregion

        #region AtoZsubcatproduct
        public ActionResult AtoZsubcatproduct(int Id)
        {
            ViewBag.subid = Id;
            ViewBag.catname = Id;
            //return View();
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            List<ProductDto> obj2 = new List<ProductDto>();
            var prodata2 = db.Products.Where(a => a.subcatid == Id && a.active == true).ToList();
            foreach (var item in prodata2)
            {
                ProductDto add2 = new ProductDto();
                add2.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                add2.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                add2.cashback = item.cashback;
                add2.Description = item.Description;
                add2.Cashbacktype = item.Cashbacktype;
                add2.Discount = item.Discount;
                add2.Unit = item.Unit;
                add2.ID = item.ID;
                add2.subcatid = item.subcatid;
                add2.Price = item.Price;
                add2.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                add2.Image = db.ProductImages.Where(a => a.ProductId == item.ID).FirstOrDefault().Images;
                obj2.Add(add2);

            }
            return View(obj2);
        }

        #endregion

        #region SellerRegistrations
        public ActionResult SellerRegistrations()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();

            return View();
        }
        private Random randomnumber = new Random();
        public string GenerateNumber()
        {
            return randomnumber.Next(0, 99999).ToString("D5");
        }

        [HttpPost]
        public ActionResult SellerRegistrations(sallerDetail obj, HttpPostedFileBase File)
        {

            var img = FileUploader.UploadImage(File, "Images");
            if (img != null)
            {
                obj.Salerusername = "wanc" + GenerateNumber() + "Seller";
                obj.sallerpass = GenerateNumber();
                obj.Document = img;
                obj.createdate = DateTime.Now;
                obj.Active = false;
                db.sallerDetails.Add(obj);
                db.SaveChanges();
                return RedirectToAction("Welcomeseller", "Home", new { id = obj.Salerusername, pass = obj.sallerpass });
            }
            else
            {
                return View();
            }

        }
        #endregion

        #region welcomeletter
        public ActionResult Welcomeseller(string id, string pass)
        {
            ViewBag.category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();

            ViewBag.id = id;
            ViewBag.pass = pass;
            return View();
        }

        #endregion

        public ActionResult Viewallbrand()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();
            return View();
        }

        public ActionResult shop()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();
            return View();
        }

        public ActionResult products()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();
            return View();
        }

        public ActionResult checkout()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();

            if (Session["eamil"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            string userid = Convert.ToString(Session["eamil"]);
            var data1 = db.tbl_registration.Where(a => a.eamil == userid).FirstOrDefault();
            return View(data1);
        }
        [HttpPost]
        public ActionResult Checkout(tbl_Order objj1, decimal Amount, string Pincode)
        {
            if (Session["eamil"] == null)
            {
                TempData["msg"] = "Please Login First";
                return RedirectToAction("Index");
            }
            var data = db.tbl_pincode.Where(z=>z.Pincode == Pincode).ToList();
            if(data.Count>0)
            {
                Random Random = new Random();
                string userdata1 = Session["eamil"].ToString();
                var userdata = db.tbl_registration.Where(a => a.eamil == userdata1).FirstOrDefault();
                var kart = db.KartDetails.Where(a => a.uniqueID == userdata1).ToList();
                string ORDERID_ = "ORDER" + Random.Next(99999, 10000000);
                tbl_Order obj1 = new tbl_Order();
                obj1.CustomerUniqueID = userdata1;
                obj1.CustomerName = userdata.name;
                obj1.CustomerNumber = userdata.mobile1;
                obj1.ShippingAddress = objj1.ShippingAddress;
                obj1.Createdate = DateTime.Now;
                obj1.DeliveryStatus = "Pending";
                obj1.InvoiceNumber = ORDERID_;
                obj1.OrderId = ORDERID_;
                obj1.SellerUniqueID = "Admin";
                obj1.TotalOrderAmount = Convert.ToInt32(Amount);
                obj1.PaymentMode = objj1.PaymentMode;
                db.tbl_Order.Add(obj1);
                db.SaveChanges();

                if (obj1 != null)
                {
                    foreach (var item in kart)
                    {
                        var Product = db.Products.Where(a => a.ID == item.ProductId).FirstOrDefault();
                        OrderDetail add = new OrderDetail();
                        add.OrderId = obj1.ID;
                        add.ModifiedDate = DateTime.Now;
                        add.CreateDate = DateTime.Now;
                        add.ListedPrice = Convert.ToInt32(Product.MRP);
                        add.SellingPrice = Convert.ToInt32(Product.Price);
                        add.PaymentMode = objj1.PaymentMode;
                        add.GST = Product.GST;
                        add.ProductId = item.ProductId;
                        add.ProductName = Product.Name;
                        add.ProductImage = Product.Image;
                        add.DeliveryStatus = "Pending";
                        add.Quantity = (int)item.Quantity;
                        add.sallerid = Product.SelleroradminId;
                        db.OrderDetails.Add(add);
                        db.KartDetails.Remove(item);
                        db.SaveChanges();
                    }
                    string userpass = userdata.name + ", Thank You For Shopping With Us, Your OrderID is <b>" + obj1.InvoiceNumber + "</b>";
                    Session["ShoppingMessage"] = userpass;
                }
                return RedirectToAction("OrderSucessPage");
            }
            else
            {
                TempData["Message"] = "Service not available in this area";
                return View(db.tbl_registration.FirstOrDefault());
            }
            
            
        }
        public ActionResult OrderSucessPage()
        {
            return View();
        }
        public ActionResult ProductPage(int ProductID)
        {
            return View(db.Products.Where(a => a.ID == ProductID).FirstOrDefault());
        }

        public ActionResult CartDetailBydatabes()
        {
            if (Session["eamil"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            string UserSelfID = Session["eamil"].ToString();
            var kart = db.KartDetails.Where(a => a.uniqueID == UserSelfID).ToList();
            var ProductList = db.Products.ToList();
            List<KartDetailDto> Kdto = new List<KartDetailDto>();
            Session["KartCount"] = kart.Count();
            return View(kart);
        }

        #region ADD TO SESSION CART | AddToKart | KartPlus| KartMinus | Cart Detail By databes | Cart Detail By Session

        //Add To Kart
        public ActionResult AddToKart(int ProductId, int Quantity)
        {
            if (Session["eamil"] != null)
            {
                string useruniquID = Session["eamil"].ToString();

                var product = db.Products.Find(ProductId);

                if (db.KartDetails.Any(a => a.ProductId == ProductId && a.uniqueID == useruniquID))
                {
                    db.KartDetails.Where(a => a.ProductId == ProductId && a.uniqueID == useruniquID).FirstOrDefault().Quantity++;
                    db.SaveChanges();
                }
                else
                {
                    KartDetail kd = new KartDetail();

                    kd.ProductId = product.ID;
                    kd.uniqueID = useruniquID;
                    kd.Quantity = Quantity;
                    kd.Price = (int)product.Price;

                    db.KartDetails.Add(kd);
                    db.SaveChanges();
                    Session["KartCount"] = db.KartDetails.Where(a => a.uniqueID == useruniquID && a.ProductId == ProductId).ToList().Count();
                    ViewBag.kart = db.KartDetails.Where(a => a.uniqueID == useruniquID && a.ProductId == ProductId).ToList();
                    return RedirectToAction("ProductPage", new { ProductId = ProductId });
                }
                return RedirectToAction("ProductPage", new { ProductId = ProductId });
            }
            else
            {
                return RedirectToAction("ProductPage", new { ProductId = ProductId });
            }
        }
        public JsonResult KartPlus(int ID)
        {
            if (Session["eamil"] != null)
            {
                //var clientid = Convert.ToInt32(Session["eamil"]);
                var kartdata = db.KartDetails.Where(a => a.ID == ID).FirstOrDefault();
                kartdata.Quantity = Convert.ToInt32(kartdata.Quantity + 1);
                db.Entry<KartDetail>(kartdata).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                Session["KartCount"] = db.KartDetails.Where(a => a.ID == ID).ToList().Count();
                ViewBag.kart = db.KartDetails.Where(a => a.ID == ID).ToList();
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
            return Json("Notok", JsonRequestBehavior.AllowGet);
        }
        public JsonResult KartMinus(int ID)
        {
            if (Session["eamil"] != null)
            {
                var kartdata = db.KartDetails.Where(a => a.ID == ID).FirstOrDefault();
                kartdata.Quantity = Convert.ToInt32(kartdata.Quantity - 1);
                if (kartdata.Quantity == 0)
                {
                    db.KartDetails.Remove(kartdata);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry<KartDetail>(kartdata).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                }
                Session["KartCount"] = db.KartDetails.Where(a => a.ID == ID).ToList().Count();
                ViewBag.kart = db.KartDetails.Where(a => a.ID == ID).ToList();

            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        public JsonResult deletecart(int ID)
        {
            if (Session["eamil"] != null)
            {
                var kartdata = db.KartDetails.Where(a => a.ID == ID).FirstOrDefault();
                db.KartDetails.Remove(kartdata);
                db.SaveChanges();
                Session["KartCount"] = db.KartDetails.Where(a => a.ID == ID).ToList().Count();
                ViewBag.kart = db.KartDetails.Where(a => a.ID == ID).ToList();
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }


        public ActionResult CartDetailsBySession()
        {
            if (Session["Kart"] != null)
            {
                return RedirectToAction("Index", "Home");
                List<KartDetailDto> KartList = (List<KartDetailDto>)Session["Kart"];
                foreach (var item in KartList)
                {
                    item.Productname = db.ProductImages.Where(a => a.ProductId == (int)item.ProductId).FirstOrDefault().Images[0].ToString();
                    if (item.Productname != null)
                    {
                        item.Productname = db.Products.Find((int)item.ProductId).Name;
                    }
                    else
                    {
                        item.Productname = "No Product Name";

                    }
                }
                return View(KartList);
            }
            else
            {
                return View();
            }
        }
        #endregion

        #region Get Products By Seller
        public ActionResult GetProductBySeller(int SellerId)
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            List<ProductDto> obj = new List<ProductDto>();

            var Prodata = db.Products.Where(a => a.SelleroradminId == SellerId && a.active == true).ToList();
            foreach (var item in Prodata)
            {
                ProductDto Add = new ProductDto();
                Add.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                Add.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                Add.Description = item.Description;
                Add.Discount = item.Discount;
                Add.Unit = item.Unit;
                Add.ID = item.ID;
                Add.Name = item.Name;
                Add.Price = item.Price;
                Add.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                Add.Image = db.ProductImages.Where(a => a.ProductId == Add.ID).FirstOrDefault().Images;
                obj.Add(Add);
            }

            return View(obj);
        }
        #endregion

        public int GenerateOtp()
        {
            Random rand = new Random();
            foreach (var item in db.OTPs.ToList())
            {
                if (item.Time.Value.AddMinutes(20) > DateTime.Now)
                {
                    db.OTPs.Remove(item);
                }
            }
            db.SaveChanges();
            var OTPList = db.OTPs.ToList();
            var SuggesntedOTP = rand.Next(000001, 999999);
            if (OTPList.Any(a => a.Otp1 == SuggesntedOTP))
            {
                GenerateOtp();
            }
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            return SuggesntedOTP;
        }
        public ActionResult Successfullreg()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            return View();
        }

        #region search
        public ActionResult Search(string search)
        {
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();

            List<ProductDto> obj = new List<ProductDto>();
            var Prodata = db.Products.Where(a => a.Name.Contains(search) && a.active == true).ToList();
            return View(Prodata);
        }
        #endregion

        #region Showallproduct

        public ActionResult Showallproduct()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            ViewBag.pincode = db.tbl_pincode.ToList();
            return View();
        }

        #endregion


        #region myprofile

        public ActionResult MyProfile()
        {
            if (Session["eamil"] == null)
            {
                RedirectToAction("Index");
            }
            ViewBag.categorylist = db.Categories.ToList();
            ViewBag.itemdata = db.subcategories.ToList();
            ViewBag.pro = db.Products.ToList();
            string Email = Session["eamil"].ToString();
            var UserData = db.tbl_registration.Where(a => a.eamil == Email).FirstOrDefault();
            return View(UserData);
        }

        #endregion


        #region TrackOrder

        public ActionResult SearchPin()
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();
            return View();
        }
        public ActionResult TrackOrder(string OrderId)
        {
            ViewBag.Category = db.Categories.ToList();
            ViewBag.subcategory = db.subcategories.ToList();

            List<OrderDetail> obj = new List<OrderDetail>();
            var Prodata = db.tbl_Order.Where(a => a.OrderId == OrderId).ToList().Take(1);
            foreach (var item in Prodata)
            {
                var data = db.OrderDetails.Where(a => a.OrderId == item.ID).ToList();
                obj.AddRange(data);
            }
            return View(obj);
        }
        #endregion

        public ActionResult OrderHistory()
        {
            return View();
        }
        public ActionResult ShippingPolicy()
        {
            return View();
        }

        public ActionResult CancellationRefund()
        {
            return View();
        }
        public ActionResult TermandCondition()
        {
            return View();
        }
        //public ActionResult CancelOrder(int id)
        //{
        //    if (Session["UserIDp"] == null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    int C_id = Convert.ToInt32(Session["UserIDp"]);
        //    var data = db.OrderDetails.Where(x => x.ID == id).FirstOrDefault();
        //    if (data != null)
        //    {
        //        data.IsCancelled = true;
        //        data.RefundReqDate = DateTime.Now;
        //        db.Entry<OrderDetail>(data).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["msg"] = "<script>alert('Order Has Been Cancelled')</script>";
        //        return RedirectToAction("My_Order", "Home", new { myid = C_id });
        //    }
        //    return RedirectToAction("My_Order", "Home");
        //}

    }
}
