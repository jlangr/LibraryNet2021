using System;
using System.Collections.Generic;
using System.Linq;
using LibraryNet2020.Controllers;
using LibraryNet2020.Models;
using LibraryNet2020.NonPersistentModels;
using LibraryNet2020.Scanner;
using LibraryNet2020.Services;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace LibraryTest.Scanner
{
    [Collection("SharedLibraryContext")]
    public class ScanStation_WhenNothingCheckedOutTest
    {
        private const string SomeBarcode = "QA123:1";
        private readonly int somePatronId;

        private LibraryContext context;
        
        private IClassificationService classificationService;
        Mock<IClassificationService> classificationServiceMock;
        private PatronsService patronsService;
        private HoldingsService holdingsService;
        ScanStation scanner;

        public ScanStation_WhenNothingCheckedOutTest(DbContextFixture fixture)
        {
            fixture.Seed();
            context = new LibraryContext(fixture.ContextOptions);
            patronsService = new PatronsService(context);
            holdingsService = new HoldingsService(context);

            classificationServiceMock = new Mock<IClassificationService>();
            classificationService = classificationServiceMock.Object;
            AlwaysReturnBookMaterial(classificationServiceMock);

            somePatronId = patronsService.Create(new Patron {Name = "x"});
            
            scanner = new ScanStation(context, 1, classificationService, new HoldingsService(context),
                new PatronsService(context));
        }

        void AlwaysReturnBookMaterial(Mock<IClassificationService> serviceMock)
        {
            serviceMock.Setup(service => service.Retrieve(It.IsAny<string>()))
                .Returns(new Material {CheckoutPolicy = new BookCheckoutPolicy()});
        }

        [Fact]
        public void StoresHoldingAtBranchWhenNewMaterialAdded()
        {
            classificationServiceMock.Setup(service => service.Classification("anIsbn")).Returns("AB123");

            scanner.AddNewHolding("anIsbn");

            Assert.Equal(scanner.BranchId, holdingsService.FindByBarcode("AB123:1").BranchId);
        }

        [Fact]
        public void CopyNumberIncrementedWhenNewMaterialWithSameIsbnAdded()
        {
            classificationServiceMock.Setup(service => service.Classification("anIsbn")).Returns("AB123");
            scanner.AddNewHolding("anIsbn");

            scanner.AddNewHolding("anIsbn");

            // todo move to service get all
            var holdingBarcodes = context.Holdings.ToList().Select(h => h.Barcode);
            // Assert.That(holdingBarcodes, Is.EquivalentTo(new List<string> { "AB123:1", "AB123:2" }));
            Assert.Equal(new List<string> {"AB123:1", "AB123:2"}, holdingBarcodes);
        }

        [Fact]
        public void ThrowsWhenCheckingInCheckedOutBookWithoutPatronScan()
        {
            scanner.ScanNewMaterial(SomeBarcode, classificationServiceMock);

            Assert.Throws<CheckoutException>(() => scanner.AcceptBarcode(SomeBarcode));
        }

        [Fact]
        public void PatronIdUpdatedWhenLibraryCardAccepted()
        {
            scanner.AcceptLibraryCard(somePatronId);

            Assert.Equal(somePatronId, scanner.CurrentPatronId);
        }

        [Fact]
        public void PatronIdClearedWhenCheckoutCompleted()
        {
            scanner.AcceptLibraryCard(somePatronId);

            scanner.CompleteCheckout();

            Assert.Equal(ScanStation.NoPatron, scanner.CurrentPatronId);
        }
    }
}