using JobApplicationLibrary;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using Moq;
using FluentAssertions;

namespace Test_JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {


        // UnitOfWork_Condition_ExpectedResult



        // minimum yasa uymama durumu
        [Test]
        public void Application_WithUnderAge_ShouldTransferredToAutoRejected()
        {
            // Arrange
            var evaluator = new ApplicationEvaluator(null);

            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }

            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            appResult.Should().Be(ApplicationResult.AutoRejected);

        }


        
        // Teknik yetkinlik olmamasi durumu
        [Test]
        public void Application_WithNoTechStack_ShouldTransferredToAutoRejected()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);

            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                    IdentityNumber = "123"
                },
                TechStackList = new List<string>
                {
                    ""
                }

            };

            // Action

            var appResult = evaluator.Evaluate(form);

            // Assert
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }



        // Teknik yetkinlik %75'den fazla olmasi durumu
        [Test]
        public void Application_WithTechStackOver75P_ShouldTransferredToAutoAccepted()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);

            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                },
                TechStackList = new List<string>
                {
                    "C#",
                    "RabbitMQ",
                    "Microservice",
                    "Visual Studio"
                },
                YearsOfExperience = 16,

            };

            // Action

            var appResult = evaluator.Evaluate(form);

            // Assert
            appResult.Should().Be(ApplicationResult.AutoAccepted);
        }



        // Gecersiz Identity Durumu
        [Test]
        public void Application_WithInvalidIdentity_ShouldTransferredToHR()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);

            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            // Mock Behavior Loose olmasi default degerler atanmasini saglar. Default deger loosedir.

            /* Mock Behavior Strict olmasi deger atanmasinin manuel olmasini zorunlu kilar.
            ve ayni zamanda test edilen sinifta tanimlanan hersey icin setup yapilmasi zorunludur. */

            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);

            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                }
            };

            // Action

            var appResult = evaluator.Evaluate(form);

            // Assert
            appResult.Should().Be(ApplicationResult.TransferredToHR);
        }



        // Calisma lokasyonu Turkiyeden farkli olma durumu
        [Test]
        public void Application_WithOfficeLocation_ShouldTransferredToCTO()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("SPAIN");


            var evaluator = new ApplicationEvaluator(mockValidator.Object);

            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                }
            };

            // Action

            var appResult = evaluator.Evaluate(form);

            // Assert
            appResult.Should().Be(ApplicationResult.TransferredToCTO);
        }



        // Yas 50'den buyuk olma detailed validation mode durumu
        [Test]
        public void Application_WithOver50Y_ValidationModeDetailed()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");


            mockValidator.SetupProperty(i => i.ValidationMode);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);

            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 51,
                }
            };

            // Action

            var appResult = evaluator.Evaluate(form);

            // Assert
            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }



        // Null applicant durumu - Throw exception unit test islemi
        [Test]
        public void Application_WithNullApplicant_ThrowsArgumentNullException()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication();


            // Action

            Action appResultAction = () => evaluator.Evaluate(form);

            // Assert
            appResultAction.Should().Throw<ArgumentNullException>();
        }



        // Verify islemi yapilmasi - IsValid metodu calisiyor mu?
        [Test]
        public void Application_WithDefaultValue_IsValidCalled()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);

            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 19,
                    IdentityNumber = "123"
                }
            };

            // Action

            var appResult = evaluator.Evaluate(form);

            // Assert

            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()));
        }



        // Metodun hic calismadigini kontrol etmek
        [Test]
        public void Application_WithYoungAge_IsValidNeverCalled()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 15,
                }
            };

            // Action

            var appResult = evaluator.Evaluate(form);

            // Assert

            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),Times.Never());

            // Times.Exactly(kac kere cagirilmasi gerektigi buraya yazilir.)
        }
    }
}