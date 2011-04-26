using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuSurvey.Core.Domain;
using NuSurvey.Tests.Core;
using NuSurvey.Tests.Core.Extensions;
using NuSurvey.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Testing;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace NuSurvey.Tests.RepositoryTests
{
    /// <summary>
    /// Entity Name:		Survey
    /// LookupFieldName:	Name
    /// </summary>
    [TestClass]
    public class SurveyRepositoryTests : AbstractRepositoryTests<Survey, int, SurveyMap>
    {
        /// <summary>
        /// Gets or sets the Survey repository.
        /// </summary>
        /// <value>The Survey repository.</value>
        public IRepository<Survey> SurveyRepository { get; set; }
		
        #region Init and Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveyRepositoryTests"/> class.
        /// </summary>
        public SurveyRepositoryTests()
        {
            SurveyRepository = new Repository<Survey>();
        }

        /// <summary>
        /// Gets the valid entity of type T
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override Survey GetValid(int? counter)
        {
            return CreateValidEntities.Survey(counter);
        }

        /// <summary>
        /// A Query which will return a single record
        /// </summary>
        /// <param name="numberAtEnd"></param>
        /// <returns></returns>
        protected override IQueryable<Survey> GetQuery(int numberAtEnd)
        {
            return SurveyRepository.Queryable.Where(a => a.Name.EndsWith(numberAtEnd.ToString()));
        }

        /// <summary>
        /// A way to compare the entities that were read.
        /// For example, this would have the assert.AreEqual("Comment" + counter, entity.Comment);
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="counter"></param>
        protected override void FoundEntityComparison(Survey entity, int counter)
        {
            Assert.AreEqual("Name" + counter, entity.Name);
        }

        /// <summary>
        /// Updates , compares, restores.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        protected override void UpdateUtility(Survey entity, ARTAction action)
        {
            const string updateValue = "Updated";
            switch (action)
            {
                case ARTAction.Compare:
                    Assert.AreEqual(updateValue, entity.Name);
                    break;
                case ARTAction.Restore:
                    entity.Name = RestoreValue;
                    break;
                case ARTAction.Update:
                    RestoreValue = entity.Name;
                    entity.Name = updateValue;
                    break;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            SurveyRepository.DbContext.BeginTransaction();
            LoadRecords(5);
            SurveyRepository.DbContext.CommitTransaction();
        }

        #endregion Init and Overrides	
        
        #region Name Tests
        #region Invalid Tests

        /// <summary>
        /// Tests the Name with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithNullValueDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.Name = null;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with empty string does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithEmptyStringDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.Name = string.Empty;
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with spaces only does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithSpacesOnlyDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.Name = " ";
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: may not be null or empty");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Tests the Name with too long value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNameWithTooLongValueDoesNotSave()
        {
            Survey survey = null;
            try
            {
                #region Arrange
                survey = GetValid(9);
                survey.Name = "x".RepeatTimes((100 + 1));
                #endregion Arrange

                #region Act
                SurveyRepository.DbContext.BeginTransaction();
                SurveyRepository.EnsurePersistent(survey);
                SurveyRepository.DbContext.CommitTransaction();
                #endregion Act
            }
            catch (Exception)
            {
                Assert.IsNotNull(survey);
                Assert.AreEqual(100 + 1, survey.Name.Length);
                var results = survey.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Name: length must be between 0 and 100");
                Assert.IsTrue(survey.IsTransient());
                Assert.IsFalse(survey.IsValid());
                throw;
            }
        }
        #endregion Invalid Tests

        #region Valid Tests

        /// <summary>
        /// Tests the Name with one character saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithOneCharacterSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.Name = "x";
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        /// <summary>
        /// Tests the Name with long value saves.
        /// </summary>
        [TestMethod]
        public void TestNameWithLongValueSaves()
        {
            #region Arrange
            var survey = GetValid(9);
            survey.Name = "x".RepeatTimes(100);
            #endregion Arrange

            #region Act
            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();
            #endregion Act

            #region Assert
            Assert.AreEqual(100, survey.Name.Length);
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());
            #endregion Assert
        }

        #endregion Valid Tests
        #endregion Name Tests

        #region IsActive Tests

        /// <summary>
        /// Tests the IsActive is false saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsFalseSaves()
        {
            #region Arrange

            Survey survey = GetValid(9);
            survey.IsActive = false;

            #endregion Arrange

            #region Act

            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsFalse(survey.IsActive);
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());

            #endregion Assert
        }

        /// <summary>
        /// Tests the IsActive is true saves.
        /// </summary>
        [TestMethod]
        public void TestIsActiveIsTrueSaves()
        {
            #region Arrange

            var survey = GetValid(9);
            survey.IsActive = true;

            #endregion Arrange

            #region Act

            SurveyRepository.DbContext.BeginTransaction();
            SurveyRepository.EnsurePersistent(survey);
            SurveyRepository.DbContext.CommitTransaction();

            #endregion Act

            #region Assert

            Assert.IsTrue(survey.IsActive);
            Assert.IsFalse(survey.IsTransient());
            Assert.IsTrue(survey.IsValid());

            #endregion Assert
        }

        #endregion IsActive Tests
        
        
        
        #region Reflection of Database.

        /// <summary>
        /// Tests all fields in the database have been tested.
        /// If this fails and no other tests, it means that a field has been added which has not been tested above.
        /// </summary>
        [TestMethod]
        public void TestAllFieldsInTheDatabaseHaveBeenTested()
        {
            #region Arrange
            var expectedFields = new List<NameAndType>();

            expectedFields.Add(new NameAndType("Id", "System.Int32", new List<string>
            {
                "[Newtonsoft.Json.JsonPropertyAttribute()]", 
                "[System.Xml.Serialization.XmlIgnoreAttribute()]"
            }));
            expectedFields.Add(new NameAndType("IsActive", "System.Boolean", new List<string>()));
            expectedFields.Add(new NameAndType("Name", "System.String", new List<string>
            {
                 "[NHibernate.Validator.Constraints.LengthAttribute((Int32)100)]", 
                 "[UCDArch.Core.NHibernateValidator.Extensions.RequiredAttribute()]"
            }));
            #endregion Arrange

            AttributeAndFieldValidation.ValidateFieldsAndAttributes(expectedFields, typeof(Survey));

        }

        #endregion Reflection of Database.	
		
		
    }
}