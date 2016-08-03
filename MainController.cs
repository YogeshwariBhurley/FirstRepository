using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DAL;
using System.Data;

namespace NewBillingApplication.Controllers
{
    public class MainController : Controller
    {
        BLClass objBl = new BLClass();

        ConnectionClass objColl = new ConnectionClass();

        public static DataTable dtGrid = new DataTable();
        

        // GET: Main


        [HttpGet,ActionName("Index")]
        public ActionResult IndexGet()
        {
            return View();
        }

        [HttpPost,ActionName("Index")]
        public ActionResult IndexPost(FormCollection frm)
        {
            if (frm["Command"] == "New Bill")
            {
               PropertyClass objProp = new PropertyClass();
               objProp.Datetime = DateTime.Now;
               objProp.BillNo= 1 + objColl.GetNewBillId();
                dtGrid.Clear();
                return View(objProp);
             }

            if (frm["Command"] == "AddToGrid")
            {

              PropertyClass objBE = new PropertyClass();
                objBE.Datetime = Convert.ToDateTime(frm["DateTime"].ToString());
                objBE.BillNo = Convert.ToInt32(frm["BillNo"]);
                objBE.CustomerName = frm["CustomerName"].ToString();
                objBE.MobNo = frm["MobNo"].ToString();

                int ProductID = Convert.ToInt32(frm["ProductID"]);
                string ProductName = frm["ProductName"].ToString();
                int Quantity = Convert.ToInt32(frm["Qty"]);
                string UnitName = frm["UnitName"].ToString();
                decimal Price = Convert.ToDecimal(frm["Price"]);
                decimal Total = Convert.ToDecimal(frm["txtNetAmount"]);

                if (dtGrid.Columns.Count < 1)
                {
                    dtGrid.Columns.Add("ProductID", typeof(int));
                    dtGrid.Columns.Add("ProductName", typeof(string));
                    dtGrid.Columns.Add("Price", typeof(decimal));
                    dtGrid.Columns.Add("Quantity", typeof(int));
                    dtGrid.Columns.Add("UnitName", typeof(string));
                    dtGrid.Columns.Add("Total", typeof(decimal));
                }

                dtGrid.Rows.Add(ProductID, ProductName, Price, Quantity, UnitName, Total);
                ViewData["data"] = dtGrid;

                return View(objBE);


            }

            if (frm["Command"] == "Update")
            {
               PropertyClass objBE = new PropertyClass();
                objBE.Datetime= Convert.ToDateTime(frm["Datetime"].ToString());
                objBE.BillNo = Convert.ToInt32(frm["BillNo"]);
                objBE.CustomerName = frm["CustomerName"].ToString();
                objBE.MobNo = frm["MobNo"].ToString();

                int EditProductId = Convert.ToInt32(frm["txtEditProductId"]);
                string EditProductName = frm["txtEditProductName"].ToString();
                int EditQuantity = Convert.ToInt32(frm["txtEditQuantity"]);
                string EditUnitName = frm["txtEditUnitName"].ToString();
                decimal EditPrice = Convert.ToDecimal(frm["txtEditPrice"]);
                decimal EditTotal = Convert.ToDecimal(frm["txtEditAmount"]);

                decimal TotalPrice = EditQuantity * EditPrice;
                for (int i = dtGrid.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtGrid.Rows[i];
                    if (Convert.ToInt32(dr["ProductId"]) == EditProductId)
                    {
                        dr["ProductName"] = EditProductName;
                        dr["Quantity"] = EditQuantity;
                        dr["Price"] = EditPrice;
                        dr["UnitName"] = EditUnitName;
                        dr["Total"] = TotalPrice;
                    }
                }

                ViewData["data"] = dtGrid;


                return View(objBE);
            }

            if (frm["Command"] == "Save")
            {
                PropertyClass objBE = new PropertyClass();
                objBE.Datetime = Convert.ToDateTime(frm["Datetime"].ToString());
                objBE.BillNo = Convert.ToInt32(frm["BillNo"]);
                objBE.CustomerName = frm["CustomerName"].ToString();
                objBE.MobNo = frm["MobNo"].ToString();

                int flag = 0;
                List<string> list = new List<string>();
                list.Add(frm["CustomerName"].ToString());
                list.Add(frm["MobNo"].ToString());
                list.Add(frm["Datetime"].ToString()); ;
                list.Add(frm["txtNetTotal"].ToString());

                flag = objColl.CreateBill(list);
                flag = objColl.CreateReceipt(dtGrid, Convert.ToInt32(frm["BillNo"].ToString()));


                if (flag > 0)
                {
                     ViewData["data"] = dtGrid;
                    objBE.Status = "Your Record is Successfully Saved";
                    return View(objBE);
                }
            }


            if (frm["Command"] == "Print")
            {
               PropertyClass objBE = new PropertyClass ();
                objBE.Datetime= Convert.ToDateTime(frm["DateTime"].ToString());
                objBE.BillNo = Convert.ToInt32(frm["BillNo"]);
                objBE.CustomerName = frm["CustomerName"].ToString();
                objBE.MobNo = frm["MobNo"].ToString();


                ViewData["data"] = dtGrid;

                return View(objBE);

            }

            if (frm["Command"] == "Delete")
            {
                PropertyClass objBE = new PropertyClass();
                objBE.Datetime = Convert.ToDateTime(frm["Datetime"].ToString());
                objBE.BillNo = Convert.ToInt32(frm["BillNo"]);
                objBE.CustomerName = frm["CustomerName"].ToString();
                objBE.MobNo= frm["MobNo"].ToString();

                int deleteId = Convert.ToInt32(frm["txtDeleteValue"]);
                Response.Write(deleteId);
                //foreach (DataRow dr in dtGrid.Rows)
                //{
                //    if( Convert.ToInt32(dr["ProductId"]) == deleteId)
                //        dr.Delete();
                //}

                for (int i = dtGrid.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtGrid.Rows[i];
                    if (Convert.ToInt32(dr["ProductId"]) == deleteId)
                        dr.Delete();
                }

                ViewData["data"] = dtGrid;


                return View(objBE);
            }
            return View();
        }


        public JsonResult GetProductDataById(int ProductID)
        {
            try
            {

                
                DataTable dt = objColl.SelectDetailsById("sp_GetProductDetailsById",ProductID);
                string _ProductName = dt.Rows[0]["ProductName"].ToString();
                string _Price = dt.Rows[0]["Price"].ToString();
                string _UnitName = dt.Rows[0]["unitname"].ToString();

                return Json(new { ok = true, ProductName = _ProductName, Price = _Price, UnitName = _UnitName, message = "ok" });
            }
            catch (Exception ex)
            {
                return Json(new { ok = true, message = ex.Message });
            }

        }

        public JsonResult GetProductDataByName(string id)
        {
            try
            {


                DataTable dt = objColl.SelectDetailsByName("sp_GetProductDetailsByName", id);
                string _ProductId = dt.Rows[0]["ProductID"].ToString();
                string _Price = dt.Rows[0]["Price"].ToString();
                string _UnitName = dt.Rows[0]["unitname"].ToString();

                //List<string > prod= new List<string>();




                return Json(new { ok = true, ProductID = _ProductId, Price = _Price, UnitName = _UnitName, message = "ok" });
            }
            catch (Exception ex)
            {
                return Json(new { ok = true, message = ex.Message });
            }

        }
            
    }
}