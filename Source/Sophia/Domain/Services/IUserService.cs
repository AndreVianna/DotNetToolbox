﻿
namespace Sophia.WebApp.Services;

public interface IUserService {
    Task<UserData> GetCurrentUserProfile();
}
