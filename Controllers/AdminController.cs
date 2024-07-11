using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Wanc.Models;
using Wanc;

namespace Wanc.Controllers
{
    public class AdminController : Controller
    {

        wanc_dbEntities db = new wanc_dbEntities();

        #region Dashboard / Index/ Logout
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string logingid, string pwd)
        {

            Admin obj = db.Admins.Where(x => x.Username == logingid && x.Password == pwd).SingleOrDefault();
            if (obj != null)
            {
                Session["adminid"] = obj.ID;
                Session.Timeout = 10;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.mode = "Invalid Email Id Or Password";
                return View();
            }

        }

        public ActionResult Dashboard()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Logout()
        {
            if (Session["adminid"] != null)
            {
                Session["adminid"] = null;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult ChangePassword()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldpassword, string Password, string Confirm)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            if (oldpassword != db.Admins.FirstOrDefault().Password)
            {
                TempData["MSG"] = "old Password Do not Match!!!";
                return RedirectToAction("ChangePassword");
            }
            if (Password != Confirm)
            {
                TempData["MSG"] = "Passwords Do not Match or password length is too small..!!Minimum Length is 8.";
                return RedirectToAction("ChangePassword");
            }
            db.Admins.FirstOrDefault().Password = Password;
            db.SaveChanges();
            TempData["MSG"] = "Successfully updated Your Password . Please Login With Your UserId And New Password...";
            return RedirectToAction("Index", "Admin");
        }

        #endregion

        #region  CreatePurchase
        public ActionResult CreatePurchase()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreatePurchase(PurchaseDetail obj)
        {
            obj.UpdateBy = "Admin";
            obj.PurDate = DateTime.Now;
            db.PurchaseDetails.Add(obj);
            db.SaveChanges();
            TempData["MGS1"] = "Added Successfully";
            return RedirectToAction("CreatePurchase", "Admin");
        }
        public ActionResult PurchaseProductList()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            return View(db.PurchaseDetails.OrderByDescending(a => a.ID).ToList());
        }

        public ActionResult Inventory()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }

            List<ProductDto> obj = new List<ProductDto>();
            var Prodata = db.Products.OrderByDescending(a => a.ID).ToList();
            foreach (var item in Prodata)
            {
                ProductDto Add = new ProductDto();
                if (db.Categories.Any(a => a.ID == item.categoryId))
                {
                    Add.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                }
                if (db.subcategories.Any(a => a.id == item.subcatid))
                {
                    Add.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                }
                if (db.Brands.Any(a => a.ID == item.BrandID))
                {
                    Add.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                }
                Add.cashback = item.cashback;
                Add.Description = item.Description;
                Add.Discount = item.Discount;
                Add.Unit = item.Unit;
                Add.UpdateBy = item.UpdateBy;
                Add.ID = item.ID;
                Add.Name = item.Name;
                Add.Price = item.Price;
                if (db.ProductImages.Any(a => a.ProductId == Add.ID))
                {
                    Add.Image = db.ProductImages.Where(a => a.ProductId == Add.ID).FirstOrDefault().Images;
                }
                obj.Add(Add);
            }

            return View(obj);
        }
        #endregion

        #region  banner

        public ActionResult AddBanner()
        {
            return View();
        }

        [HttpPost]

        public ActionResult AddBanner(BannerDto obj, HttpPostedFileBase file)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            Banner add = new Banner()
            {
                //Images = FileUploader.UploadImage(obj.ImagesFile, "IndexBanners")
                Images = file != null ? "/IndexBanners/" + FileUploader.UploadImage(file, "IndexBanners") : null

            };

            //if (obj.ImagesFile.ContentLength > 1 * 1230 * 425)
            //{
            //    TempData["msg"] = "Banner Size Is InValid";
            //    return RedirectToAction("AddBanner");
            //}
            //else
            //{
            add.URL = obj.Url;
            db.Banners.Add(add);
            db.SaveChanges();
            TempData["msg"] = "Banner is Uploaded";
            return RedirectToAction("AddBanner");
            //}
        }

        public ActionResult ViewBanner()
        {
            var data = db.Banners.OrderByDescending(a => a.ID).ToList();
            ViewBag.Data = data;
            return View();
        }

        public ActionResult DelBanner(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }

            db.Banners.Remove(db.Banners.Find(Id));
            db.SaveChanges();
            return RedirectToAction("ViewBanner");
        }

        public ActionResult EditBanner(int Id)
        {
            ViewBag.banner = db.Banners.ToList();
            var userdata = db.Banners.Where(a => a.ID == Id).FirstOrDefault();
            Banner add = new Banner()
            {
                ID = userdata.ID,
                Images = userdata.Images,
                URL = userdata.URL,
            };

            return View(add);
        }
        [HttpPost]
        public ActionResult EditBanner(Banner obj, HttpPostedFileBase file)
        {
            Banner add = new Banner();
            add.ID = obj.ID;
            if (obj.Images != null)
            {
                add.Images = obj.Images;
            }
            if (file != null)
            {
                //add.Images = FileUploader.UploadImage(file, "IndexBanners");
                add.Images = file != null ? "/IndexBanners/" + FileUploader.UploadImage(file, "IndexBanners") : null;
            }
            add.URL = obj.URL;
            db.Entry<Banner>(add).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewBanner", "Admin");
        }

        public ActionResult AddNewBrand()
        {
            ViewBag.pin = db.tbl_pincode.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddNewBrand(BrandDto obj, HttpPostedFileBase file)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            if (db.Brands.Any(a => a.BrandName == obj.BrandName))
            {
                TempData["msg"] = "Already Exists";
                return RedirectToAction("AddNewBrand");
            }
            Brand add = new Brand()
            {
                UpdateBy = "Admin",
                BrandName = obj.BrandName,
                //Images = FileUploader.UploadImage(obj.ImagesFile, "BrandImage")
                Images = file != null ? "/BrandImage/" + FileUploader.UploadImage(file, "BrandImage") : null,
            };
            db.Brands.Add(add);
            db.SaveChanges();
            TempData["msg"] = "Brand is Uploaded";
            return RedirectToAction("AddNewBrand");
        }
        public JsonResult Addbrands(string BRAND)
        {
            if (db.Brands.Any(a => a.BrandName == BRAND))
            {
                return Json("NOTOK", JsonRequestBehavior.AllowGet);
            }
            Brand brand = new Brand();
            brand.UpdateBy = "Admin";
            brand.BrandName = BRAND;
            db.Brands.Add(brand);
            db.SaveChanges();
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DelBrand(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            if (db.Products.Any(a => a.BrandID == Id))
            {
                var ProductData = db.Products.Where(a => a.BrandID == Id).ToList();
                foreach (var item in ProductData)
                {
                    db.Products.Remove(db.Products.Find(item.ID));
                    db.SaveChanges();
                }
            }
            db.Brands.Remove(db.Brands.Find(Id));
            db.SaveChanges();
            return RedirectToAction("ViewBrand");
        }

        public ActionResult ViewBrand()
        {
            var data = db.Brands.OrderByDescending(a => a.ID).ToList();
            ViewBag.data = data;
            return View(data);
        }

        #endregion


        #region Category

        public ActionResult AddNewCategory()
        {
            ViewBag.pin = db.tbl_pincode.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult AddNewCategory(CategoryDto obj, HttpPostedFileBase file)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            if (db.Categories.Any(a => a.Name == obj.Name))
            {
                TempData["msg"] = "Already Exists";
                return RedirectToAction("AddNewCategory");
            }
            Category add = new Category()
            {
                UpdateBy = "Admin",
                Description = obj.Description,
                Name = obj.Name,
                Image = file != null ? "/CategoryImage/" + FileUploader.UploadImage(file, "CategoryImage") : null,
                SelleroradminId = 0,
                createDate = DateTime.Now,
            };
            db.Categories.Add(add);
            db.SaveChanges();
            TempData["msg"] = "Category is Added";
            return RedirectToAction("AddNewCategory");
        }
        public ActionResult ViewAllCategory()
        {
            var data = db.Categories.OrderByDescending(a => a.ID).ToList();
            ViewBag.data = data;
            return View();
        }
        public ActionResult DelCategory(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            if (db.subcategories.Any(a => a.catid == Id))
            {
                var SubCategoryData = db.subcategories.Where(a => a.catid == Id).ToList();
                foreach (var item in SubCategoryData)
                {
                    db.subcategories.Remove(db.subcategories.Find(item.id));
                    db.SaveChanges();
                }
            }

            db.Categories.Remove(db.Categories.Find(Id));
            db.SaveChanges();
            return RedirectToAction("ViewAllCategory");
        }
        public ActionResult Editcategory(int Id)
        {
            var catdata = db.Categories.Where(a => a.ID == Id).FirstOrDefault();
            CategoryDto add = new CategoryDto()
            {
                Image = catdata.Image,
                ID = catdata.ID,
                UpdateBy = catdata.UpdateBy,
                Description = catdata.Description,
                Name = catdata.Name,
                Parentid = catdata.Parentid
            };

            return View(add);
        }

        [HttpPost]
        public ActionResult Editcategory(CategoryDto obj, HttpPostedFileBase file)
        {
            Category add = new Category();
            add.Description = obj.Description;
            add.Name = obj.Name;
            add.UpdateBy = obj.UpdateBy;
            add.ID = obj.ID;
            add.Parentid = obj.Parentid;
            add.Image = file != null ? "/CategoryImage/" + FileUploader.UploadImage(file, "CategoryImage") : obj.Image;
            db.Entry<Category>(add).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewAllCategory", "Admin");
        }

        #endregion

        #region SubCategory

        public ActionResult AddSubCategory()
        {
            ViewBag.pin = db.tbl_pincode.ToList();
            ViewBag.Cate = db.Categories.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddSubCategory(subcategory obj)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            if (db.subcategories.Any(a => a.subcategorname == obj.subcategorname))
            {
                TempData["msg"] = "Already Exists";
                return RedirectToAction("AddSubCategory");
            }
            obj.UpdateBy = "Admin";
            db.subcategories.Add(obj);
            db.SaveChanges();
            TempData["msg"] = "Subcategory is Added";
            return RedirectToAction("AddSubCategory");
        }

        public ActionResult ViewSubCategory()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            List<SubcategoryDto> obj = new List<SubcategoryDto>();
            var subcatdata = db.subcategories.OrderByDescending(a => a.id).ToList();
            foreach (var item in subcatdata)
            {
                SubcategoryDto Add = new SubcategoryDto();
                Add.catid = item.catid;
                Add.id = item.id;
                Add.UpdateBy = item.UpdateBy;
                Add.subcategorname = item.subcategorname;
                if (db.Categories.Any(a => a.ID == item.catid))
                {
                    Add.ParentCategoryName = db.Categories.Where(a => a.ID == item.catid).FirstOrDefault().Name.ToString();
                }
                obj.Add(Add);
            }
            ViewBag.data = obj;
            return View();
        }

        public ActionResult Deletesubcategory(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            db.subcategories.Remove(db.subcategories.Find(Id));
            db.SaveChanges();
            return RedirectToAction("ViewSubCategory");
        }

        public ActionResult EditSubcategory(int Id)
        {
            ViewBag.Cate = db.Categories.ToList();
            var userdata = db.subcategories.Where(a => a.id == Id).FirstOrDefault();
            SubcategoryDto add = new SubcategoryDto()
            {
                catid = userdata.catid,
                id = userdata.id,
                subcategorname = userdata.subcategorname,
                ParentCategoryName = userdata.subcategorname,
                UpdateBy = userdata.UpdateBy
            };

            return View(add);
        }

        [HttpPost]
        public ActionResult EditSubcategory(SubcategoryDto obj)
        {
            subcategory add = new subcategory()
            {
                id = obj.id,
                catid = obj.catid,
                subcategorname = obj.subcategorname,
                UpdateBy = obj.UpdateBy
            };
            db.Entry<subcategory>(add).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewSubCategory", "Admin");

        }

        #endregion

        #region Product
        public ActionResult AddnewProduct()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.pin = db.tbl_pincode.ToList();
            ViewBag.Brand = db.Brands.ToList();
            ViewBag.Cate = db.Categories.ToList();
            ViewBag.subcate = db.subcategories.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult AddnewProduct(ProductDto obj, HttpPostedFileBase file)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }

            StringBuilder pincodee = new StringBuilder();
            for (int i = 0; i < obj.Pincode.Length; i++)
            {
                pincodee.Append("," + obj.Pincode[i]);
            }



            var getbrandname = db.Brands.Where(a => a.ID == obj.BrandID).FirstOrDefault();
            if (getbrandname != null)
            {
                Product Add = new Product();
                Add.BrandID = obj.BrandID;
                Add.Brandname = getbrandname.BrandName;
                Add.UpdateBy = "Admin";
                Add.pincode = pincodee.ToString();
                Add.categoryId = obj.categoryId;
                Add.Cashbacktype = obj.Cashbacktype;
                Add.Description = obj.Description;
                Add.Discount = obj.Discount;
                Add.createDate = DateTime.Now;
                Add.Name = obj.Name;
                Add.GST = obj.GST;
                Add.Weight = obj.Weight;
                Add.Price = obj.Price;
                Add.Type = obj.Type;
                Add.ApprovedByAdmin = true;
                Add.subcatid = obj.subcatid;
                Add.Unit = obj.Unit;
                Add.Image = file != null ? "/Products/" + FileUploader.UploadImage(file, "Products") : null;
                Add.active = true;
                if (obj.color != null)
                {
                    StringBuilder Color = new StringBuilder();
                    for (int i = 0; i < obj.color.Length; i++)
                    {
                        Color.Append(obj.color[i] + ",");
                    }
                    Add.productColor = Color.ToString();
                }
                db.Products.Add(Add);
                db.SaveChanges();

                //foreach (var item in file)
                //{
                //if (file != null)
                //{
                //    ProductImage Add1 = new ProductImage();
                //    //Add1.Images = FileUploader.UploadImage(item, "ProductImages");
                //    Add1.Images = file != null ? "/ProductImages/" + FileUploader.UploadImage(file, "ProductImages") : null;
                //    Add1.ProductId = Add.ID;
                //    db.ProductImages.Add(Add1);
                //    db.SaveChanges();
                //}
                //}

                if (obj.Heading != null && obj.Para != null)
                {
                    foreach (var item in obj.Heading)
                    {
                        foreach (var item1 in obj.Para)
                        {
                            tbl_Dimensions dm = new tbl_Dimensions();
                            dm.Heading = item;
                            dm.Para = item1;
                            dm.ProductID = Add.ID;
                            db.tbl_Dimensions.Add(dm);
                            db.SaveChanges();
                        }
                    }

                }
                if (obj.SHeading != null && obj.SPara != null)
                {
                    foreach (var item in obj.SHeading)
                    {
                        foreach (var item1 in obj.SPara)
                        {
                            tbl_Specifications dm = new tbl_Specifications();
                            dm.Heading = item;
                            dm.Para = item1;
                            dm.ProductID = Add.ID;
                            db.tbl_Specifications.Add(dm);
                            db.SaveChanges();
                        }
                    }

                }
                TempData["MSG"] = "<script>alert('Product Added successfully')</script>";
                return RedirectToAction("AddnewProduct", "Admin");
            }
            return RedirectToAction("AddnewProduct", "Admin");
        }

        public JsonResult getsubcategory(int categoryId)
        {
            var data = db.subcategories.Where(a => a.catid == categoryId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditProducts(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            var userdata = db.Products.Where(a => a.ID == Id).FirstOrDefault();
            ViewBag.Brand = db.Brands.ToList();
            ViewBag.Cate = db.Categories.ToList();
            ViewBag.subcate = db.subcategories.ToList();

            ProductDto obj = new ProductDto()
            {
                BrandID = userdata.BrandID,
                ID = userdata.ID,
                UpdateBy = userdata.UpdateBy,
                BrandName = db.Products.Where(a => a.BrandID == userdata.BrandID).FirstOrDefault()==null?"": db.Products.Where(a => a.BrandID == userdata.BrandID).FirstOrDefault().Name,
                active = userdata.active,
                Name = userdata.Name,
                categoryId = userdata.categoryId,
                CategoryName = db.Categories.Where(a => a.ID == userdata.categoryId).FirstOrDefault()==null?"" : db.Categories.Where(a => a.ID == userdata.categoryId).FirstOrDefault().Name,
                Description = userdata.Description,
                Discount = userdata.Discount,
                GST = userdata.GST,
                Weight = userdata.Weight,
                Image = db.ProductImages.Where(a => a.ProductId == userdata.ID).FirstOrDefault() ==null?"": db.ProductImages.Where(a => a.ProductId == userdata.ID).FirstOrDefault().Images,
                ImageId = db.ProductImages.Where(a => a.ProductId == userdata.ID).FirstOrDefault()==null?0: db.ProductImages.Where(a => a.ProductId == userdata.ID).FirstOrDefault().id,
                Price = userdata.Price,
                subcatid = userdata.subcatid,
                Subname = db.subcategories.Where(a => a.id == userdata.subcatid).FirstOrDefault()==null?"" : db.subcategories.Where(a => a.id == userdata.subcatid).FirstOrDefault().subcategorname,
                Unit = userdata.Unit
            };

            return View(obj);
        }

        [HttpPost]
        public ActionResult EditProducts(ProductDto obj, HttpPostedFileBase file)
        {
            Product Add = new Product()
            {
                BrandID = obj.BrandID,
                Unit = obj.Unit,
                GST = obj.GST,
                active = obj.active,
                categoryId = obj.categoryId,
                UpdateBy = obj.UpdateBy,
                Weight = obj.Weight,
                Description = obj.Description,
                Discount = obj.Discount,
                Name = obj.Name,
                ID = obj.ID,
                Price = obj.Price,
                subcatid = obj.subcatid,
                Image = file != null ? "/Products/" + FileUploader.UploadImage(file, "Products") : null,
            };
            db.Entry<Product>(Add).State = System.Data.EntityState.Modified;
            db.SaveChanges();

            //ProductImage Add1 = new ProductImage();
            //Add1.ProductId = obj.ID;
            //if (obj.Image != null)
            //{
            //    Add1.Images = obj.Image;
            //}
            //if (obj.file != null)
            //{
            //    //Add1.Images = FileUploader.UploadImage(obj.file, "ProductImages");
            //    Add1.Images = file != null ? "/ProductImages/" + FileUploader.UploadImage(file, "ProductImages") : null;
            //}
            //Add1.id = Convert.ToInt32(obj.ImageId);
            //db.Entry<ProductImage>(Add1).State = System.Data.EntityState.Modified;
            //db.SaveChanges();
            return RedirectToAction("ViewAllProduct");
        }

        public ActionResult ViewAllProduct(string tosellproduct)
        {
            var data = db.Products.OrderByDescending(a => a.ID).ToList();

            return View(data);
        }

        public ActionResult DeleteProduct(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }

            db.Products.Remove(db.Products.Find(Id));
            db.SaveChanges();
            return RedirectToAction("ViewAllProduct");
        }
        #endregion


        #region New Order Management
        public ActionResult ViewAllOrder()
        {
            List<innerjoinClass> ob = new List<innerjoinClass>();
            var orders = db.tbl_Order.ToList();
            var orderDetails = db.OrderDetails.ToList();
            foreach (var Order in orders)
            {
                var Getdetails = orderDetails.Where(a => a.OrderId == Order.ID).ToList();
                foreach (var OrderDetail in Getdetails)
                {
                    innerjoinClass abc = new innerjoinClass()
                    {
                        tbl_OrderID = Order.ID,
                        OrderdetailsID = OrderDetail.ID,
                        ordernumber = Order.OrderId,
                        CustomerName = Order.CustomerName,
                        CustomerMobile = Order.CustomerNumber,
                        OrderTotal = (int)Order.TotalOrderAmount,
                        PayMode = Order.PaymentMode,
                        ProductID = (int)OrderDetail.ProductId,
                        ProductImage = OrderDetail.ProductImage,
                        ProductName = OrderDetail.ProductName,
                        DeliveryStatus = OrderDetail.DeliveryStatus,
                        Date = (DateTime)OrderDetail.CreateDate,
                        CustomerShippingDetails = Order.ShippingAddress,
                        ProductDescription = OrderDetail.ProductName,
                        ProductPrice = (int)OrderDetail.ListedPrice,
                        Quantity = (int)OrderDetail.Quantity,
                    };
                    ob.Add(abc);
                }
            }

            return View(ob);
        }

        #endregion

        #region Return Management
        //public ActionResult AllReturnRequests()
        //{
        //    if (Session["adminid"] == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    List<MyReturnProductsDto> ob = new List<MyReturnProductsDto>();
        //    var Prodata = db.tbl_Order.ToList();
        //    foreach (var item in Prodata)
        //    {
        //        foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.Isreturned == true && a.ReturnRequest == null).ToList())
        //        {
        //            MyReturnProductsDto obj = new MyReturnProductsDto();
        //            obj.ID = item.ID;
        //            obj.Status = item1.DeliveryStatus;
        //            obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //            obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //            obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //            obj.Od_ID = item1.ID;
        //            obj.ProductId = item1.ProductId;
        //            if (db.Products.Any(a => a.ID == item1.ProductId))
        //            {
        //                obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //            }
        //            obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //            obj.Price = item1.Price;
        //            obj.PaymentMethod = item.paymentMethod;
        //            obj.Quantity = item1.Quantity;
        //            obj.Discount = item1.Discount;
        //            obj.ReturnReason = item1.ReasonForReturn;
        //            obj.Returnproof = item1.ImagesForReturn;
        //            obj.TotalAmount = Convert.ToDouble(item1.Amount);
        //            obj.Od_p_status = obj.Status;
        //            obj.OrderIdN = item.OrderId;
        //            obj.isReturnApproved = item1.ReturnRequest;
        //            if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //            {
        //                obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //            }
        //            if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //            {
        //                obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //            }
        //            else
        //            {
        //                obj.seller_SelID = "Admin";
        //            }

        //            ob.Add(obj);
        //        }
        //    }
        //    ViewBag.data = ob;
        //    return View();
        //}

        //public ActionResult RejectedReturns()
        //{
        //    if (Session["adminid"] == null)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    List<MyReturnProductsDto> ob = new List<MyReturnProductsDto>();
        //    var Prodata = db.tbl_Order.ToList();
        //    foreach (var item in Prodata)
        //    {
        //        foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.Isreturned == true && a.ReturnRequest == "REJECTED").ToList())
        //        {
        //            MyReturnProductsDto obj = new MyReturnProductsDto();
        //            obj.ID = item.ID;
        //            obj.Status = item1.DeliveryStatus;
        //            obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //            obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //            obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //            obj.Od_ID = item1.ID;
        //            obj.ProductId = item1.ProductId;
        //            if (db.Products.Any(a => a.ID == item1.ProductId))
        //            {
        //                obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //            }
        //            obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //            obj.Price = item1.Price;
        //            obj.PaymentMethod = item.paymentMethod;
        //            obj.Quantity = item1.Quantity;
        //            obj.Discount = item1.Discount;
        //            obj.ReturnReason = item1.ReasonForReturn;
        //            obj.Returnproof = item1.ImagesForReturn;
        //            obj.TotalAmount = Convert.ToDouble(Convert.ToDouble(item1.Price * item1.Quantity) - Convert.ToDouble(item1.Price * item1.Quantity) * item1.Discount / 100);
        //            obj.Od_p_status = obj.Status;
        //            obj.OrderIdN = item.OrderId;
        //            obj.isReturnApproved = item1.ReturnRequest;
        //            if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //            {
        //                obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //            }
        //            if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //            {
        //                obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //            }
        //            else
        //            {
        //                obj.seller_SelID = "Admin";
        //            }

        //            ob.Add(obj);
        //        }
        //    }
        //    ViewBag.data = ob;
        //    return View();
        //}

        //public ActionResult ApprovedReturns()
        //{
        //    if (Session["adminid"] == null)
        //    {
        //        return RedirectToAction("Index");
        //    }


        //    List<MyReturnProductsDto> ob = new List<MyReturnProductsDto>();
        //    var Prodata = db.tbl_Order.ToList();
        //    foreach (var item in Prodata)
        //    {
        //        foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.Isreturned == true && a.ReturnRequest == "APPROVED" && a.DeliveryStatusOfReturn != "PICKUP_UNDER_PROCESS" && a.DeliveryStatusOfReturn != "PICKUP_RETURN" && a.DeliveryStatusOfReturn != "PICKUP_SHIPPED" && a.DeliveryStatusOfReturn != "PICKUP_TRANSIT" && a.DeliveryStatusOfReturn != "PICKUP_DELIVERED" && a.DeliveryStatusOfReturn != "RETURN_UNDER_PROCESS" && a.DeliveryStatusOfReturn != "RETURN_PICKUP" && a.DeliveryStatusOfReturn != "RETURN_SHIPPED" && a.DeliveryStatusOfReturn != "RETURN_TRANSIT" && a.DeliveryStatusOfReturn != "RETURN_DELIVERED").ToList())
        //        {
        //            MyReturnProductsDto obj = new MyReturnProductsDto();
        //            obj.ID = item.ID;
        //            obj.Status = item1.DeliveryStatus;
        //            obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //            obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //            obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //            obj.Od_ID = item1.ID;
        //            obj.ProductId = item1.ProductId;
        //            if (db.Products.Any(a => a.ID == item1.ProductId))
        //            {
        //                obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //            }
        //            obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //            obj.Price = item1.Price;
        //            obj.PaymentMethod = item.paymentMethod;
        //            obj.Quantity = item1.Quantity;
        //            obj.Discount = item1.Discount;
        //            obj.ReturnReason = item1.ReasonForReturn;
        //            obj.Returnproof = item1.ImagesForReturn;
        //            obj.TotalAmount = Convert.ToDouble(Convert.ToDouble(item1.Price * item1.Quantity) - Convert.ToDouble(item1.Price * item1.Quantity) * item1.Discount / 100);
        //            obj.Od_p_status = obj.Status;
        //            obj.OrderIdN = item.OrderId;
        //            obj.isReturnApproved = item1.ReturnRequest;
        //            if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //            {
        //                obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //            }
        //            if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //            {
        //                obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //            }
        //            else
        //            {
        //                obj.seller_SelID = "Admin";
        //            }

        //            ob.Add(obj);
        //        }
        //    }
        //    ViewBag.data = ob;
        //    return View();
        //}

        //public ActionResult PickupUnderProcess(string Type)
        //{
        //    if (Session["adminid"] == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    TempData["title"] = null;

        //    List<MyReturnProductsDto> ob = new List<MyReturnProductsDto>();
        //    var Prodata = db.tbl_Order.ToList();

        //    string Type_ = "";
        //    if (Type == "PICKUP_RETURN")
        //    {
        //        Type_ = "PICKUP_RETURN";
        //        TempData["title"] = "PICKUP_RETURN";
        //    }
        //    if (Type == "PICKUP_RETURN")
        //    {
        //        Type_ = "PICKUP_RETURN";
        //        TempData["title"] = "PICKUP_RETURN";
        //    }

        //    else if (Type == "PICKUP_SHIPPED")
        //    {
        //        Type_ = "PICKUP_SHIPPED";
        //        TempData["title"] = "PICKUP_SHIPPED";
        //    }

        //    else if (Type == "PICKUP_TRANSIT")
        //    {
        //        Type_ = "PICKUP_TRANSIT";
        //        TempData["title"] = "PICKUP_TRANSIT";
        //    }
        //    else if (Type == "PICKUP_DELIVERED")
        //    {
        //        Type_ = "PICKUP_DELIVERED";
        //        TempData["title"] = "PICKUP_DELIVERED";
        //    }

        //    else if (Type == "RETURN_UNDER_PROCESS")
        //    {
        //        Type_ = "RETURN_UNDER_PROCESS";
        //        TempData["title"] = "RETURN_UNDER_PROCESS";
        //    }
        //    else if (Type == "RETURN_PICKUP")
        //    {
        //        Type_ = "RETURN_PICKUP";
        //        TempData["title"] = "RETURN_PICKUP";
        //    }
        //    else if (Type == "RETURN_SHIPPED")
        //    {
        //        Type_ = "RETURN_SHIPPED";
        //        TempData["title"] = "RETURN_SHIPPED";
        //    }
        //    else if (Type == "RETURN_TRANSIT")
        //    {
        //        Type_ = "RETURN_TRANSIT";
        //        TempData["title"] = "RETURN_TRANSIT";
        //    }
        //    else if (Type == "RETURN_DELIVERED")
        //    {
        //        Type_ = "RETURN_DELIVERED";
        //        TempData["title"] = "RETURN_DELIVERED";
        //    }

        //    if (Type != null && Type_ != "")
        //    {
        //        foreach (var item in Prodata)
        //        {
        //            foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.Isreturned == true && a.ReturnRequest == "APPROVED" && a.DeliveryStatusOfReturn == Type_).ToList())
        //            {
        //                MyReturnProductsDto obj = new MyReturnProductsDto();
        //                obj.ID = item.ID;
        //                obj.Status = item1.DeliveryStatusOfReturn;
        //                obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //                obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //                obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //                obj.Od_ID = item1.ID;
        //                obj.ProductId = item1.ProductId;
        //                if (db.Products.Any(a => a.ID == item1.ProductId))
        //                {
        //                    obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //                }
        //                obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //                obj.Price = item1.Price;
        //                obj.PaymentMethod = item.paymentMethod;
        //                obj.Quantity = item1.Quantity;
        //                obj.Discount = item1.Discount;
        //                obj.ReturnReason = item1.ReasonForReturn;
        //                obj.Returnproof = item1.ImagesForReturn;
        //                obj.TotalAmount = Convert.ToDouble(Convert.ToDouble(item1.Price * item1.Quantity) - Convert.ToDouble(item1.Price * item1.Quantity) * item1.Discount / 100);
        //                obj.Od_p_status = obj.Status;
        //                obj.OrderIdN = item.OrderId;
        //                obj.isReturnApproved = item1.ReturnRequest;
        //                if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //                {
        //                    obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                    obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //                }
        //                if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //                {
        //                    obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //                }
        //                else
        //                {
        //                    obj.seller_SelID = "Admin";
        //                }

        //                ob.Add(obj);
        //            }
        //        }
        //        ViewBag.data = ob;
        //        return View();
        //    }

        //    foreach (var item in Prodata)
        //    {
        //        foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.Isreturned == true && a.ReturnRequest == "APPROVED" && a.DeliveryStatusOfReturn == "PICKUP_UNDER_PROCESS").ToList())
        //        {
        //            MyReturnProductsDto obj = new MyReturnProductsDto();
        //            obj.ID = item.ID;
        //            obj.Status = item1.DeliveryStatusOfReturn;
        //            obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //            obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //            obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //            obj.Od_ID = item1.ID;
        //            obj.ProductId = item1.ProductId;
        //            if (db.Products.Any(a => a.ID == item1.ProductId))
        //            {
        //                obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //            }
        //            obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //            obj.Price = item1.Price;
        //            obj.PaymentMethod = item.paymentMethod;
        //            obj.Quantity = item1.Quantity;
        //            obj.Discount = item1.Discount;
        //            obj.ReturnReason = item1.ReasonForReturn;
        //            obj.Returnproof = item1.ImagesForReturn;
        //            obj.TotalAmount = Convert.ToDouble(Convert.ToDouble(item1.Price * item1.Quantity) - Convert.ToDouble(item1.Price * item1.Quantity) * item1.Discount / 100);
        //            obj.Od_p_status = obj.Status;
        //            obj.OrderIdN = item.OrderId;
        //            obj.isReturnApproved = item1.ReturnRequest;
        //            if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //            {
        //                obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //            }
        //            if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //            {
        //                obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //            }
        //            else
        //            {
        //                obj.seller_SelID = "Admin";
        //            }

        //            ob.Add(obj);
        //        }
        //    }
        //    TempData["title"] = "Pickup Under Process";
        //    ViewBag.data = ob;
        //    return View();
        //}
        #endregion

        #region Refund Management
        //public ActionResult RefundRequest()
        //{
        //    if (Session["adminid"] == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    List<RefundOrdersDTO> ob = new List<RefundOrdersDTO>();
        //    var Prodata = db.tbl_Order.ToList();
        //    foreach (var item in Prodata)
        //    {
        //        foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.IsCancelled == true && a.Refundrequest == null).ToList())
        //        {
        //            RefundOrdersDTO obj = new RefundOrdersDTO();
        //            obj.ID = item.ID;
        //            obj.Status = item1.DeliveryStatus;
        //            obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //            obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //            obj.Iscanclled = Convert.ToBoolean(item1.IsCancelled);
        //            obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //            obj.RefundDate = Convert.ToDateTime(item1.RefundReqDate);
        //            obj.refundapproveStatus = item1.Refundrequest;
        //            obj.RefundStatus = item1.RefundStatus;
        //            obj.Od_ID = item1.ID;
        //            obj.ProductId = item1.ProductId;
        //            if (db.Products.Any(a => a.ID == item1.ProductId))
        //            {
        //                obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //            }
        //            obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //            obj.Price = item1.Price;
        //            obj.PaymentMethod = item.paymentMethod;
        //            obj.Quantity = item1.Quantity;
        //            obj.Discount = item1.Discount;
        //            obj.TotalAmount = Convert.ToDouble(Convert.ToDouble(item1.Price * item1.Quantity) - Convert.ToDouble(item1.Price * item1.Quantity) * item1.Discount / 100);
        //            obj.Od_p_status = obj.Status;
        //            obj.OrderIdN = item.OrderId;
        //            obj.isReturnApproved = item1.ReturnRequest;
        //            if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //            {
        //                obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //            }
        //            if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //            {
        //                obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //            }
        //            else
        //            {
        //                obj.seller_SelID = "Admin";
        //            }

        //            ob.Add(obj);
        //        }
        //    }
        //    ViewBag.data = ob;
        //    return View();
        //}

        //public ActionResult ApprovedRefund(string Type)
        //{
        //    if (Session["adminid"] == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    TempData["title"] = null;
        //    List<RefundOrdersDTO> ob = new List<RefundOrdersDTO>();
        //    var Prodata = db.tbl_Order.ToList();

        //    string Type_ = "";

        //    if (Type == "PICKUP_UNDER_PROCESS")
        //    {
        //        Type_ = "PICKUP_UNDER_PROCESS";
        //        TempData["title"] = "PICKUP_UNDER_PROCESS";
        //    }

        //    else if (Type == "PICKUP_RETURN")
        //    {
        //        Type_ = "PICKUP_RETURN";
        //        TempData["title"] = "PICKUP_RETURN";
        //    }

        //    else if (Type == "PICKUP_SHIPPED")
        //    {
        //        Type_ = "PICKUP_SHIPPED";
        //        TempData["title"] = "PICKUP_SHIPPED";
        //    }
        //    else if (Type == "PICKUP_TRANSIT")
        //    {
        //        Type_ = "PICKUP_TRANSIT";
        //        TempData["title"] = "PICKUP_TRANSIT";
        //    }

        //    else if (Type == "PICKUP_DELIVERED")
        //    {
        //        Type_ = "PICKUP_DELIVERED";
        //        TempData["title"] = "PICKUP_DELIVERED";
        //    }
        //    else if (Type == "PROCEED_REFUND")
        //    {
        //        Type_ = "PROCEED_REFUND";
        //        TempData["title"] = "PROCEED_REFUND";
        //    }
        //    else if (Type == "CancelledOrders")
        //    {
        //        foreach (var item in Prodata)
        //        {
        //            foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.IsCancelled == true && a.Refundrequest == "REJECTED").ToList())
        //            {
        //                RefundOrdersDTO obj = new RefundOrdersDTO();
        //                obj.ID = item.ID;
        //                obj.Status = item1.DeliveryStatus;
        //                obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //                obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //                obj.Iscanclled = Convert.ToBoolean(item1.IsCancelled);
        //                obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //                obj.RefundDate = Convert.ToDateTime(item1.RefundReqDate);
        //                obj.refundapproveStatus = item1.Refundrequest;
        //                obj.RefundStatus = item1.RefundStatus;
        //                obj.Od_ID = item1.ID;
        //                obj.ProductId = item1.ProductId;
        //                if (db.Products.Any(a => a.ID == item1.ProductId))
        //                {
        //                    obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //                }
        //                obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //                obj.Price = item1.Price;
        //                obj.PaymentMethod = item.paymentMethod;
        //                obj.Quantity = item1.Quantity;
        //                obj.Discount = item1.Discount;
        //                obj.TotalAmount = Convert.ToDouble(Convert.ToDouble(item1.Price * item1.Quantity) - Convert.ToDouble(item1.Price * item1.Quantity) * item1.Discount / 100);
        //                obj.Od_p_status = obj.Status;
        //                obj.OrderIdN = item.OrderId;
        //                obj.isReturnApproved = item1.ReturnRequest;
        //                if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //                {
        //                    obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                    obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //                }
        //                if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //                {
        //                    obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //                }
        //                else
        //                {
        //                    obj.seller_SelID = "Admin";
        //                }

        //                ob.Add(obj);
        //            }
        //        }
        //        ViewBag.data = ob;
        //        return View();
        //        TempData["title"] = "PROCEED_REFUND";
        //    }


        //    if (Type != null && Type_ != "")
        //    {
        //        foreach (var item in Prodata)
        //        {
        //            foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.IsCancelled == true && a.Refundrequest == "APPROVED" && a.RefundStatus == Type_).ToList())
        //            {
        //                RefundOrdersDTO obj = new RefundOrdersDTO();
        //                obj.ID = item.ID;
        //                obj.Status = item1.DeliveryStatus;
        //                obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //                obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //                obj.Iscanclled = Convert.ToBoolean(item1.IsCancelled);
        //                obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //                obj.RefundDate = Convert.ToDateTime(item1.RefundReqDate);
        //                obj.refundapproveStatus = item1.Refundrequest;
        //                obj.RefundStatus = item1.RefundStatus;
        //                obj.Od_ID = item1.ID;
        //                obj.ProductId = item1.ProductId;
        //                if (db.Products.Any(a => a.ID == item1.ProductId))
        //                {
        //                    obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //                }
        //                obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //                obj.Price = item1.Price;
        //                obj.PaymentMethod = item.paymentMethod;
        //                obj.Quantity = item1.Quantity;
        //                obj.Discount = item1.Discount;
        //                obj.TotalAmount = Convert.ToDouble(Convert.ToDouble(item1.Price * item1.Quantity) - Convert.ToDouble(item1.Price * item1.Quantity) * item1.Discount / 100);
        //                obj.Od_p_status = obj.Status;
        //                obj.OrderIdN = item.OrderId;
        //                obj.isReturnApproved = item1.ReturnRequest;
        //                if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //                {
        //                    obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                    obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //                }
        //                if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //                {
        //                    obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //                }
        //                else
        //                {
        //                    obj.seller_SelID = "Admin";
        //                }

        //                ob.Add(obj);
        //            }
        //        }
        //        ViewBag.data = ob;
        //        return View();
        //    }


        //    foreach (var item in Prodata)
        //    {
        //        foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.IsCancelled == true && a.Refundrequest == "APPROVED").ToList())
        //        {
        //            RefundOrdersDTO obj = new RefundOrdersDTO();
        //            obj.ID = item.ID;
        //            obj.Status = item1.DeliveryStatus;
        //            obj.ReturnDate = Convert.ToDateTime(item1.ReturnDate);
        //            obj.IsConfirm = Convert.ToBoolean(item1.IsConfirmed);
        //            obj.Iscanclled = Convert.ToBoolean(item1.IsCancelled);
        //            obj.CreateDate = Convert.ToDateTime(item.Createdate);
        //            obj.RefundDate = Convert.ToDateTime(item1.RefundReqDate);
        //            obj.refundapproveStatus = item1.Refundrequest;
        //            obj.RefundStatus = item1.RefundStatus;
        //            obj.Od_ID = item1.ID;
        //            obj.ProductId = item1.ProductId;
        //            if (db.Products.Any(a => a.ID == item1.ProductId))
        //            {
        //                obj.Productname = db.Products.Where(a => a.ID == item1.ProductId).FirstOrDefault().Name;
        //            }
        //            obj.productImage = db.ProductImages.Where(a => a.ProductId == obj.ProductId).FirstOrDefault().Images;
        //            obj.Price = item1.Price;
        //            obj.PaymentMethod = item.paymentMethod;
        //            obj.Quantity = item1.Quantity;
        //            obj.Discount = item1.Discount;
        //            obj.TotalAmount = Convert.ToDouble(Convert.ToDouble(item1.Price * item1.Quantity) - Convert.ToDouble(item1.Price * item1.Quantity) * item1.Discount / 100);
        //            obj.Od_p_status = obj.Status;
        //            obj.OrderIdN = item.OrderId;
        //            obj.isReturnApproved = item1.ReturnRequest;
        //            if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //            {
        //                obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().self_s_id;
        //                obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //            }
        //            if (db.sallerDetails.Any(a => a.sallerId == item1.sallerid))
        //            {
        //                obj.seller_SelID = db.sallerDetails.Where(a => a.sallerId == item1.sallerid).FirstOrDefault().Salerusername;
        //            }
        //            else
        //            {
        //                obj.seller_SelID = "Admin";
        //            }

        //            ob.Add(obj);
        //        }
        //    }
        //    TempData["title"] = "Approved Refunds";
        //    ViewBag.data = ob;
        //    return View();
        //}


        #endregion

        #region PinCode

        public ActionResult AddPin()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddPin(tbl_pincode obj)
        {
            tbl_pincode pin = new tbl_pincode();
            if (db.tbl_pincode.Any(a => a.Pincode == obj.Pincode))
            {
                TempData["msg"] = "pin is added allready";
                return RedirectToAction("AddPin", "Admin");
            }
            pin.Pincode = obj.Pincode;
            pin.State = obj.State;
            pin.City = obj.City;
            db.tbl_pincode.Add(pin);
            db.SaveChanges();
            TempData["msg"] = "Pin Code Added Successfully";
            return RedirectToAction("AddPin", "Admin");
        }
        public ActionResult PinCode()
        {
            var data = db.tbl_pincode.ToList();
            ViewBag.data = data;
            return View();
        }
        public ActionResult DelPincode(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }

            db.tbl_pincode.Remove(db.tbl_pincode.Find(Id));
            db.SaveChanges();
            return RedirectToAction("PinCode");
        }
        public ActionResult EditPincode(int Id)
        {
            ViewBag.data = db.tbl_pincode.Where(a => a.Id == Id).FirstOrDefault();
            return View();
        }

        [HttpPost]
        public ActionResult EditPincode(tbl_pincode obj)
        {

            db.Entry<tbl_pincode>(obj).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            TempData["msg"] = "<script>alert('Update Successfull!!!')</script>";
            return RedirectToAction("PinCode");
        }

        public ActionResult Usersdetails()
        {
            return View(db.Clients.OrderByDescending(a => a.ID).ToList());
        }

        public ActionResult Deluser(int id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }

            db.Clients.Remove(db.Clients.Find(id));
            var cashback = db.tbl_cashback.Where(a => a.CientId == id).ToList();
            foreach (var item in cashback)
            {
                db.tbl_cashback.Remove(item);

            }
            var order = db.tbl_Order.Where(a => a.ClientId == id).ToList();
            foreach (var item in order)
            {

                db.tbl_Order.Remove(item);
                var orderdetail = db.OrderDetails.Where(a => a.OrderId == item.ID).ToList();
                foreach (var item1 in orderdetail)
                {
                    db.OrderDetails.Remove(item1);
                }


            }
            var address = db.tbl_address.Where(a => a.ClientId == id).ToList();
            foreach (var item in address)
            {
                db.tbl_address.Remove(item);
            }
            //var kart = db.KartDetails.Where(a => a.ClientId == id).ToList();
            //foreach (var item in kart)
            //{
            //    db.KartDetails.Remove(item);
            //}


            db.SaveChanges();
            return RedirectToAction("Usersdetails");
        }
        #endregion

        #region ProceedOrder
        public ActionResult ProceedOrder(int ID)
        {
            var Data = db.OrderDetails.Where(a => a.ID == ID).FirstOrDefault();
            if (Data != null)
            {
                if (Data.DeliveryStatus == "Pending")
                {
                    Data.DeliveryStatus = "UNDER_PROCESS";
                    db.Entry<OrderDetail>(Data).State = System.Data.EntityState.Modified;
                }
                else if (Data.DeliveryStatus == "UNDER_PROCESS")
                {
                    Data.DeliveryStatus = "AWATING_PICKUP";
                    db.Entry<OrderDetail>(Data).State = System.Data.EntityState.Modified;
                }
                else if (Data.DeliveryStatus == "AWATING_PICKUP")
                {
                    Data.DeliveryStatus = "SHIPPED";
                    db.Entry<OrderDetail>(Data).State = System.Data.EntityState.Modified;
                }
                else if (Data.DeliveryStatus == "SHIPPED")
                {
                    Data.DeliveryStatus = "ORDER_TRANSIT";
                    db.Entry<OrderDetail>(Data).State = System.Data.EntityState.Modified;
                }
                else if (Data.DeliveryStatus == "ORDER_TRANSIT")
                {
                    Data.DeliveryStatus = "DELIVERED";
                    db.Entry<OrderDetail>(Data).State = System.Data.EntityState.Modified;
                }
            }
            db.SaveChanges();
            return RedirectToAction("ViewAllOrder", "Admin");
        }

        #endregion

        #region Return Proceed
        public ActionResult ReturnProceedOrder(int ID)
        {
            var sallerdatra = db.OrderDetails.Where(a => a.ID == ID && a.Isreturned == true && a.ReturnRequest == "APPROVED").FirstOrDefault();
            if (sallerdatra != null)
            {
                if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_UNDER_PROCESS")
                {
                    sallerdatra.DeliveryStatusOfReturn = "PICKUP_RETURN";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_RETURN")
                {
                    sallerdatra.DeliveryStatusOfReturn = "PICKUP_SHIPPED";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_SHIPPED")
                {
                    sallerdatra.DeliveryStatusOfReturn = "PICKUP_TRANSIT";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_TRANSIT")
                {
                    sallerdatra.DeliveryStatusOfReturn = "PICKUP_DELIVERED";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_DELIVERED")
                {
                    sallerdatra.DeliveryStatusOfReturn = "RETURN_UNDER_PROCESS";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (sallerdatra.DeliveryStatusOfReturn == "RETURN_UNDER_PROCESS")
                {
                    sallerdatra.DeliveryStatusOfReturn = "RETURN_PICKUP";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (sallerdatra.DeliveryStatusOfReturn == "RETURN_PICKUP")
                {
                    sallerdatra.DeliveryStatusOfReturn = "RETURN_SHIPPED";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (sallerdatra.DeliveryStatusOfReturn == "RETURN_SHIPPED")
                {
                    sallerdatra.DeliveryStatusOfReturn = "RETURN_TRANSIT";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }
                if (sallerdatra.DeliveryStatusOfReturn == "RETURN_TRANSIT")
                {
                    sallerdatra.DeliveryStatusOfReturn = "RETURN_DELIVERED";
                    sallerdatra.ModifiedDate = DateTime.Now;
                    db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Admin");
                }

            }

            return RedirectToAction("Dashboard", "Admin");
        }
        #endregion

        #region RefundProceedOrder
        public ActionResult RefundProceedOrder(int ID)
        {
            foreach (var item in db.OrderDetails.Where(a => a.ID == ID).ToList())
            {
                if (item.ID == ID)
                {
                    if (item.RefundStatus == "PENDING")
                    {
                        item.RefundStatus = "PICKUP_UNDER_PROCESS";
                        db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    if (item.RefundStatus == "PICKUP_UNDER_PROCESS")
                    {
                        item.RefundStatus = "PICKUP_RETURN";
                        db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    if (item.RefundStatus == "PICKUP_RETURN")
                    {
                        item.RefundStatus = "PICKUP_SHIPPED";
                        db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    if (item.RefundStatus == "PICKUP_SHIPPED")
                    {
                        item.RefundStatus = "PICKUP_TRANSIT";
                        db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    if (item.RefundStatus == "PICKUP_TRANSIT")
                    {
                        item.RefundStatus = "PICKUP_DELIVERED";
                        db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    if (item.DeliveryStatus == "PICKUP_DELIVERED")
                    {
                        item.DeliveryStatus = "PROCEED_REFUND";
                        db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Dashboard", "Admin");
                    }

                }
            }
            return View();
        }

        #endregion

        #region Saller
        public ActionResult SallerList(string status)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            if (status == "active")
            {
                var data = db.sallerDetails.Where(a => a.Active == true).OrderByDescending(a => a.sallerId).ToList();
                ViewBag.dat = data;
                return View();
            }
            else if (status == "unactive")
            {
                var data = db.sallerDetails.Where(a => a.Active == false).OrderByDescending(a => a.sallerId).ToList();
                ViewBag.dat = data;
                return View();
            }
            else
            {
                var data1 = db.sallerDetails.OrderByDescending(a => a.sallerId).ToList();
                ViewBag.dat = data1;
            }

            return View();
        }

        public ActionResult ActiveSaller(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            var sallerdatra = db.sallerDetails.Where(a => a.sallerId == Id).FirstOrDefault();
            if (sallerdatra != null && sallerdatra.Active == true)
            {

                sallerdatra.Active = false;
                db.Entry<sallerDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SallerList", "Admin");
            }
            else if (sallerdatra != null && sallerdatra.Active == false)
            {
                sallerdatra.Active = true;
                db.Entry<sallerDetail>(sallerdatra).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SallerList", "Admin");
            }

            return View();
        }

        public ActionResult Delsaller(int Id)
        {
            if (Session["adminid"] != null)
            {
                db.sallerDetails.Remove(db.sallerDetails.Find(Id));
                db.SaveChanges();
                return RedirectToAction("SallerList", "Admin");
            }
            else
            {
                return RedirectToAction("SallerList", "Admin");
            }
        }

        public ActionResult ViewSallerDetails(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            var data = db.sallerDetails.Where(a => a.sallerId == Id).FirstOrDefault();
            if (data == null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            ViewBag.data = data;
            return View();
        }

        #endregion

        #region cashback list

        public ActionResult cashbacklist()
        {
            var cashbackdata = db.tbl_cashback.ToList();
            var datatbl = db.tbl_cashback.OrderByDescending(a => a.ID).ToList();
            List<UsercashbackDTO> Add = new List<UsercashbackDTO>();
            var data = db.tbl_cashback.ToList();
            foreach (var item in data)
            {
                UsercashbackDTO obj = new UsercashbackDTO();
                obj.firstname = db.Clients.Where(a => a.ID == item.CientId).FirstOrDefault().Name;
                obj.lastname = db.Clients.Where(a => a.ID == item.CientId).FirstOrDefault().lastname;
                obj.Userid = item.CientId;
                obj.CashbackId = item.ID;
                obj.CashbackAmount = Convert.ToInt32(item.cashbackvalue);
                Add.Add(obj);
            }
            ViewBag.data = Add;
            return View();
        }

        public ActionResult Wallet(int Id)
        {
            var data = db.tbl_cashback.Where(a => a.ID == Id).FirstOrDefault().cashbackvalue;
            if (data == null)
            {
                return RedirectToAction("cashbacklist", "Admin");
            }
            else
            {
                HttpClient clientlocation = new HttpClient();
                clientlocation.BaseAddress = new Uri("http://freegeoip.net/");
                HttpResponseMessage response_location = clientlocation.GetAsync("json/").Result;
                var jsonlocation = response_location.Content.ReadAsStringAsync().Result;
                var jsonlocationdata = JsonConvert.DeserializeObject<locationNew>(jsonlocation);
                int UserId = Convert.ToInt32(db.tbl_cashback.Where(a => a.ID == Id).FirstOrDefault().CientId);
                tbl_wallettrans obj_credit = new tbl_wallettrans();
                obj_credit.amount = Convert.ToDecimal(db.tbl_cashback.Where(a => a.ID == Id).FirstOrDefault().cashbackvalue);
                obj_credit.createddate = DateTime.Now;
                obj_credit.flag = "C";
                obj_credit.modifieddate = DateTime.Now;
                int cltid = Convert.ToInt32(db.tbl_cashback.Where(a => a.ID == Id).FirstOrDefault().CientId);
                obj_credit.note = db.Clients.Where(a => a.ID == cltid).FirstOrDefault().Mobile;
                obj_credit.source = "Web";
                obj_credit.sysgenid = GenerateNumber();
                obj_credit.userid = UserId;
                obj_credit.appname = Request.Browser.Browser;
                obj_credit.appversion = Request.Browser.Version;
                obj_credit.providertranid = GenerateNumber();
                obj_credit.platform = Request.Browser.Platform;
                obj_credit.latitude = jsonlocationdata.latitude.ToString();
                obj_credit.longitude = jsonlocationdata.longitude.ToString();
                obj_credit.pincode = jsonlocationdata.zip_code;
                obj_credit.timezone = jsonlocationdata.time_zone;
                obj_credit.region = jsonlocationdata.region_name;
                obj_credit.ipaddress = jsonlocationdata.ip;
                obj_credit.city = jsonlocationdata.city;
                obj_credit.countryname = jsonlocationdata.country_name;
                db.tbl_wallettrans.Add(obj_credit);
                db.SaveChanges();
                db.tbl_cashback.Remove(db.tbl_cashback.Find(Id));
                db.SaveChanges();
                return RedirectToAction("cashbacklist", "Admin");
            }

        }
        #endregion


        #region  BOOKING SLOT
        public ActionResult AddBookingSlot()
        {
            return View(db.tbl_orderdate.OrderByDescending(a => a.ID).ToList());
        }
        [HttpPost]
        public ActionResult OrderDates(tbl_orderdate obj, DateTime datadate)
        {
            tbl_orderdate orderd = new tbl_orderdate();
            orderd.Starttime = obj.Starttime;
            orderd.Endtime = obj.Endtime;
            orderd.Date = datadate.ToString("dd-MMM-yyyy");
            orderd.Day = obj.Day;
            db.tbl_orderdate.Add(orderd);
            db.SaveChanges();
            TempData["msg"] = "Order Dates Added";
            return RedirectToAction("AddBookingSlot", "Admin");
        }
        public ActionResult DelOrderDates(int Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            db.tbl_orderdate.Remove(db.tbl_orderdate.Find(Id));
            db.SaveChanges();
            return RedirectToAction("AddBookingSlot", "Admin");
        }
        #endregion

        #region random no

        private Random randomnumber = new Random();
        public string GenerateNumber()
        {
            return randomnumber.Next(0, 99999).ToString("D5");
        }
        #endregion

        #region UserList
        public ActionResult UserLists()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            var data = db.tbl_user.Where(a => a.Active == true).ToList();
            ViewBag.Data = data;
            return View();
        }
        public ActionResult NewUsers()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
             
            return View(db.tbl_registration.Where(x=>x.date.Value.Day == DateTime.Now.Day && x.date.Value.Month == DateTime.Now.Month && x.date.Value.Year == DateTime.Now.Year).ToList());

        }

        //public ActionResult NewUsers()
        //{
        //    List<TotalSellersnbuyers> obj = new List<TotalSellersnbuyers>();

        //    foreach (var item in db.tbl_user.Where(a => a.Active == false).ToList())
        //    {

        //        TotalSellersnbuyers obj1 = new TotalSellersnbuyers();
        //        var data1 = db.tbl_user.Where(a => a.UserId == item.UserId).FirstOrDefault();
        //        obj1.UserId = data1.UserId;
        //        obj1.full_name = data1.full_name;
        //        obj1.email = data1.email;
        //        obj1.self_s_id = data1.self_s_id;
        //        obj1.Active = data1.Active;
        //        obj1.Document = data1.DocImage;
        //        obj1.Address = data1.Address;
        //        obj1.UserType = "Buyers";
        //        obj.Add(obj1);

        //    }

        //    foreach (var item in db.sallerDetails.Where(a => a.Active == false).ToList())
        //    {

        //        TotalSellersnbuyers obj2 = new TotalSellersnbuyers();
        //        var data1 = db.sallerDetails.Where(a => a.sallerId == item.sallerId).FirstOrDefault();
        //        obj2.UserId = data1.sallerId;
        //        obj2.full_name = data1.FirstName;
        //        obj2.email = data1.SallerEmailId;
        //        obj2.self_s_id = data1.Salerusername;
        //        obj2.Active = data1.Active;
        //        obj2.BussinessType = data1.BussinessType;
        //        obj2.Document = data1.Document;
        //        obj2.UserType = "Sellers";
        //        obj.Add(obj2);
        //    }
        //    ViewBag.Data = obj;
        //    return View();
        //}

        public ActionResult ActiveDeactive(string id, string usertype)
        {
            if (id != null && usertype == "Buyers")
            {
                var data = db.tbl_user.Where(f => f.self_s_id == id).FirstOrDefault();
                if (data.Active == false)
                {
                    data.Active = true;
                    db.Entry<tbl_user>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('User iS Active')</script>";
                    return RedirectToAction("NewUsers", "Admin");
                }
                else
                {
                    data.Active = false;
                    db.Entry<tbl_user>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('User iS De-Active')</script>";
                    return RedirectToAction("NewUsers", "Admin");
                }
            }
            if (id != null && usertype == "Sellers")
            {
                var data = db.sallerDetails.Where(f => f.Salerusername == id).FirstOrDefault();
                if (data.Active == false)
                {
                    data.Active = true;
                    db.Entry<sallerDetail>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('User iS Active')</script>";
                    return RedirectToAction("NewUsers", "Admin");
                }
                else
                {
                    data.Active = false;
                    db.Entry<sallerDetail>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('User iS De-Active')</script>";
                    return RedirectToAction("NewUsers", "Admin");
                }
            }
            if (id != null && usertype == "Delete")
            {
                var data = db.sallerDetails.Where(f => f.Salerusername == id).FirstOrDefault();
                if (data != null)
                {
                    db.sallerDetails.Remove(data);
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('User Deleted')</script>";
                    return RedirectToAction("NewUsers", "Admin");
                }
                var data1 = db.tbl_user.Where(f => f.self_s_id == id).FirstOrDefault();
                if (data1 != null)
                {
                    db.tbl_user.Remove(data1);
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('User Deleted')</script>";
                    return RedirectToAction("NewUsers", "Admin");
                }

            }
            return View();
        }


        public ActionResult DeactivateORActivateUsers(tbl_user obj)
        {
            if (obj.UserId != null)
            {
                var data = db.tbl_user.Where(f => f.UserId == obj.UserId).FirstOrDefault();
                if (data.Active == false)
                {
                    data.Active = true;
                    db.Entry<tbl_user>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('User iS Active')</script>";
                    return RedirectToAction("UserLists", "Admin");
                }
                else
                {
                    data.Active = false;
                    db.Entry<tbl_user>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msg"] = "<script>alert('User iS De-Active')</script>";
                    return RedirectToAction("UserLists", "Admin");
                }
            }
            return View();
        }

        public ActionResult ViewUserDetails(String Id)
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            var data = db.tbl_user.Where(a => a.self_s_id == Id).FirstOrDefault();
            return View(data);
        }


        #endregion


        #region Top Selling Products
        public ActionResult TopSellingProducts()
        {
            var duplicates = db.OrderDetails.GroupBy(s => s.ProductId).Where(g => g.Count() > 1).Select(g => g.Key); // or .SelectMany(g => g)

            foreach (var item in duplicates)
            {
                Console.WriteLine(item);

            }

            return View();
        }
        #endregion

        #region DownloadFile
        public FileResult DownloadFile(string Filename_)
        {
            string[] array = Filename_.Split('.');
            string path = Server.MapPath("~/Images");//name of Folder
            string filename = Path.GetFileName(Filename_);//name of file
            string fullpath = Path.Combine(path, filename);// path of a file
            string type = "Images/" + array[1];//type of file
            string name = "Download." + array[1];//name of file after download
            return File(fullpath, type, name); //full path of downloaded file
        }
        #endregion

        #region Reviws Manage
        public ActionResult Reviews()
        {
            if (Session["adminid"] != null)
            {
                var data = db.tbl_review.OrderByDescending(a => a.ID).ToList();
                if (data != null)
                {
                    ViewBag.dataa = data;
                    return View();
                }
            }
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Delreview(int ReviewId)
        {
            var dataa = db.tbl_review.Where(a => a.ID == ReviewId).FirstOrDefault();
            db.tbl_review.Remove(dataa);
            db.SaveChanges();
            TempData["msg"] = "<script>alert('Review Is Deleted')</script>";
            return RedirectToAction("Reviews", "Admin");
        }


        #endregion

        #region Sponser Product
        public ActionResult SponserProduct(int Id)
        {
            if (db.Products.Any(a => a.ID == Id))
            {
                var data = db.Products.Where(a => a.ID == Id).FirstOrDefault();
                if (data.Sponserd == true)
                {
                    data.Sponserd = false;
                    db.Entry<Product>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msgg"] = "<script>alert('Remove from Sponser!!')</script>";
                }
                else
                {
                    data.Sponserd = true;
                    db.Entry<Product>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msgg"] = "<script>alert('Add To Sponsered!!')</script>";
                }

                return RedirectToAction("ViewAllProduct");
            }
            TempData["msgg"] = "<script>alert('Product is not found!!')</script>";
            return RedirectToAction("ViewAllProduct");
        }
        #endregion

        #region Manage Home Categories
        public ActionResult HomeCateegories()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }

        [HttpPost]
        public ActionResult HomeCateegories(int[] CatID)
        {
            foreach (var item in CatID)
            {
                if (db.Categories.Any(a => a.ID == item))
                {
                    tbl_HomePageCaetegories obj = new tbl_HomePageCaetegories();
                    obj.CatID = item;
                    obj.name = db.Categories.Where(a => a.ID == item).FirstOrDefault().Name;
                    obj.image = db.Categories.Where(a => a.ID == item).FirstOrDefault().Image;
                    db.tbl_HomePageCaetegories.Add(obj);
                    db.SaveChanges();

                }
                else
                {
                    TempData["MSG"] = "<script>alert('Error!!!')</script>";
                    return View();
                }
            }
            TempData["MSG"] = "<script>alert('Categories Is Added!!!')</script>";
            return View();
        }
        public ActionResult ViewHomeCateegories()
        {
            var data = db.tbl_HomePageCaetegories.OrderByDescending(a => a.id).ToList();
            return View();
        }

        public ActionResult DelHomeCategories(int id)
        {
            db.tbl_HomePageCaetegories.Remove(db.tbl_HomePageCaetegories.Where(a => a.id == id).FirstOrDefault());
            db.SaveChanges();
            TempData["MSG"] = "<script>alert('Categories Is Deleted!!!')</script>";
            return RedirectToAction("ViewHomeCateegories");
        }
        #endregion

        #region BlockProduct
        public ActionResult BlockProduct(int Id)
        {

            if (db.Products.Any(a => a.ID == Id))
            {
                var data = db.Products.Where(a => a.ID == Id).FirstOrDefault();
                if (data.active == true)
                {
                    data.active = false;
                    db.Entry<Product>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msgg"] = "<script>alert('Product Blocked!!')</script>";
                }
                else
                {
                    data.active = true;
                    db.Entry<Product>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msgg"] = "<script>alert('Product Unblocked!!')</script>";
                }

                return RedirectToAction("ViewAllProduct", new { tosellproduct = "BlockProduct" });
            }
            TempData["msgg"] = "<script>alert('Product is not found!!')</script>";
            return RedirectToAction("ViewAllProduct", new { tosellproduct = "BlockProduct" });
        }
        #endregion

        #region Approve Or Reject Products
        public ActionResult ApproveProduct(int Id)
        {

            if (db.Products.Any(a => a.ID == Id))
            {
                var data = db.Products.Where(a => a.ID == Id).FirstOrDefault();
                if (data.ApprovedByAdmin == true)
                {

                    data.ApprovedByAdmin = false;
                    db.Entry<Product>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msgg"] = "<script>alert('Product Rejected!!')</script>";


                }
                else
                {
                    data.ApprovedByAdmin = true;
                    db.Entry<Product>(data).State = System.Data.EntityState.Modified;
                    db.SaveChanges();
                    TempData["msgg"] = "<script>alert('Product Approved!!')</script>";
                }
            }
            return RedirectToAction("ViewAllProduct", new { tosellproduct = "SellersProducts" });
        }
        #endregion

        #region Approve Return or Block unlbloack Products
        //public ActionResult BlockOrApproveProducts(int Id, string type)
        //{
        //    var data = db.OrderDetails.Where(a => a.ID == Id).FirstOrDefault();

        //    if (type == "ReturnUnderProcess")
        //    {
        //        data.DeliveryStatusOfReturn = "PICKUP_UNDER_PROCESS";
        //        TempData["Dashboardmessage"] = "<script>alert('pICKUP Under Process!!')</script>";
        //        db.Entry<OrderDetail>(data).State = System.Data.EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Dashboard");
        //    }

        //    if (type == "BlockUnblock")
        //    {

        //        if (data.IsConfirmed == true)
        //        {
        //            data.IsConfirmed = false;
        //            TempData["Dashboardmessage"] = "<script>alert('Order Blocked!!')</script>";
        //            db.Entry<OrderDetail>(data).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard");

        //        }
        //        else
        //        {
        //            data.IsConfirmed = true;
        //            TempData["Dashboardmessage"] = "<script>alert('Order Confirmed!!')</script>";
        //            db.Entry<OrderDetail>(data).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard");
        //        }
        //    }
        //    else if (type == "RefundrequestYES")
        //    {
        //        data.RefundStatus = "PENDING";
        //        data.Refundrequest = "APPROVED";
        //        TempData["Dashboardmessage"] = "<script>alert('Refund Request Approved!!')</script>";
        //        db.Entry<OrderDetail>(data).State = System.Data.EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Dashboard");
        //    }
        //    else if (type == "RefundrequestNO")
        //    {

        //        data.Refundrequest = "REJECTED";
        //        TempData["Dashboardmessage"] = "<script>alert('Refund Request Rejected!!')</script>";
        //        db.Entry<OrderDetail>(data).State = System.Data.EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Dashboard");
        //    }

        //    else if (type == "ReturnAccept")
        //    {
        //        data.ReturnRequest = "APPROVED";
        //        db.Entry<OrderDetail>(data).State = System.Data.EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["Dashboardmessage"] = "<script>alert('Return APPROVED!!')</script>";
        //        return RedirectToAction("Dashboard", "Admin");

        //    }
        //    else if (type == "ReturnReject")
        //    {
        //        data.ReturnRequest = "REJECTED";
        //        db.Entry<OrderDetail>(data).State = System.Data.EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["Dashboardmessage"] = "<script>alert('Return REJECTED!!')</script>";
        //        return RedirectToAction("Dashboard", "Admin");
        //    }


        //    return View();
        //}
        #endregion

        #region SellerOrdersDetails
        //public ActionResult SellerOrdersDetails()
        //{
        //    List<OrderDetailDTO> obj = new List<OrderDetailDTO>();
        //    var Orderdata = db.OrderDetails.OrderByDescending(a => a.ID).Where(a => a.sallerid != null).ToList();
        //    foreach (var item in Orderdata)
        //    {
        //        OrderDetailDTO Add = new OrderDetailDTO();
        //        Add.ID = item.ID;
        //        Add.ProductName = db.Products.Where(a => a.ID == item.ProductId).FirstOrDefault().Name;
        //        Add.Quantity = item.Quantity;
        //        Add.Price = item.Price;
        //        Add.Discount = item.Discount;
        //        var orderdata1 = db.tbl_Order.Where(a => a.ID == item.OrderId).FirstOrDefault();
        //        Add.ClientName = orderdata1.Name;
        //        Add.Address = orderdata1.Address;
        //        Add.DeliveryStatus = orderdata1.DeliveryStatus;
        //        Add.PaymentMethod = orderdata1.paymentMethod;
        //        Add.PaymentStatus = orderdata1.PaymentStatus;
        //        Add.Isconfirm = item.IsConfirmed;
        //        Add.saller_id = Convert.ToString(item.sallerid);
        //        obj.Add(Add);
        //    }
        //    ViewBag.dataa = obj;
        //    return View();
        //}
        #endregion

        #region Cotact
        public ActionResult Querylist()
        {
            ViewBag.data = db.tbl_Contact.ToList();
            return View();
        }
        #endregion

        #region addnotification
        public ActionResult addnotification()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addnotification(tbl_Notifcation obj)
        {
            obj.createdate = DateTime.Now;
            db.tbl_Notifcation.Add(obj);
            db.SaveChanges();
            TempData["msg"] = "<script>alert('Added Succefully')</script>";
            return View();
        }

        public ActionResult ViewNotification()
        {
            ViewBag.data = db.tbl_Notifcation.Where(a => a.MessageFor == "Buyers" || a.MessageFor == "Sellers").ToList();
            return View();
        }
        public ActionResult Deletenotification(int id)
        {
            db.tbl_Notifcation.Remove(db.tbl_Notifcation.Find(id));
            db.SaveChanges();
            TempData["msg"] = "<script>alert('Deleted Succefully')</script>";
            return RedirectToAction("ViewNotification");
        }
        #endregion

        #region Manage Discount
        public ActionResult ChooseAppyOn()
        {
            return View();
        }

        public ActionResult AddDiscount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDiscount(int Appyon)
        {

            if (Appyon != null)
            {
                List<ProductDto> objj = new List<ProductDto>();
                if (Appyon == 0)
                {
                    foreach (var item1 in db.Products.Where(a => a.UpdateBy == "Admin" && a.DiscountType == null).ToList())
                    {
                        ProductDto ob = new ProductDto()
                        {
                            ID = item1.ID,
                            Name = item1.Name,
                            SelleroradminId = item1.SelleroradminId,
                            Price = item1.Price,
                            Discount = item1.Discount,
                            Unit = item1.Unit,
                            GST = item1.GST,
                            BrandName = db.Brands.Where(a => a.ID == item1.BrandID).FirstOrDefault().BrandName,
                            CategoryName = db.Categories.Where(a => a.ID == item1.categoryId).FirstOrDefault().Name,
                            Subname = db.subcategories.Where(a => a.id == item1.subcatid).FirstOrDefault().subcategorname,
                            UpdateBy = item1.UpdateBy
                        };
                        objj.Add(ob);
                    }
                    ViewBag.data = objj;
                    return View();
                }

                int id = Convert.ToInt32(Appyon);
                foreach (var item1 in db.Products.Where(a => a.SelleroradminId == id && a.DiscountType == null).ToList())
                {
                    ProductDto ob = new ProductDto()
                    {
                        ID = item1.ID,
                        Name = item1.Name,
                        SelleroradminId = item1.SelleroradminId,
                        Price = item1.Price,
                        Discount = item1.Discount,
                        Unit = item1.Unit,
                        GST = item1.GST,
                        BrandName = db.Brands.Where(a => a.ID == item1.BrandID).FirstOrDefault().BrandName,
                        CategoryName = db.Categories.Where(a => a.ID == item1.categoryId).FirstOrDefault().Name,
                        Subname = db.subcategories.Where(a => a.id == item1.subcatid).FirstOrDefault().subcategorname,
                        UpdateBy = item1.UpdateBy
                    };
                    objj.Add(ob);
                }
                ViewBag.data = objj;
                return View();
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddFlatOrCouponDiscount(ADDDiscountDTO obj)
        {
            string type = "";
            if (obj.DiscountType == "Coupon")
            {
                type = "Coupon";
            }
            else if (obj.DiscountType == "Flat")
            {
                type = "Flat";
            }

            if (obj.Product_IDS != null)
            {
                foreach (var item in obj.Product_IDS)
                {
                    var products = db.Products.Where(a => a.ID == item).FirstOrDefault();
                    products.Fromdate = obj.fromdate;
                    products.Todate = obj.todate;
                    products.ApplyDiscount = obj.Discount;
                    if (obj.CouponCode != "")
                    {
                        products.Copon = obj.CouponCode;
                    }
                    products.DiscountType = type;
                    products.AverageOrPercantage = obj.averageOrPercentage;
                    db.Entry<Product>(products).State = System.Data.EntityState.Modified;
                    db.SaveChanges();

                }
                TempData["Dashboardmessage"] = "<script>alert('Flat Discount Added')</script>";
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        public ActionResult ViewAllDiscount()
        {
            if (Session["adminid"] == null)
            {
                return RedirectToAction("Index");
            }

            List<ProductDto> obj = new List<ProductDto>();
            var Prodataaa = db.Products.OrderByDescending(a => a.ID).Where(a => a.Fromdate != null && a.Todate != null && a.ApplyDiscount != null && a.DiscountType != null && a.AverageOrPercantage != null).ToList();
            foreach (var item in Prodataaa)
            {
                ProductDto Add = new ProductDto();
                if (db.Categories.Any(a => a.ID == item.categoryId))
                {
                    Add.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                }
                if (db.subcategories.Any(a => a.id == item.subcatid))
                {
                    Add.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                }
                if (db.Brands.Any(a => a.ID == item.BrandID))
                {
                    Add.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                }

                Add.Description = item.Description;
                Add.Discount = item.Discount;
                Add.Unit = item.Unit;
                Add.GST = item.GST;
                Add.UpdateBy = item.UpdateBy;
                Add.SelleroradminId = item.SelleroradminId;
                Add.ID = item.ID;
                Add.Sponser = item.Sponserd;
                Add.Name = item.Name;
                Add.active = item.active;
                Add.Price = item.Price;
                Add.ApplyDiscount = Convert.ToInt32(item.ApplyDiscount);
                Add.DiscountType = item.DiscountType;
                Add.Fromdate = Convert.ToDateTime(item.Fromdate);
                Add.Todate = Convert.ToDateTime(item.Todate);
                Add.AverageOrPercentage = item.AverageOrPercantage;
                if (db.ProductImages.Any(a => a.ProductId == Add.ID))
                {
                    Add.Image = db.ProductImages.Where(a => a.ProductId == Add.ID).FirstOrDefault().Images;
                }
                obj.Add(Add);
            }
            ViewBag.data = obj;
            return View();

        }


        public ActionResult RemoveAddedDiscount(int Id)
        {
            var data = db.Products.Where(a => a.ID == Id).FirstOrDefault();
            if (data != null)
            {
                data.Fromdate = null;
                data.Todate = null;
                data.ApplyDiscount = null;
                data.DiscountType = null;
                data.AverageOrPercantage = null;
                data.Copon = null;
                db.Entry<Product>(data).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                TempData["Dashboardmessage"] = "<script>alert('Removed SuccessFully')</script>";
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        public ActionResult EditAddedDiscounts(int Id)
        {
            var data = db.Products.Where(a => a.ID == Id).FirstOrDefault();
            if (data != null)
            {
                ViewBag.data = data;
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditAddedDiscounts(ADDDiscountDTO obj)
        {
            var data = db.Products.Where(a => a.ID == obj.ID).FirstOrDefault();
            if (data != null)
            {
                data.Fromdate = obj.fromdate;
                data.Todate = obj.todate;
                data.ApplyDiscount = obj.Discount;
                data.DiscountType = obj.DiscountType;
                data.AverageOrPercantage = obj.averageOrPercentage;
                db.Entry<Product>(data).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                TempData["Dashboardmessage"] = "<script>alert('Discount Edit SuccessFully')</script>";
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }


        public ActionResult CreateCoupon(ADDDiscountDTO obj)
        {
            return View();
        }

        #endregion

        #region Manage Delivery Charges
        public ActionResult AddDeliveryCharges()
        {


            return View();
        }
        [HttpPost]
        public ActionResult AddDeliveryCharges(Tbl_DeliveryCharge obj)
        {
            if (db.Tbl_DeliveryCharge.Any(a => a.Courier_Type == obj.Courier_Type && a.Charge == obj.Charge && a.From_Weight == obj.From_Weight && a.To_Weight == obj.To_Weight))
            {
                TempData["msg"] = "<script>alert('Already Exist !!')</script>";

                return View();
            }
            db.Tbl_DeliveryCharge.Add(obj);
            db.SaveChanges();
            return View();
        }
        public ActionResult DeleteDeliveryCharge(Tbl_DeliveryCharge obj)
        {
            db.Tbl_DeliveryCharge.Remove(db.Tbl_DeliveryCharge.Where(a => a.ID == obj.ID).FirstOrDefault());
            db.SaveChanges();
            return RedirectToAction("AddDeliveryCharges");
        }
        #endregion

        #region SellerConditions
        public ActionResult minOrderofsellers()
        {
            return View();
        }
        [HttpPost]
        public ActionResult minOrderofsellers(MinOrder_For_sallers obj)
        {
            if (db.MinOrder_For_sallers.Any(a => a.sellerid == obj.sellerid))
            {
                TempData["Dashboardmessage"] = "<script>alert('Already Exist!!')</script>";
                return RedirectToAction("Dashboard");
            }
            obj.craetedate = DateTime.Now;
            obj.UpdateBy = "Admin";
            obj.Active = true;
            db.MinOrder_For_sallers.Add(obj);
            db.SaveChanges();
            return View();
        }
        #endregion

    }

    public class innerjoinClass
    {
        public int tbl_OrderID { get; set; }
        public int OrderdetailsID { get; set; }
        public string ordernumber { get; set; }
        public string PayMode { get; set; }
        public string DeliveryStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerShippingDetails { get; set; }
        public int OrderTotal { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public int sellerid { get; set; }
    }

}



