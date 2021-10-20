using System;
using LibraryNet2020.Controllers;
using LibraryNet2020.Models;
using LibraryNet2020.NonPersistentModels;
using LibraryNet2020.Services;
using LibraryNet2020.Util;

namespace LibraryNet2020.Scanner
{
    public class ScanStation
    {
        public const int NoPatron = -1;
        private readonly IClassificationService classificationService;
        private readonly int branchId;
        private DateTime currentTimestamp;
        private HoldingsService holdingsService;
        private PatronsService patronsService;

        public ScanStation(LibraryContext context, int branchId)
            : this(context,
                branchId,
                new MasterClassificationService(),
                new HoldingsService(context),
                new PatronsService(context))
        {
        }

        public ScanStation(LibraryContext _context, int branchId, IClassificationService classificationService,
            HoldingsService holdingsService, PatronsService patronsService)
        {
            this.classificationService = classificationService;
            this.holdingsService = holdingsService;
            this.patronsService = patronsService;
            BranchId = branchId;
            this.branchId = BranchId;
        }

        public Holding AddNewHolding(string isbn)
        {
            var classification = classificationService.Classification(isbn);
            var holding = new Holding
            {
                Classification = classification,
                CopyNumber = holdingsService.NextAvailableCopyNumber(classification),
                BranchId = BranchId
            };
            holdingsService.Add(holding);
            return holding;
        }

        public int BranchId { get; }

        public int CurrentPatronId { get; private set; } = NoPatron;

        public void AcceptLibraryCard(int patronId)
        {
            CurrentPatronId = patronId;
            currentTimestamp = TimeService.Now;
        }

        // public void AcceptBarcode(string barcode)
        // {
        //     var holding = holdingsService.FindByBarcode(barcode);
        //     var now = TimeService.Now;
        //
        //     if (IsInCheckinMode() && holding.IsCheckedOut)
        //     {
        //         if (holding.IsCheckedOut)
        //         {
        //             CheckInHolding(holding, now);
        //         }
        //         else
        //         {
        //             
        //         }
        //     }
        //     else if (IsInPatronCheckoutMode())
        //     {
        //         if (holding.IsCheckedOut)
        //         {
        //             if (HasCurrentPatronCheckedOutHolding(holding))
        //             {
        //                 CheckInHolding(holding, now);
        //                 CheckOutHolding(holding, now);
        //             }
        //         }
        //         else 
        //             CheckOutHolding(holding, now);
        //     }
        //     else if (!holding.IsCheckedOut) {
        //         if (IsInPatronCheckoutMode())
        //             CheckOutHolding(holding, now);
        //         else
        //             throw new CheckoutException();
        //     }
        // }
        
        public void AcceptBarcode(string barcode)
        {
            var holding = holdingsService.FindByBarcode(barcode);
            var now = TimeService.Now;

            if (holding.IsCheckedOut)
            {
                if (IsInCheckinMode())
                    CheckInHolding(holding, now);
                else
                    if (HasAnotherPatronCheckedOutHolding(holding))
                    {
                        CheckInHolding(holding, now);
                        holdingsService.CheckOutHolding(holding, now, CurrentPatronId);
                    }
            }
            else
            {
                if (IsInPatronCheckoutMode())
                    holdingsService.CheckOutHolding(holding, now, CurrentPatronId);
                else
                    throw new CheckoutException();
            }
        }

        public void AssessAnyFineOnPatron(Holding holding, DateTime now)
        {
            var patron = PatronWithHolding(holding);
            patron.Fine(CalculateFineAmount(holding, now));
            patronsService.Update(patron);
        }

        private Patron PatronWithHolding(Holding holding)
        {
            return patronsService.FindById(holding.HeldByPatronId);
        }

        private decimal CalculateFineAmount(Holding holding, DateTime now)
        {
            var material = classificationService.Retrieve(holding.Classification);
            return material.CheckoutPolicy.FineAmount(holding.CheckOutTimestamp.Value, now);
        }

        private void CheckInHolding(Holding holding, DateTime timestamp)
        {
            AssessAnyFineOnPatron(holding, timestamp);
            holding.CheckIn(timestamp, branchId);
            holdingsService.Update(holding);
        }

        private bool HasAnotherPatronCheckedOutHolding(Holding holding)
        {
            return holding.HeldByPatronId != CurrentPatronId;
        }

        public void CompleteCheckout()
        {
            CurrentPatronId = NoPatron;
        }

        private bool IsInCheckinMode()
        {
            return CurrentPatronId == NoPatron;
        }

        private bool IsInPatronCheckoutMode()
        {
            return !IsInCheckinMode();
        }
    }
}