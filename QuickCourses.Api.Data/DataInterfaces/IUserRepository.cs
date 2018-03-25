﻿using System.Threading.Tasks;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.DataInterfaces
{
    public interface IUserRepository
    {
        Task<User> Get(int id);
        Task<bool> Contains(int id);
        Task Insert(User user);
    }
}
