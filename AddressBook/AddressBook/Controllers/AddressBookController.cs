using AddressBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AddressBook.DataAccessLayer;
using System.Configuration;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using System.Net;
using Twilio.TwiML;

namespace AddressBook.Controllers
{
    public class AddressBookController : Controller
    {
        // GET: /AddressBook/AddAddressBook
        public ActionResult AddEditAddressBook()
        {
            return View(new AddressBookModel());
        }


        //
        // POST: /AddressBook/AddAddressBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEditAddressBook(AddressBookModel model)
        {
            if (ModelState.IsValid) //checking model is valid or not  
            {
                DbData objDB = new DbData(); //calling class DBdata  
                string result = string.Empty;
                if (model.ID.HasValue)
                {
                    result = objDB.UpdateData(model); // passing Value to DBClass from model  
                }
                else
                {
                    result = objDB.InsertData(model);
                }
                ViewData["result"] = result;
                ModelState.Clear(); //clearing model  
                return RedirectToActionPermanent("ManageAddressBook");
                // return View();
            }
            else
            {
                // ModelState.AddModelError("", "Error in saving data");
                return View(new AddressBookModel());
            }
        }

        //GET : EditAddressBook
        public ActionResult EditAddressBook(int Id)
        {
            DbData objDB = new DbData(); //calling class DBdata  
            var result = objDB.GetAddressBookDataByID(Id); // get values for editing

            return View("AddEditAddressBook", result);
        }

        // GET: ManageAddressBook  
        public ActionResult ManageAddressBook()
        {
            return View();
        }

        public ActionResult LoadAddressBookData()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();


                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;


                // Getting all address book data    
                DbData objDB = new DbData(); //calling class DBdata  
                var addressBookData = objDB.PopulateAddressBookData();

                //Sorting    
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    addressBookData = addressBookData.OrderBy(sortColumn + " " + sortColumnDir);
                //}
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    addressBookData = addressBookData.Where(m => m.Name == searchValue);
                }

                //total number of rows count     
                recordsTotal = addressBookData.Count();
                //Paging     
                var data = addressBookData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpPost]
        public JsonResult DeleteAddressBook(int ID)
        {
            DbData objDB = new DbData(); //calling class DBdata  
            var addressBookRecord = objDB.GetAddressBookDataByID(ID);
            if (addressBookRecord == null)
            {
                return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
            }
            var result = objDB.DeleteData(ID);
            return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
        }

        // GET: /AddressBook/ScheduleCall
        public ActionResult ScheduleCall(int Id)
        {
            DbData objDB = new DbData(); //calling class DBdata  
            var result = objDB.GetAddressBookDataByID(Id); // get values for editing
            return View(result);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ScheduleCall(AddressBookModel model)
        {
            string accountSid = ConfigurationManager.AppSettings["Twilio:Sid"].ToString();
            string authToken = ConfigurationManager.AppSettings["Twilio:AuthToken"].ToString();
            string fromNumber = ConfigurationManager.AppSettings["Twilio:FromNumber"].ToString();
            TwilioClient.Init(accountSid, authToken);

            var to = new PhoneNumber(model.MobileNumber);
            var from = new PhoneNumber(fromNumber);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                | SecurityProtocolType.Tls11
                                                | SecurityProtocolType.Tls12
                                                | SecurityProtocolType.Ssl3;


            var gatherStr = $"<Gather input='speech dtmf' action='/AddressBook/Gather' finishOnKey='1' numDigits='1' timeout='5' length='5'><Say>Please press 1 to confirm you have understood the message</Say></Gather>";//<Say>We didn't receive any input. Goodbye!</Say>";

            var call = CallResource.Create(
            twiml: new Twilio.Types.Twiml($"<Response><Say>{model.Message}</Say>{gatherStr}<Pause length='3'/>{gatherStr}</Response>"),
            //twiml: new Twilio.Types.Twiml($"<Response><Say>{model.Message}</Say>{gatherStr}<Pause length='5'/>{gatherStr}</Response>"),
            to: new Twilio.Types.PhoneNumber(model.MobileNumber),
            from: new Twilio.Types.PhoneNumber(fromNumber)
        );



            Console.WriteLine(call.Sid);

            //DbData objDB = new DbData(); //calling class DBdata  
            //var result = objDB.GetAddressBookDataByID(Id); // get values for editing
            ViewBag.Message = "Success";
            return View(new AddressBookModel());

        }

        [HttpPost]
	public ActionResult Gather(string digits)
        {
            var response = new VoiceResponse();

            // If the user entered digits, process their request
            if (!string.IsNullOrEmpty(digits))
            {
                switch (digits)
                {
                    case "1":
                        response.Say("<Hangup/>");
                        break;
                    default:
                        response.Say("Sorry, I don't understand that choice.").Pause();
                        //response.Redirect("/voice");
                        break;
                }
            }
            else
            {
                // If no input was sent, redirect to the /voice route
              //  response.Redirect("/voice");
            }

            return View(new AddressBookModel());
        }

    }
}