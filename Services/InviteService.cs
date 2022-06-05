﻿using JATS.Data;
using JATS.Models;
using JATS.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace JATS.Services
{
    public class InviteService : IInviteService
    {
        private readonly ApplicationDbContext _context;
        public InviteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);
            if (invite is null)
            {
                return false;
            }

            try
            {
                invite.IsValid = false;
                invite.InviteeId = userId;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task AddNewInviteAsync(Invite invite)
        {
            try
            {
                await _context
                    .Invites
                    .AddAsync(invite);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                bool result = await _context
                    .Invites
                    .Where(i => i.CompanyId == companyId)
                    .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                return await _context
                    .Invites
                    .Where(i => i.CompanyId == companyId)
                    .Include(i => i.Company)
                    .Include(i => i.Project)
                    .Include(i => i.Invitor)
                    .FirstOrDefaultAsync(i => i.Id == inviteId);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                return await _context
                    .Invites
                    .Where(i => i.CompanyId == companyId)
                    .Include(i => i.Company)
                    .Include(i => i.Project)
                    .Include(i => i.Invitor)
                    .FirstOrDefaultAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> ValidateInviteCodeASync(Guid? token)
        {
            if (token is null)
            {
                return false;
            }

            bool result = false;
            Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

            if (invite is not null)
            {

                bool validDate = (DateTime.Now - invite.InviteDate.DateTime).TotalDays <= 7;
                if (validDate)
                {
                    result = invite.IsValid;
                }
            }

            return result;
        }
    }
}
