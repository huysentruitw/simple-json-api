using NUnit.Framework;

namespace SimpleJsonApi.Tests
{
    [TestFixture]
    public class ChangesTests
    {
        [Test]
        public void Changes_ApplyTo_AddedChanges_ShouldApplyAddedChanges()
        {
            var changes = new Changes<User>();
            changes.AddChange(x => x.GivenName = "TestGivenName");
            changes.AddChange(x => x.Age = 25);

            var user = new User { GivenName = "DefaultGivenName", Surname = "DefaultSurname", Age = 30 };
            changes.ApplyTo(user);

            Assert.That(user.GivenName, Is.EqualTo("TestGivenName"));
            Assert.That(user.Surname, Is.EqualTo("DefaultSurname"));
            Assert.That(user.Age, Is.EqualTo(25));
        }

        private class User
        {
            public string GivenName { get; set; }
            public string Surname { get; set; }
            public int Age { get; set; }
        }
    }
}
