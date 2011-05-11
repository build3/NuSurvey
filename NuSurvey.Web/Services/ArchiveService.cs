﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using NuSurvey.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace NuSurvey.Web.Services
{
    public interface IArchiveService
    {
        /// <summary>
        /// This creates and saves a new version of the category and saves the old version of the category
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id">id of the current category</param>
        /// <param name="updatedCategory">the current category with updated values</param>
        /// <returns>the new category with updated values</returns>
        Category ArchiveCategory(IRepository repository, int id, Category updatedCategory);
    }

    public class ArchiveService : IArchiveService
    {
        /// <summary>
        /// This creates and saves a new version of the category and saves the old version of the category
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="id">id of the current category</param>
        /// <param name="updatedCategory">the current category with updated values</param>
        /// <returns>the new category with updated values</returns>
        public Category ArchiveCategory(IRepository repository, int id, Category updatedCategory)
        {
            var oldVersion = repository.OfType<Category>().GetNullableById(id);
            var newVersion = new Category(oldVersion.Survey);

            newVersion.Rank = oldVersion.Rank;
            newVersion.LastUpdate = DateTime.Now;
            newVersion.CreateDate = newVersion.LastUpdate;

            foreach (var categoryGoal in oldVersion.CategoryGoals)
            {
                var categoryGoalToDuplicate = new CategoryGoal();
                Mapper.Map(categoryGoal, categoryGoalToDuplicate);
                newVersion.AddCategoryGoal(categoryGoalToDuplicate);
            }
            foreach (var question in oldVersion.Questions)
            {
                var questionToDuplicate = new Question(oldVersion.Survey);
                questionToDuplicate.Order = question.Order;
                foreach (var response in question.Responses)
                {
                    var newResponse = new Response(); //If I don't do this, the old responses are *moved* here, not copied
                    Mapper.Map(response, newResponse);
                    questionToDuplicate.AddResponse(newResponse);
                }
                Mapper.Map(question, questionToDuplicate);
                newVersion.AddQuestions(questionToDuplicate);
            }


            newVersion.IsActive = updatedCategory.IsActive;
            newVersion.Name = updatedCategory.Name;
            newVersion.Affirmation = updatedCategory.Affirmation;
            newVersion.Encouragement = updatedCategory.Encouragement;
            newVersion.DoNotUseForCalculations = updatedCategory.DoNotUseForCalculations;
            newVersion.PreviousVersion = oldVersion;

            //*******************  SAVE
            repository.OfType<Category>().EnsurePersistent(newVersion);
            //*******************  SAVE


            oldVersion = repository.OfType<Category>().GetNullableById(id);
            oldVersion.IsCurrentVersion = false;

            //*******************  SAVE
            repository.OfType<Category>().EnsurePersistent(oldVersion);
            //*******************  SAVE

            return newVersion;
        }
    }
}