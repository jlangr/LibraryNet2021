using System.Collections.Generic;
using LibraryNet2020.ControllerHelpers;
using LibraryNet2020.Models;
using LibraryNet2020.Services;
using LibraryNet2020.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryNet2020.Controllers
{
    public class CheckOutController : LibraryController
    {
        public const string ModelKey = "CheckOut";
        private readonly CheckOutService checkOutService;
        private readonly BranchesService branchesService;

        public CheckOutController(LibraryContext _context, CheckOutService checkOutService, BranchesService branchesService)
        {
            this.checkOutService = checkOutService;
            this.branchesService = branchesService;
        }

        // GET: CheckOut
        public ActionResult Index()
        {
            var model = new CheckOutViewModel
                {BranchesViewList = new List<Branch>(branchesService.AllBranchesIncludingVirtual())};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CheckOutViewModel checkout)
        {
            if (!ModelState.IsValid) return View(checkout);

            checkout.BranchesViewList = new List<Branch>(branchesService.AllBranchesIncludingVirtual());
            if (!checkOutService.Checkout(checkout))
            {
                AddModelErrors(checkOutService.ErrorMessages, ModelKey);
                return View(checkout);
            }
            return RedirectToAction("Index");
        }
    }
}