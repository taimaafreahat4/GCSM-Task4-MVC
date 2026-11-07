
using Microsoft.AspNetCore.Mvc;
using MVC_task4.Models;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using static System.Net.Mime.MediaTypeNames;

namespace MVC_task4.Controllers
{
    public class SupplyRequestController : Controller
    {
        // عرض جميع العروض
        public IActionResult Index()
        {
            return View(GetOffers());
        }
        public IActionResult About()
        {
            return View();
        }

        // قائمة العروض التسعة
        private List<Offer> GetOffers()
        {
            List<Offer> offers = new List<Offer>();

            // Graduation Certificates

            offers.Add(new Offer
            {
                Id = 1,
                SupplierName = "Supplier A",
                OfferName = "Premium Certificate Package",
                Category = "Graduation Certificates",
                Quantity = 35,
                UnitPrice = 2
            });

            offers.Add(new Offer
            {
                Id = 2,
                SupplierName = "Supplier B",
                OfferName = "Standard Certificate Package",
                Category = "Graduation Certificates",
                Quantity = 35,
                UnitPrice = 1.5m
            });

            offers.Add(new Offer
            {
                Id = 3,
                SupplierName = "Supplier C",
                OfferName = "Luxury Certificate Package",
                Category = "Graduation Certificates",
                Quantity = 35,
                UnitPrice = 2.5m
            });

            // Hospitality

            offers.Add(new Offer
            {
                Id = 4,
                SupplierName = "Supplier D",
                OfferName = "Coffee & Drinks Package",
                Category = "Hospitality",
                Quantity = 100,
                UnitPrice = 2
            });

            offers.Add(new Offer
            {
                Id = 5,
                SupplierName = "Supplier E",
                OfferName = "Sandwiches & Snacks Package",
                Category = "Hospitality",
                Quantity = 100,
                UnitPrice = 3
            });

            offers.Add(new Offer
            {
                Id = 6,
                SupplierName = "Supplier F",
                OfferName = "Full Hospitality Package",
                Category = "Hospitality",
                Quantity = 100,
                UnitPrice = 4
            });

            // Graduation Gifts

            offers.Add(new Offer
            {
                Id = 7,
                SupplierName = "Supplier G",
                OfferName = "Customized Mugs Package",
                Category = "Graduation Gifts",
                Quantity = 35,
                UnitPrice = 2
            });

            offers.Add(new Offer
            {
                Id = 8,
                SupplierName = "Supplier H",
                OfferName = "Graduation Bags Package",
                Category = "Graduation Gifts",
                Quantity = 35,
                UnitPrice = 3
            });

            offers.Add(new Offer
            {
                Id = 9,
                SupplierName = "Supplier I",
                OfferName = "Premium Gift Boxes Package",
                Category = "Graduation Gifts",
                Quantity = 35,
                UnitPrice = 4
            });

            return offers;
        }

        // حفظ العرض المختار في Session
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectOffer(int offerId, string category)
        {
            List<Offer> offers = GetOffers();

            Offer? selectedOffer = offers.FirstOrDefault(
                x => x.Id == offerId && x.Category == category);

            if (selectedOffer == null)
            {
                return BadRequest("Invalid offer selection.");
            }

            switch (category)
            {
                case "Graduation Certificates":
                    HttpContext.Session.SetInt32("CertificateOffer", offerId);
                    break;

                case "Hospitality":
                    HttpContext.Session.SetInt32("HospitalityOffer", offerId);
                    break;

                case "Graduation Gifts":
                    HttpContext.Session.SetInt32("GiftOffer", offerId);
                    break;
            }

            return RedirectToAction("Index");
        }

        // مراجعة العروض المختارة

        public IActionResult Review()
        {
            int? certificateId =
                HttpContext.Session.GetInt32("CertificateOffer");

            int? hospitalityId =
                HttpContext.Session.GetInt32("HospitalityOffer");

            int? giftId =
                HttpContext.Session.GetInt32("GiftOffer");

            // التأكد من اختيار 3 عروض
            if (certificateId == null ||
                hospitalityId == null ||
                giftId == null)
            {
                TempData["Message"] =
                    "Please select one offer from each category.";

                return RedirectToAction("Index");
            }

            List<Offer> offers = GetOffers();

            List<Offer> selectedOffers = new List<Offer>();

            Offer? certificate = offers.FirstOrDefault(
                x => x.Id == certificateId &&
                     x.Category == "Graduation Certificates");

            Offer? hospitality = offers.FirstOrDefault(
                x => x.Id == hospitalityId &&
                     x.Category == "Hospitality");

            Offer? gift = offers.FirstOrDefault(
                x => x.Id == giftId &&
                     x.Category == "Graduation Gifts");

            if (certificate == null ||
                hospitality == null ||
                gift == null)
            {
                return RedirectToAction("Index");
            }

            selectedOffers.Add(certificate);
            selectedOffers.Add(hospitality);
            selectedOffers.Add(gift);

            // حساب سعر كل عرض والمجموع
            //offertotals==>نتائج الحسابات فقط 
            //selectedoffers==> معلومات الموردين
            decimal totalPrice = 0;
            List<decimal> offerTotals = new List<decimal>();

            foreach (var offer in selectedOffers)
            {
                //offertotal=quantity*unitprice

                decimal offerTotal =
                    offer.Quantity * offer.UnitPrice;

                offerTotals.Add(offerTotal);
                totalPrice += offerTotal;
            }

            // حساب الخصم
            decimal discountPercentage = 0;

            if (totalPrice > 500)
            {
                discountPercentage = 15;
            }
            else if (totalPrice > 300)
            {
                discountPercentage = 10;
            }
            else
            {
                discountPercentage = 0;
            }

            decimal discountAmount =
                totalPrice * discountPercentage / 100;

            decimal finalTotal =
                totalPrice - discountAmount;

            // إرسال النتائج إلى View
            ViewBag.OfferTotals = offerTotals;
            ViewBag.TotalPrice = totalPrice;
            ViewBag.DiscountPercentage = discountPercentage;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.FinalTotal = finalTotal;

            return View(selectedOffers);
        }
    }
}
