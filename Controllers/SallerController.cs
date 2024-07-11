using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wanc.Models;
using Wanc;
using System.Text;

namespace Wanc.Controllers
{
    public class SallerController : Controller
    {
        //
        // GET: /Saller/
        wanc_dbEntities db = new wanc_dbEntities();

        public ActionResult Index()
        {
            return View();
        }

        #region Dashboard logout
        public ActionResult Dashboard()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            return View();
        }
        public ActionResult SallerLogin()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SallerLogin(string logingid, string pwd)
        {
            var obj = db.sallerDetails.Where(x => x.Salerusername == logingid && x.sallerpass == pwd || x.SallerEmailId == logingid && x.sallerpass == pwd).FirstOrDefault();
            if (obj != null)
            {
                Session["Sallername"] = obj.FirstName;
                Session["SallerID"] = obj.sallerId;
                return RedirectToAction("Dashboard", "Saller");
            }
            else
            {
                TempData["msg"] = "Invalid Id Or Password";
                return RedirectToAction("SallerLogin", "Saller");

            }
        }
        public ActionResult ChangePassword()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string oldpassword, string Password, string Confirm)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            if (oldpassword != db.sallerDetails.FirstOrDefault().sallerpass)
            {
                TempData["MSG"] = "old Password Do not Match!!!";
                return RedirectToAction("ChangePassword", "Saller");

            }
            if (Password != Confirm)
            {
                TempData["MSG"] = "Passwords Do not Match or password length is too small..!!Minimum Length is 8.";
                return RedirectToAction("ChangePassword", "Saller");
            }
            db.sallerDetails.FirstOrDefault().sallerpass = Password;
            db.SaveChanges();
            TempData["MSG"] = "Successfully updated Your Password . Please Login With Your UserId And New Password...";
            return RedirectToAction("SallerLogin", "Saller");
        }
        public ActionResult Logout()
        {
            if (Session["SallerID"] != null)
            {
                Session["SallerID"] = null;
                return RedirectToAction("SallerLogin", "Saller");
            }
            return RedirectToAction("SallerLogin", "Saller");
        }
        #endregion

        #region  Brand
        public ActionResult AddNewBrand()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {
                ViewBag.pin = db.tbl_pincode.ToList();
                return View();
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
        }
        [HttpPost]
        public ActionResult AddNewBrand(BrandDto obj, HttpPostedFileBase file)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            int SellerId = Convert.ToInt32(Session["SallerID"]);
            Brand add = new Brand()
            {
                UpdateBy = "Saller",
                SellereoradminId = SellerId,
                BrandName = obj.BrandName,
                //Images = FileUploader.UploadImage(obj.ImagesFile, "BrandImage")
                Images = file!=null? "/BrandImage/"+ FileUploader.UploadImage(file, "BrandImage"):null,
            };
            db.Brands.Add(add);
            db.SaveChanges();
            TempData["msg"] = "Brand is Uploaded";
            return RedirectToAction("AddNewBrand");
        }
        public ActionResult DelBrand(int Id)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            db.Brands.Remove(db.Brands.Find(Id));
            db.SaveChanges();
            return RedirectToAction("ViewBrand");
        }
        public ActionResult ViewBrand()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {
                int sellerid = Convert.ToInt32(Session["SallerID"]);
                return View(db.Brands.Where(a => a.UpdateBy == "Saller" && a.SellereoradminId == sellerid).ToList());
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
        }

        #endregion

        #region Category

        public ActionResult AddNewCategory()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {
                return View();
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }

        }

        [HttpPost]
        public ActionResult AddNewCategory(CategoryDto obj)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            int SellerId = Convert.ToInt32(Session["SallerID"]);
            Category add = new Category()
            {
                UpdateBy = "Saller",
                SelleroradminId = SellerId,
                Description = obj.Description,
                Name = obj.Name,
                Image = FileUploader.UploadImage(obj.file, "CategoryImage")
            };
            db.Categories.Add(add);
            db.SaveChanges();
            TempData["msg"] = "Category is Added";
            return RedirectToAction("AddNewCategory");
        }

        public ActionResult ViewAllCategory()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }

            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {
                int sellerid = Convert.ToInt32(Session["SallerID"]);
                return View(db.Categories.Where(a => a.UpdateBy == "Saller" && a.SelleroradminId == sellerid).ToList());
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
        }

        public ActionResult DelCategory(int Id)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }

            db.Categories.Remove(db.Categories.Find(Id));
            db.SaveChanges();
            return RedirectToAction("ViewAllCategory");
        }

        public ActionResult Editcategory(int Id)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            var catdata = db.Categories.Where(a => a.ID == Id).FirstOrDefault();
            CategoryDto add = new CategoryDto()
            {
                Image = catdata.Image,
                ID = catdata.ID,
                SelleroradminId = catdata.SelleroradminId,
                UpdateBy = catdata.UpdateBy,
                Description = catdata.Description,
                Name = catdata.Name,
                Parentid = catdata.Parentid
            };

            return View(add);
        }

        [HttpPost]
        public ActionResult Editcategory(CategoryDto obj)
        {
            Category add = new Category();
            add.Description = obj.Description;
            add.Name = obj.Name;
            add.UpdateBy = obj.UpdateBy;
            add.ID = obj.ID;
            add.SelleroradminId = obj.SelleroradminId;
            add.Parentid = obj.Parentid;
            if (obj.Image != null)
            {
                add.Image = obj.Image;
            }
            if (obj.file != null)
            {
                add.Image = FileUploader.UploadImage(obj.file, "CategoryImage");
            }
            db.Entry<Category>(add).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewAllCategory", "Saller");
        }

        #endregion

        #region SubCategory

        public ActionResult AddSubCategory()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("Index");
            }
            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {
                ViewBag.Cate = db.Categories.ToList();
                return View();
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
        }

        [HttpPost]
        public ActionResult AddSubCategory(subcategory obj)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("Index");
            }
            int SellerId = Convert.ToInt32(Session["SallerID"]);
            obj.UpdateBy = "Saller";
            obj.SelleroradminId = SellerId;
            db.subcategories.Add(obj);
            db.SaveChanges();
            TempData["msg"] = "Subcategory is Added";
            return RedirectToAction("AddSubCategory");
        }

        public ActionResult ViewSubCategory()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("Index");
            }

            int sellerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sellerid).FirstOrDefault();
            if (data.Active == true)
            {
                List<SubcategoryDto> obj = new List<SubcategoryDto>();
                var subcatdata = db.subcategories.Where(a => a.UpdateBy == "Saller" && a.SelleroradminId == sellerid).ToList();
                foreach (var item in subcatdata)
                {
                    SubcategoryDto Add = new SubcategoryDto();
                    Add.catid = item.catid;
                    Add.id = item.id;
                    Add.UpdateBy = item.UpdateBy;
                    Add.subcategorname = item.subcategorname;
                    Add.ParentCategoryName = db.Categories.Where(a => a.ID == item.catid).FirstOrDefault().Name.ToString();
                    obj.Add(Add);
                }

                return View(obj);
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
        }

        public ActionResult Deletesubcategory(int Id)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("Index");
            }

            db.subcategories.Remove(db.subcategories.Find(Id));
            db.SaveChanges();
            return RedirectToAction("ViewSubCategory");
        }

        public ActionResult EditSubcategory(int Id)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("Index");
            }

            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {
                ViewBag.Cate = db.Categories.ToList();
                var userdata = db.subcategories.Where(a => a.id == Id).FirstOrDefault();
                SubcategoryDto add = new SubcategoryDto()
                {
                    catid = userdata.catid,
                    id = userdata.id,
                    SelleroradminId = userdata.SelleroradminId,
                    subcategorname = userdata.subcategorname,
                    ParentCategoryName = userdata.subcategorname,
                    UpdateBy = userdata.UpdateBy,
                };

                return View(add);
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
        }

        [HttpPost]
        public ActionResult EditSubcategory(SubcategoryDto obj)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("Index");
            }
            subcategory add = new subcategory()
            {
                id = obj.id,
                catid = obj.catid,
                subcategorname = obj.subcategorname,
                SelleroradminId = obj.SelleroradminId,
                UpdateBy = obj.UpdateBy
            };
            db.Entry<subcategory>(add).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewSubCategory", "Saller");

        }

        #endregion

        #region Product

        public ActionResult AddnewProduct()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {
                ViewBag.pin = db.tbl_pincode.ToList();
                ViewBag.Brand = db.Brands.ToList();
                ViewBag.Cate = db.Categories.ToList();
                ViewBag.subcate = db.subcategories.ToList();
                return View();
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
                }

        [HttpPost]
        public ActionResult AddnewProduct(ProductDto obj, string Wght, string[] Pincode, string[] Heading, string[] Para, string[] SHeading, string[] SPara, HttpPostedFileBase file)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            StringBuilder pincodee = new StringBuilder();
            for (int i = 0; i < Pincode.Length; i++)
            {
                pincodee.Append("," + Pincode[i]);
            }

            int SellerId = Convert.ToInt32(Session["SallerID"]);
            Product Add = new Product();
            Add.BrandID = obj.BrandID;
            Add.Type = "New";
            Add.SelleroradminId = SellerId;
            Add.pincode = pincodee.ToString();
            Add.UpdateBy = "Saller" + Session["Sallername"].ToString();
            Add.categoryId = obj.categoryId;
            Add.Description = obj.Description;
            Add.Discount = obj.Discount;
            Add.Specification = obj.Specification;
            Add.Dimentions = obj.Dimensions;
            Add.GST = obj.GST;
            Add.Name = obj.Name;
            Add.Price = obj.Price;
            Add.createDate = DateTime.Now;
            Add.subcatid = obj.subcatid;
            Add.ApprovedByAdmin = false;
            Add.Brandname = db.Brands.Where(a => a.ID == obj.BrandID).FirstOrDefault().BrandName;
            Add.Unit = obj.Unit;
            Add.active = false;
            if (Wght == "g")
            {
                Add.Weight = obj.Weight;
            }
            else if (Wght == "kg")
            {
                decimal wt = Convert.ToDecimal(obj.Weight) * 1000;
                Add.Weight = wt.ToString();
            }
            if (obj.color != null)
            {
                StringBuilder Color = new StringBuilder();
                for (int i = 0; i < obj.color.Length; i++)
                {
                    Color.Append(obj.color[i] + ",");
                }
                Add.productColor = Color.ToString();
            }
            if (file != null)
            {
                ProductImage Add1 = new ProductImage();
                Add1.Images = file != null ? "/Products/" + FileUploader.UploadImage(file, "Products") : null;
                ViewBag.image = Add1.Images;
                Add1.ProductId = Add.ID;
                db.ProductImages.Add(Add1);
                db.SaveChanges();
            }
            Add.Image = ViewBag.image;
            db.Products.Add(Add);
            db.SaveChanges();
            if (Heading != null && Para != null)
            {
                foreach (var item in Heading)
                {
                    foreach (var item1 in Para)
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
            if (SHeading != null && SPara != null)
            {
                foreach (var item in SHeading)
                {
                    foreach (var item1 in SPara)
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
            TempData["MSG"] = "Product add successfully";
            return RedirectToAction("AddnewProduct", "Saller");
        }

        public ActionResult EditProducts(int Id)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }

            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {

                var userdata = db.Products.Where(a => a.ID == Id).FirstOrDefault();
                ViewBag.Brand = db.Brands.ToList();
                ViewBag.Cate = db.Categories.ToList();
                ViewBag.subcate = db.subcategories.ToList();
                ProductDto obj = new ProductDto()
                {
                    BrandID = userdata.BrandID,
                    ID = userdata.ID,
                    cashback = userdata.cashback,
                    UpdateBy = userdata.UpdateBy,
                    BrandName = db.Products.Where(a => a.BrandID == userdata.BrandID).FirstOrDefault().Name,
                    Cashbacktype = userdata.Cashbacktype,
                    Name = userdata.Name,
                    categoryId = userdata.categoryId,
                    CategoryName = db.Categories.Where(a => a.ID == userdata.categoryId).FirstOrDefault().Name,
                    Description = userdata.Description,
                    Discount = userdata.Discount,
                    Image = db.ProductImages.Where(a => a.ProductId == userdata.ID).FirstOrDefault().Images,
                    ImageId = db.ProductImages.Where(a => a.ProductId == userdata.ID).FirstOrDefault().id,
                    Price = userdata.Price,
                    subcatid = userdata.subcatid,
                    SelleroradminId = userdata.SelleroradminId,
                    Subname = db.subcategories.Where(a => a.id == userdata.subcatid).FirstOrDefault().subcategorname,
                    Unit = userdata.Unit
                };
                return View(obj);

            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }

        }

        [HttpPost]
        public ActionResult EditProducts(ProductDto obj, HttpPostedFileBase file)
        {
            Product Add = new Product()
            {
                BrandID = obj.BrandID,
                SelleroradminId = obj.SelleroradminId,
                Unit = obj.Unit,
                cashback = obj.cashback,
                Cashbacktype = obj.Cashbacktype,
                categoryId = obj.categoryId,
                UpdateBy = obj.UpdateBy,
                Description = obj.Description,
                Discount = obj.Discount,
                Name = obj.Name,
                ID = obj.ID,
                Price = obj.Price,
                subcatid = obj.subcatid,
                Image = file!=null ? "/Products/" +FileUploader.UploadImage(file,"Products"):null,
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
            //    Add1.Images = FileUploader.UploadImage(obj.file, "ProductImages");
            //}
            //Add1.ProductId = obj.ImageId;
            //return RedirectToAction("ViewAllProduct");
            Product Add1 = new Product();
            Add1.ID = obj.ID;
            if (obj.Image != null)
            {
                Add1.Image = obj.Image;
            }
            if (obj.file != null)
            {
                Add1.Image = FileUploader.UploadImage(obj.file, "Products");
            }
            Add1.ID = Convert.ToInt32(obj.ImageId);
            //Add1.ProductId = obj.ImageId;
            db.Entry<Product>(Add1).State = System.Data.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ViewAllProduct");
        }

        public ActionResult ViewAllProduct(string active)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }


            int sallerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sallerid).FirstOrDefault();
            if (data.Active == true)
            {
                int sellerid = Convert.ToInt32(Session["SallerID"]);
                List<ProductDto> obj = new List<ProductDto>();

                if (active == "blocked")
                {
                    var Prodata1 = db.Products.Where(a => a.UpdateBy != "Admin" && a.active == false && a.SelleroradminId == sellerid).OrderByDescending(a => a.ID).ToList();
                    foreach (var item in Prodata1)
                    {
                        ProductDto Add = new ProductDto();
                        Add.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                        Add.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                        Add.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                        Add.Description = item.Description;
                        Add.Discount = item.Discount;
                        Add.Unit = item.Unit;
                        Add.UpdateBy = item.UpdateBy;
                        Add.ID = item.ID;
                        Add.GST = item.GST;
                        Add.Name = item.Name;
                        Add.Price = item.Price;
                        Add.UpdateBy = item.UpdateBy;
                        Add.sallerID = db.sallerDetails.Where(a => a.sallerId == item.SelleroradminId).FirstOrDefault().Salerusername;
                        Add.active = item.active;
                        Add.ApprovedByadmin = Convert.ToBoolean(item.ApprovedByAdmin);
                        Add.Image = db.ProductImages.Where(a => a.ProductId == Add.ID).FirstOrDefault().Images;
                        obj.Add(Add);
                    }

                    return View(obj);
                }
                if (active == "Sponsered")
                {
                    var Prodata1 = db.Products.Where(a => a.Sponserd == true && a.SelleroradminId == sellerid).OrderByDescending(a => a.ID).ToList();
                    foreach (var item in Prodata1)
                    {
                        ProductDto Add = new ProductDto();
                        Add.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                        Add.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                        Add.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                        Add.Description = item.Description;
                        Add.Discount = item.Discount;
                        Add.Unit = item.Unit;
                        Add.UpdateBy = item.UpdateBy;
                        Add.ID = item.ID;
                        Add.GST = item.GST;
                        Add.Name = item.Name;
                        Add.Price = item.Price;
                        Add.UpdateBy = item.UpdateBy;
                        Add.sallerID = db.sallerDetails.Where(a => a.sallerId == item.SelleroradminId).FirstOrDefault().Salerusername;
                        Add.active = item.active;
                        Add.ApprovedByadmin = Convert.ToBoolean(item.ApprovedByAdmin);
                        Add.Image = db.ProductImages.Where(a => a.ProductId == Add.ID).FirstOrDefault().Images;
                        obj.Add(Add);
                    }

                    return View(obj);
                }

                if (active == "Approved")
                {
                    var Prodata1 = db.Products.Where(a => a.ApprovedByAdmin == true && a.SelleroradminId == sellerid).OrderByDescending(a => a.ID).ToList();
                    foreach (var item in Prodata1)
                    {
                        ProductDto Add = new ProductDto();
                        Add.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                        Add.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                        Add.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                        Add.Description = item.Description;
                        Add.Discount = item.Discount;
                        Add.Unit = item.Unit;
                        Add.UpdateBy = item.UpdateBy;
                        Add.ID = item.ID;
                        Add.GST = item.GST;
                        Add.Name = item.Name;
                        Add.Price = item.Price;
                        Add.UpdateBy = item.UpdateBy;
                        Add.sallerID = db.sallerDetails.Where(a => a.sallerId == item.SelleroradminId).FirstOrDefault().Salerusername;
                        Add.active = item.active;
                        Add.ApprovedByadmin = Convert.ToBoolean(item.ApprovedByAdmin);
                        Add.Image = db.ProductImages.Where(a => a.ProductId == Add.ID).FirstOrDefault().Images;
                        obj.Add(Add);
                    }

                    return View(obj);
                }



                var Prodata = db.Products.Where(a => a.UpdateBy != "Admin" && a.SelleroradminId == sellerid).OrderByDescending(a => a.ID).ToList();
                foreach (var item in Prodata)
                {
                    ProductDto Add = new ProductDto();
                    Add.CategoryName = db.Categories.Where(a => a.ID == item.categoryId).FirstOrDefault().Name;
                    Add.Subname = db.subcategories.Where(a => a.id == item.subcatid).FirstOrDefault().subcategorname;
                    Add.BrandName = db.Brands.Where(a => a.ID == item.BrandID).FirstOrDefault().BrandName;
                    Add.Description = item.Description;
                    Add.Discount = item.Discount;
                    Add.Unit = item.Unit;
                    Add.UpdateBy = item.UpdateBy;
                    Add.ID = item.ID;
                    //int n = item.ID;
                    Add.GST = item.GST;
                    Add.Name = item.Name;
                    Add.Price = item.Price;
                    Add.UpdateBy = item.UpdateBy;
                    Add.sallerID = db.sallerDetails.Where(a => a.sallerId == item.SelleroradminId).FirstOrDefault().Salerusername;
                    Add.active = item.active;
                    Add.ApprovedByadmin = Convert.ToBoolean(item.ApprovedByAdmin);
                    Add.Image = item.Image;
                    //Add.Image = db.ProductImages.Where(a => a.ProductId == n).FirstOrDefault().Images;
                    obj.Add(Add);
                }

                return View(obj);
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }

        }

        public ActionResult DeleteProduct(int Id)
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }

            int SellerId = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == SellerId).FirstOrDefault();
            if (data.Active == true)
            {
                db.Products.Remove(db.Products.Find(Id));
                db.SaveChanges();
                return RedirectToAction("ViewAllProduct");
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
        }
        public JsonResult getsubcategory(int categoryId)
        {
            var data = db.subcategories.Where(a => a.catid == categoryId).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ViewAllOrder(string Type)
        {
            int idd = Convert.ToInt32(Session["SallerID"]);
            List<innerjoinClass> ob = new List<innerjoinClass>();
            var orders = db.tbl_Order.ToList();
            var orderDetails = db.OrderDetails.Where(x=>x.sallerid == idd).ToList();
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
                        sellerid = (int)OrderDetail.sallerid

                    };
                    ob.Add(abc);
                }
            }
            return View(ob);
            //if (Session["SallerID"] == null)
            //{
            //    return RedirectToAction("SallerLogin", "Saller");
            //}
            //TempData["title"] = null;
            //int sellerid = Convert.ToInt32(Session["SallerID"]);
            //List<OrderDetailDTO> obj = new List<OrderDetailDTO>();

            //string Type_ = "";
            //if (Type == "UNDER_PROCESS")
            //{
            //    Type_ = "UNDER_PROCESS";
            //    TempData["title"] = "UNDER_PROCESS";
            //}
            //if (Type == "AWATING_PICKUP")
            //{
            //    Type_ = "AWATING_PICKUP";
            //    TempData["title"] = "AWATING_PICKUP";
            //}

            //else if (Type == "SHIPPED")
            //{
            //    Type_ = "SHIPPED";
            //    TempData["title"] = "SHIPPED";
            //}

            //else if (Type == "ORDER_TRANSIT")
            //{
            //    Type_ = "ORDER_TRANSIT";
            //    TempData["title"] = "ORDER_TRANSIT";
            //}
            //else if (Type == "DELIVERED")
            //{
            //    Type_ = "DELIVERED";
            //    TempData["title"] = "DELIVERED";
            //}
            //if (Type != null && Type_ != "")
            //{

            //    foreach (var item1 in db.tbl_Order.OrderByDescending(a => a.ID).ToList())
            //    {
            //        foreach (var item in db.OrderDetails.OrderByDescending(a => a.ID).Where(a => a.OrderId == item1.ID && a.sallerid == sellerid && a.DeliveryStatus == Type_ && a.IsConfirmed == true).ToList())
            //        {
            //            OrderDetailDTO Add = new OrderDetailDTO();
            //            var distype = db.Products.Where(a => a.ID == item.ProductId).FirstOrDefault();
            //            if (distype.DiscountType != null)
            //            {
            //                Add.CoupenOrFlatDiscount = distype.DiscountType;
            //            }

            //            Add.Quantity = item.Quantity;
            //            Add.ID = item.ID;
            //            Add.ProductId = item.ProductId;
            //            Add.ProductName = item.ProductName;
            //            Add.OrderIDn = item.Product_ORDERID;
            //            Add.Amount = Convert.ToDecimal(item.Amount);
            //            Add.PaymentMethod = item1.paymentMethod;
            //            Add.CreateDate = item.OrderDate;
            //            Add.PaymentStatus = item1.PaymentStatus;
            //            Add.ClientName = item1.Name;
            //            Add.DeliveryCharge = Convert.ToDouble(item.DeliveryCharge);
            //            Add.Isconfirm = item.IsConfirmed;
            //            Add.DeliveryStatus = item.DeliveryStatus;
            //            Add.Price = item.Price;
            //            Add.Discount = item.Discount;
            //            var BuyerData = db.tbl_user.Where(a => a.UserId == item.Clientid).FirstOrDefault();
            //            if (BuyerData != null)
            //            {
            //                Add.BuyerName = BuyerData.full_name;
            //                Add.BuyerAddress = BuyerData.ShippingAddress;
            //            }

            //            obj.Add(Add);
            //        }
            //    }
            //    return View(obj);
            //}


            //foreach (var item1 in db.tbl_Order.OrderByDescending(a => a.ID).ToList())
            //{

            //    foreach (var item in db.OrderDetails.OrderByDescending(a => a.ID).Where(a => a.OrderId == item1.ID && a.sallerid == sellerid && a.DeliveryStatus == "PENDING").ToList())
            //    {
            //        OrderDetailDTO Add = new OrderDetailDTO();
            //        var distype = db.Products.Where(a => a.ID == item.ProductId).FirstOrDefault();
            //        if (distype.DiscountType != null)
            //        {
            //            Add.CoupenOrFlatDiscount = distype.DiscountType;
            //        }

            //        Add.Quantity = item.Quantity;
            //        Add.ID = item.ID;
            //        Add.ProductId = item.ProductId;
            //        Add.ProductName = item.ProductName;
            //        Add.OrderIDn = item.Product_ORDERID;
            //        Add.Amount = Convert.ToDecimal(item.Amount);
            //        Add.PaymentMethod = item1.paymentMethod;
            //        Add.CreateDate = item.OrderDate;
            //        Add.PaymentStatus = item1.PaymentStatus;
            //        Add.ClientName = item1.Name;
            //        Add.DeliveryCharge = Convert.ToDouble(item.DeliveryCharge);
            //        Add.Isconfirm = item.IsConfirmed;
            //        Add.DeliveryStatus = item.DeliveryStatus;
            //        Add.Price = item.Price;
            //        Add.Discount = item.Discount;
            //        var BuyerData = db.tbl_user.Where(a => a.UserId == item.Clientid).FirstOrDefault();
            //        if (BuyerData != null)
            //        {
            //            Add.BuyerName = BuyerData.full_name;
            //            Add.BuyerAddress = BuyerData.ShippingAddress;
            //        }


            //        obj.Add(Add);
            //    }

            //}

            //return View(obj);
        }

        //#region Return Management
        //public ActionResult ApprovedReturns(string Type)
        //{

        //    if (Session["SallerID"] == null)
        //    {
        //        return RedirectToAction("SallerLogin", "Saller");
        //    }
        //    TempData["title"] = null;
        //    int sellerid = Convert.ToInt32(Session["SallerID"]);
        //    List<MyReturnProductsDto> ob = new List<MyReturnProductsDto>();

        //    string Type_ = "";
        //    if (Type == "PICKUP_UNDER_PROCESS")
        //    {
        //        Type_ = "PICKUP_UNDER_PROCESS";
        //        TempData["title"] = "PICKUP_UNDER_PROCESS";
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
        //        foreach (var item in db.tbl_Order.ToList())
        //        {
        //            var data = db.OrderDetails.Where(a => a.OrderId == item.ID && a.sallerid == sellerid && a.Isreturned == true && a.ReturnRequest == "APPROVED" && a.DeliveryStatusOfReturn == Type_).ToList();
        //            foreach (var item1 in data)
        //            {
        //                MyReturnProductsDto obj = new MyReturnProductsDto();
        //                obj.ID = item.ID;
        //                obj.Status = item1.DeliveryStatus;
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
        //                obj.TotalAmount = Convert.ToDouble(item1.Amount);
        //                obj.DeliveryCharge = Convert.ToDecimal(item1.DeliveryCharge);
        //                obj.Od_p_status = obj.Status;
        //                obj.OrderIdN = item.OrderId;

        //                obj.DeliveryStatus = item1.DeliveryStatusOfReturn;
        //                obj.isReturnApproved = item1.ReturnRequest;
        //                if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //                {
        //                    obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().full_name;
        //                    obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //                }

        //                ob.Add(obj);
        //            }
        //        }


        //        return View(ob);
        //    }


        //    var Prodata = db.tbl_Order.ToList();
        //    foreach (var item in Prodata)
        //    {
        //        foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.sallerid == sellerid && a.Isreturned == true && a.ReturnRequest == "APPROVED" && a.DeliveryStatusOfReturn != "PICKUP_UNDER_PROCESS" && a.DeliveryStatusOfReturn != "PICKUP_RETURN" && a.DeliveryStatusOfReturn != "PICKUP_SHIPPED" && a.DeliveryStatusOfReturn != "PICKUP_TRANSIT" && a.DeliveryStatusOfReturn != "PICKUP_DELIVERED" && a.DeliveryStatusOfReturn != "RETURN_UNDER_PROCESS" && a.DeliveryStatusOfReturn != "RETURN_PICKUP" && a.DeliveryStatusOfReturn != "RETURN_SHIPPED" && a.DeliveryStatusOfReturn != "RETURN_TRANSIT" && a.DeliveryStatusOfReturn != "RETURN_DELIVERED").ToList())
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
        //            obj.DeliveryCharge = Convert.ToDecimal(item1.DeliveryCharge);
        //            obj.Od_p_status = obj.Status;
        //            obj.OrderIdN = item.OrderId;
        //            obj.DeliveryStatus = item1.DeliveryStatusOfReturn;
        //            obj.isReturnApproved = item1.ReturnRequest;
        //            if (db.tbl_user.Any(a => a.UserId == item.ClientId))
        //            {
        //                obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().full_name;
        //                obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //            }

        //            ob.Add(obj);
        //        }
        //    }


        //    return View(ob);
        //}


        //public ActionResult ReturnProceedOrder(int ID)
        //{
        //    if (Session["SallerID"] == null)
        //    {
        //        return RedirectToAction("SallerLogin", "Saller");
        //    }
        //    TempData["title"] = null;
        //    int sellerid = Convert.ToInt32(Session["SallerID"]);
        //    var sallerdatra = db.OrderDetails.Where(a => a.ID == ID && a.sallerid == sellerid && a.Isreturned == true && a.ReturnRequest == "APPROVED").FirstOrDefault();
        //    if (sallerdatra != null)
        //    {
        //        if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_UNDER_PROCESS")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "PICKUP_RETURN";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }
        //        if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_RETURN")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "PICKUP_SHIPPED";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }
        //        if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_SHIPPED")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "PICKUP_TRANSIT";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }
        //        if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_TRANSIT")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "PICKUP_DELIVERED";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }
        //        if (sallerdatra.DeliveryStatusOfReturn == "PICKUP_DELIVERED")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "RETURN_UNDER_PROCESS";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }
        //        if (sallerdatra.DeliveryStatusOfReturn == "RETURN_UNDER_PROCESS")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "RETURN_PICKUP";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }
        //        if (sallerdatra.DeliveryStatusOfReturn == "RETURN_PICKUP")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "RETURN_SHIPPED";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }
        //        if (sallerdatra.DeliveryStatusOfReturn == "RETURN_SHIPPED")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "RETURN_TRANSIT";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }
        //        if (sallerdatra.DeliveryStatusOfReturn == "RETURN_TRANSIT")
        //        {
        //            sallerdatra.DeliveryStatusOfReturn = "RETURN_DELIVERED";
        //            sallerdatra.Modified = DateTime.Now;
        //            db.Entry<OrderDetail>(sallerdatra).State = System.Data.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Dashboard", "Saller");
        //        }

        //    }

        //    return RedirectToAction("Dashboard", "Admin");
        //}

        //#endregion

        public ActionResult ProceedOrder(int ID)
        {
            int sellerid = Convert.ToInt32(Session["SallerID"]);
            var data = db.sallerDetails.Where(a => a.sallerId == sellerid).FirstOrDefault();
            if (data.Active == true)
            {
                var sallerdatra = db.OrderDetails.Where(a => a.ID == ID).ToList();
                foreach (var item in sallerdatra)
                {
                    if (item.ID == ID)
                    {
                        if (item.DeliveryStatus == "PENDING")
                        {
                            item.DeliveryStatus = "UNDER_PROCESS";
                            db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("ViewAllOrder", "Saller");
                        }
                        if (item.DeliveryStatus == "UNDER_PROCESS")
                        {
                            item.DeliveryStatus = "AWATING_PICKUP";
                            db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("ViewAllOrder", "Saller");
                        }
                        if (item.DeliveryStatus == "AWATING_PICKUP")
                        {
                            item.DeliveryStatus = "SHIPPED";
                            db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("ViewAllOrder", "Saller");
                        }
                        if (item.DeliveryStatus == "SHIPPED")
                        {
                            item.DeliveryStatus = "ORDER_TRANSIT";
                            db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("ViewAllOrder", "Saller");
                        }
                        if (item.DeliveryStatus == "ORDER_TRANSIT")
                        {
                            item.DeliveryStatus = "DELIVERED";
                            db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("ViewAllOrder", "Saller");
                        }

                    }
                }
                return View();
            }
            else
            {
                Session["msgg"] = "<script>alert('Your Id is Not Active!! Contact To Admin')</script>";
                return RedirectToAction("Dashboard", "Saller");
            }
        }
        #endregion
        #region ProceedOrder
        public ActionResult ProceedOrders(int ID)
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
            return RedirectToAction("ViewAllOrder", "Saller");
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

                return RedirectToAction("ViewAllProduct");
            }
            TempData["msgg"] = "<script>alert('Product is not found!!')</script>";
            return RedirectToAction("ViewAllProduct");
        }
        #endregion

        //#region ApprovedRefund
        //public ActionResult ApprovedRefund(string Type)
        //{
        //    if (Session["SallerID"] == null)
        //    {
        //        return RedirectToAction("SallerLogin", "Saller");
        //    }
        //    int sallerid = Convert.ToInt32(Session["SallerID"]);
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
        //    if (Type != null && Type_ != "")
        //    {
        //        foreach (var item in Prodata)
        //        {
        //            foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.sallerid == sallerid && a.IsCancelled == true && a.RefundStatus == Type_).ToList())
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
        //                    obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().full_name;
        //                    obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //                }
        //                ob.Add(obj);
        //            }
        //        }
        //        ViewBag.data = ob;
        //        return View();
        //    }

        //    foreach (var item in Prodata)
        //    {
        //        foreach (var item1 in db.OrderDetails.Where(a => a.OrderId == item.ID && a.sallerid == sallerid && a.IsCancelled == true && a.Refundrequest == null).ToList())
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
        //                obj.Buyer_selfID = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().full_name;
        //                obj.Buyer_address = db.tbl_user.Where(a => a.UserId == item.ClientId).FirstOrDefault().ShippingAddress;
        //            }

        //            ob.Add(obj);
        //        }
        //    }
        //    ViewBag.data = ob;
        //    return View();
        //}
        //#endregion

        #region
        //public ActionResult RefundProceedOrder(int ID)
        //{
        //    if (Session["SallerID"] == null)
        //    {
        //        return RedirectToAction("SallerLogin", "Saller");
        //    }
        //    int sallerid = Convert.ToInt32(Session["SallerID"]);

        //    foreach (var item in db.OrderDetails.Where(a => a.ID == ID && a.sallerid == sallerid).ToList())
        //    {
        //        if (item.ID == ID)
        //        {
        //            if (item.RefundStatus == "PENDING")
        //            {
        //                item.RefundStatus = "PICKUP_UNDER_PROCESS";
        //                db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("Dashboard", "Saller");
        //            }
        //            if (item.RefundStatus == "PICKUP_UNDER_PROCESS")
        //            {
        //                item.RefundStatus = "PICKUP_RETURN";
        //                db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("Dashboard", "Saller");
        //            }
        //            if (item.RefundStatus == "PICKUP_RETURN")
        //            {
        //                item.RefundStatus = "PICKUP_SHIPPED";
        //                db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("Dashboard", "Saller");
        //            }
        //            if (item.RefundStatus == "PICKUP_SHIPPED")
        //            {
        //                item.RefundStatus = "PICKUP_TRANSIT";
        //                db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("Dashboard", "Saller");
        //            }
        //            if (item.RefundStatus == "PICKUP_TRANSIT")
        //            {
        //                item.RefundStatus = "PICKUP_DELIVERED";
        //                db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("Dashboard", "Saller");
        //            }
        //            if (item.DeliveryStatus == "PICKUP_DELIVERED")
        //            {
        //                item.DeliveryStatus = "PROCEED_REFUND";
        //                db.Entry<OrderDetail>(item).State = System.Data.EntityState.Modified;
        //                db.SaveChanges();
        //                return RedirectToAction("Dashboard", "Saller");
        //            }

        //        }
        //    }
        //    return View();
        //}
        #endregion

        #region SellerConditions
        public ActionResult minOrderofsellers1()
        {
            if (Session["SallerID"] == null)
            {
                return RedirectToAction("SallerLogin", "Saller");
            }
            return View();
        }
        [HttpPost]
        public ActionResult minOrderofsellers1(MinOrder_For_sallers obj)
        {
            if (db.MinOrder_For_sallers.Any(a => a.sellerid == obj.sellerid))
            {
                TempData["msg"] = "<script>alert('Already Exist!!')</script>";
            }
            else
            {
                obj.craetedate = DateTime.Now;
                obj.UpdateBy = "Admin";
                obj.Active = true;
                db.MinOrder_For_sallers.Add(obj);
                db.SaveChanges();
            }

            return RedirectToAction("minOrderofsellers1", "Saller");
        }
        #endregion
    }

}

