﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using SimpleJsonApi.Configuration;
using SimpleJsonApi.DocumentConverters;
using SimpleJsonApi.Extensions;
using SimpleJsonApi.Models;

namespace SimpleJsonApi.Tests.DocumentConverters
{
    [TestFixture]
    public class DocumentBuilderTests
    {
        private DocumentBuilder _documentBuilder;

        [SetUp]
        public void SetUp()
        {
            var resourceBuilder = new ResourceConfigurationsBuilder();
            resourceBuilder.Resource<User>();
            resourceBuilder.Resource<UserInfo>().BelongsTo<User>(x => x.UserId);
            resourceBuilder.Resource<Group>().HasMany<User>(x => x.Users);
            resourceBuilder.Resource<Population>().HasMany<Group>(x => x.Groups);

            _documentBuilder = new DocumentBuilder(resourceBuilder.Build());
        }

        [Test]
        public void Build_FlatResource_ShouldGenerateCorrectDocument()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Givenname = "Marc",
                Surname = "Coucke",
                IsActive = true
            };
            var requestUri = new Uri($"https://localhost/api/users/{user.Id}");
            var document = _documentBuilder.BuildDocument(user, new HttpRequestMessage(HttpMethod.Get, requestUri));

            Assert.That(document.Errors, Is.Null);
            Assert.That(document.Links["Self"], Is.EqualTo(requestUri));
            var data = document.Data as DocumentData;
            Assert.That(data.Id, Is.EqualTo(user.Id));
            Assert.That(data.Type, Is.EqualTo("users"));
            Assert.That(data.Attributes["Givenname"], Is.EqualTo(user.Givenname));
            Assert.That(data.Attributes["Surname"], Is.EqualTo(user.Surname));
            Assert.That(data.Attributes["IsActive"], Is.EqualTo(user.IsActive));
        }

        [Test]
        public void Build_ResourceWithToOneGuidRelation_ShouldGenerateCorrectDocument()
        {
            var userInfo = new UserInfo
            {
                Id = Guid.NewGuid(),
                Age = 35,
                UserId = Guid.NewGuid()
            };
            var requestUri = new Uri($"https://localhost/api/userinfos/{userInfo.Id}");
            var document = _documentBuilder.BuildDocument(userInfo, new HttpRequestMessage(HttpMethod.Get, requestUri));

            Assert.That(document.Errors, Is.Null);
            Assert.That(document.Links["Self"], Is.EqualTo(requestUri));
            var data = document.Data as DocumentData;
            Assert.That(data.Id, Is.EqualTo(userInfo.Id));
            Assert.That(data.Type, Is.EqualTo("userinfos"));
            Assert.That(data.Attributes["Age"], Is.EqualTo(35));
            Assert.That(data.Relationships.Count, Is.EqualTo(1));
            var relationData = data.Relationships["User"].Data as RelationData;
            Assert.That(relationData.Id, Is.EqualTo(userInfo.UserId));
            Assert.That(relationData.Type, Is.EqualTo("users"));
        }

        [Test]
        public void Build_ResourceWithToManyGuidRelation_ShouldGenerateCorrectDocument()
        {
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = "SomeGroup",
                Users = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
            };
            var requestUri = new Uri($"https://localhost/api/groups/{group.Id}");
            var document = _documentBuilder.BuildDocument(group, new HttpRequestMessage(HttpMethod.Get, requestUri));

            Assert.That(document.Errors, Is.Null);
            Assert.That(document.Links["Self"], Is.EqualTo(requestUri));
            var data = document.Data as DocumentData;
            Assert.That(data.Id, Is.EqualTo(group.Id));
            Assert.That(data.Type, Is.EqualTo("groups"));
            Assert.That(data.Attributes["Name"], Is.EqualTo("SomeGroup"));
            var relationData = (data.Relationships["Users"].Data as IEnumerable<RelationData>).ToList();
            Assert.That(relationData.Count, Is.EqualTo(3));
            foreach (var relation in relationData)
            {
                Assert.That(relation.Type, Is.EqualTo("users"));
                Assert.True(group.Users.Any(x => x == relation.Id));
            }
        }

        [Test]
        public void Build_ResourceWithToManyObjectRelation_ShouldGenerateCorrectDocument()
        {
            var population = new Population
            {
                Id = Guid.NewGuid(),
                Groups = new[]
                {
                    new Group { Id = Guid.NewGuid(), Name = "GroupA", Users = new[] { Guid.NewGuid(), Guid.NewGuid() } },
                    new Group { Id = Guid.NewGuid(), Name = "GroupB", Users = new[] { Guid.NewGuid() } }
                }
            };
            var requestUri = new Uri($"https://localhost/api/populations/{population.Id}");
            var document = _documentBuilder.BuildDocument(population, new HttpRequestMessage(HttpMethod.Get, requestUri));

            Assert.That(document.Errors, Is.Null);
            Assert.That(document.Links["Self"], Is.EqualTo(requestUri));
            var data = document.Data as DocumentData;
            Assert.That(data.Id, Is.EqualTo(population.Id));
            Assert.That(data.Type, Is.EqualTo("populations"));
            var relationData = (data.Relationships["Groups"].Data as IEnumerable<RelationData>).ToList();
            Assert.That(relationData.Count, Is.EqualTo(2));
            foreach (var relation in relationData)
            {
                Assert.That(relation.Type, Is.EqualTo("groups"));
                Assert.True(population.Groups.Any(x => x.Id == relation.Id));
            }

            var included = document.Included.ToList();
            Assert.That(included.Count, Is.EqualTo(2));
            Assert.That(included[0].Type, Is.EqualTo("groups"));
            Assert.That(included[1].Type, Is.EqualTo("groups"));
            Assert.That(included[0].Id, Is.EqualTo(population.Groups.First().Id));
            Assert.That(included[1].Id, Is.EqualTo(population.Groups.Last().Id));
            Assert.That(included[0].Attributes["Name"], Is.EqualTo("GroupA"));
            Assert.That(included[1].Attributes["Name"], Is.EqualTo("GroupB"));

            var userRelations = (included[0].Relationships["Users"].Data as IEnumerable<RelationData>).ToList();
            Assert.That(userRelations.Count, Is.EqualTo(2));
            Assert.That(userRelations[0].Type, Is.EqualTo("users"));
            Assert.That(userRelations[0].Id, Is.EqualTo(population.Groups.First().Users.First()));
            Assert.That(userRelations[1].Type, Is.EqualTo("users"));
            Assert.That(userRelations[1].Id, Is.EqualTo(population.Groups.First().Users.Last()));

            userRelations = (included[1].Relationships["Users"].Data as IEnumerable<RelationData>).ToList();
            Assert.That(userRelations.Count, Is.EqualTo(1));
            Assert.That(userRelations[0].Type, Is.EqualTo("users"));
            Assert.That(userRelations[0].Id, Is.EqualTo(population.Groups.Last().Users.First()));
        }

        [Test]
        public void Build_AdditionalResourceLinkWithoutMetadata_ShouldIncludeAdditionalResourceLink()
        {
            var user = new User { Id = Guid.NewGuid() };
            var requestUri = new Uri($"https://localhost/api/users/{user.Id}");
            var activateUserUri = new Uri($"https://localhost/api/users/{user.Id}/activate");
            var deleteUserUri = new Uri($"https://localhost/api/users/{user.Id}/delete");
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            request.AddResourceLink("ActivateUser", activateUserUri);
            request.AddResourceLink("DeleteUser", deleteUserUri);

            var document = _documentBuilder.BuildDocument(user, request);
            var links = document.Links;
            Assert.That(links.Count, Is.EqualTo(3));
            Assert.True(links.Any(x => x.Key == "Self" && x.Value.Equals(requestUri.AbsoluteUri)));
            Assert.True(links.Any(x => x.Key == "ActivateUser" && x.Value.Equals(activateUserUri.AbsoluteUri)));
            Assert.True(links.Any(x => x.Key == "DeleteUser" && x.Value.Equals(deleteUserUri.AbsoluteUri)));
        }

        [Test]
        public void Build_AdditionalResourceLinkWithMetadata_ShouldIncludeAdditionalResourceLink()
        {
            var user = new User { Id = Guid.NewGuid() };
            var requestUri = new Uri($"https://localhost/api/users/{user.Id}");
            var activateUserUri = new Uri($"https://localhost/api/users/{user.Id}/activate");
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            request.AddResourceLink("ActivateUser", activateUserUri, new
            {
                Method = "POST",
                Abc = "123"
            });

            var document = _documentBuilder.BuildDocument(user, request);
            var links = document.Links;
            Assert.That(links.Count, Is.EqualTo(2));
            Assert.True(links.Any(x => x.Key == "Self" && x.Value.Equals(requestUri.AbsoluteUri)));
            var link = links.First(x => x.Key.Equals("ActivateUser")).Value as LinkData;
            Assert.That(link.Href, Is.EqualTo(activateUserUri.AbsoluteUri));
            Assert.That(link.Meta, Is.Not.Null);
            Assert.That(((dynamic)link.Meta).Method, Is.EqualTo("POST"));
            Assert.That(((dynamic)link.Meta).Abc, Is.EqualTo("123"));
        }

        #region Test models

        private class User
        {
            public Guid Id { get; set; }
            public string Givenname { get; set; }
            public string Surname { get; set; }
            public bool IsActive { get; set; }
        }

        private class UserInfo
        {
            public Guid Id { get; set; }
            public int Age { get; set; }
            [JsonProperty("User")]
            public Guid UserId { get; set; }
        }

        private class Group
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<Guid> Users { get; set; }
        }

        private class Population
        {
            public Guid Id { get; set; }
            public IEnumerable<Group> Groups { get; set; }
        }

        #endregion
    }
}
