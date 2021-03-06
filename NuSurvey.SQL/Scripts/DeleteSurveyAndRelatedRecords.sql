/****** Script for SelectTopNRows command from SSMS  ******/
Declare @SurveyIdToDelete int
set @SurveyIdToDelete = 0


delete 
  FROM [NuSurvey].[dbo].[Answers]
  where [SurveyResponseId] in (SELECT id
  FROM [NuSurvey].[dbo].[SurveyResponses]
  where SurveyId = @SurveyIdToDelete)

delete
from NuSurvey.dbo.Responses
where QuestionId in (select id
from NuSurvey.dbo.Questions
where SurveyId = @SurveyIdToDelete)  
  
delete NuSurvey.dbo.Questions
where SurveyId = @SurveyIdToDelete

delete NuSurvey.dbo.SurveyResponses
where SurveyId = @SurveyIdToDelete  
  
delete
from NuSurvey.dbo.CategoryGoals
where CategoryId in (select id
from NuSurvey.dbo.Categories
where SurveyId = @SurveyIdToDelete)    

delete NuSurvey.dbo.Categories
where SurveyId = @SurveyIdToDelete  

delete NuSurvey.dbo.Surveys
where id = @SurveyIdToDelete  

  go
  
