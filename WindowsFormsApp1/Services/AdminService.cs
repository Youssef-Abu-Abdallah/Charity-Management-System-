using System;
using System.Threading.Tasks;
using System.Net.Http;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Config;

namespace WindowsFormsApp1.Services
{
    public class AdminService : BaseService
    {
        // 1. جلب قائمة طلبات التوثيق المعلقة (الجمعيات الجديدة)
        public async Task<PendingUsersResponse> GetPendingVerificationsAsync()
        {
            // المسار: /api/v1/admin/verifications/pending
            return await GetAsync<PendingUsersResponse>("admin/verifications/pending");
        }

        // 2. الموافقة على توثيق مستخدم (جمعية)
        public async Task<bool> VerifyUserAsync(Guid userId)
        {
            var request = new ActionUserRequestDTO { userId = userId };
            // المسار: /api/v1/admin/verifications/verify (Patch)
            var response = await PatchAsync<object, ActionUserRequestDTO>("admin/verifications/verify", request);
            return response != null;
        }
    }
}