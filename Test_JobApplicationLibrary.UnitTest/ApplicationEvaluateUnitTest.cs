using JobApplicationLibrary;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using Moq;

namespace Test_JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {


        // UnitOfWork_Condition_ExpectedResult
    


        // minimum yasa uymama durumu
        [Test]
        public void Application_ShouldTransferredToAutoRejected_WithUnderAge()
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

            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);
        }



        // Teknik yetkinlik olmamasi durumu
        [Test]
        public void Application_WithNoTechStack_ShouldTransferredToAutoRejected()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();
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

            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);
        }



        // Teknik yetkinlik %75'den fazla olmasi durumu
        [Test]
        public void Application_WithTechStackOver75P_ShouldTransferredToAutoAccepted()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>();
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

            Assert.AreEqual(appResult, ApplicationResult.AutoAccepted);
        }



        // Gecersiz Identity Durumu
        [Test]
        public void Application_WithInvalidIdentity_ShouldTransferredToHR()
        {
            // Arrange

            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Loose);
            
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

            Assert.AreEqual(appResult, ApplicationResult.TransferredToHR);
        }











    }
}