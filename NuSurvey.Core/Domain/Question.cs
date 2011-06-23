﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentNHibernate.Mapping;
using UCDArch.Core.DomainModel;
using System.Web.Mvc;

namespace NuSurvey.Core.Domain
{
    public class Question : DomainObject
    {
        #region Constructor
        public Question(Survey survey)
        {
            SetPreDefaults();
            Survey = survey;
            SetPostDefaults();
        }

        public Question()
        {
            SetPreDefaults();
            SetPostDefaults();
        }

        private void SetPreDefaults()
        {
            CreateDate = DateTime.Now;
            IsActive = true;
            Responses = new List<Response>();
            OpenEndedQuestionType = 0;
        }

        private void SetPostDefaults()
        {
            if (Order == 0)
            {
                if (Survey != null && Survey.Questions != null && Survey.Questions.Count > 0)
                {
                    Order = Survey.Questions.Max(a => a.Order) + 1;
                }
                else
                {
                    Order = 1;
                }
            }
        }
        #endregion Constructor

        [Required]
        [StringLength(100)]
        [DisplayName("Question")]
        [AllowHtml]
        public virtual string Name { get; set; }

        [DisplayName("Active")]
        public virtual bool IsActive { get; set; }

        public virtual int Order { get; set; }

        [DisplayName("Open Ended")]
        public virtual bool IsOpenEnded { get; set; }

        [DisplayName("Date Created")]
        public virtual DateTime CreateDate { get; set; }

        [Required]
        public virtual Category Category { get; set; }
        [Required]
        public virtual Survey Survey { get; set; }

        [Required]
        public virtual IList<Response> Responses { get; set; }

        [DisplayName("Question Type")]
        public virtual int OpenEndedQuestionType { get; set; }

        #region Methods
        public virtual void AddResponse(Response response)
        {
            response.Question = this;
            Responses.Add(response);
        }
        #endregion Methods
    }
    public class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.IsActive);
            Map(x => x.Order).Column("`Order`");
            Map(x => x.IsOpenEnded);
            Map(x => x.CreateDate);
            Map(x => x.OpenEndedQuestionType);

            References(x => x.Category);
            References(x => x.Survey);
            HasMany(x => x.Responses).Cascade.AllDeleteOrphan();
        }
    }
}
