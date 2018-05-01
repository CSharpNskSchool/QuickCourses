﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Extensions;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Data.Repositories
{
    public class ProgressRepository : RepositoryBase<CourseProgress>, IProgressRepository
    {
        public ProgressRepository(Settings settings) 
            : base(settings)
        {
        }

        public async Task<List<CourseProgress>> GetAllByUserAsync(string userId)
        {
            var result = await Context.Collection.Find(progress => progress.Id.StartsWith(userId)).ToListAsync();
            result.ForEach(progress => progress.SetUpLinks());
            return result;
        }

        public Task<CourseProgress> GetAsync(string userId, string courseId)
        {
            return GetAsync($"{userId}{courseId}");
        }

        public Task<bool> ContainsAsync(string userId, string courseId)
        {
            return ContainsAsync($"{userId}{courseId}");
        }

        public override async Task<List<CourseProgress>> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            result.ForEach(progress => progress.SetUpLinks());
            return result;
        }

        public override async Task<CourseProgress> GetAsync(string id)
        {
            var result = await base.GetAsync(id);
            result.SetUpLinks();
            return result;
        }

        public override async Task<string> InsertAsync(CourseProgress value)
        {
            var id = $"{value.UserId}{value.CourceId}";
            value.Id = id;
            await Context.Collection.InsertOneAsync(value);
            return id;
        }
    }
}