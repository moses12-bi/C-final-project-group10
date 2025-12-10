using Core.Models;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class InvitationService : IInvitationService
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IEmailService _emailService;

    public InvitationService(
        AppDbContext context,
        UserManager<IdentityUser<Guid>> userManager,
        IEmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Invitation> CreateInvitationAsync(string email, string role, string department, Dictionary<string, bool> permissions, Guid invitedBy)
    {
        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            Role = role,
            Department = department,
            Permissions = permissions,
            Status = "Pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            InvitedBy = invitedBy
        };

        _context.Invitations.Add(invitation);
        await _context.SaveChangesAsync();

        // Send invitation email
        await _emailService.SendInvitationEmailAsync(email, invitation.Id);

        return invitation;
    }

    public async Task<Invitation?> GetInvitationAsync(Guid token)
    {
        return await _context.Invitations
            .Include(i => i.InvitedByUser)
            .FirstOrDefaultAsync(i => i.Id == token);
    }

    public async Task<bool> ValidateInvitationAsync(Guid token, string email)
    {
        var invitation = await GetInvitationAsync(token);
        
        if (invitation == null)
            return false;

        if (invitation.Email.ToLowerInvariant() != email.ToLowerInvariant())
            return false;

        return invitation.IsValid();
    }

    public async Task<bool> UseInvitationAsync(Guid token)
    {
        var invitation = await _context.Invitations.FindAsync(token);
        
        if (invitation == null || !invitation.IsValid())
            return false;

        invitation.Status = "Used";
        invitation.UsedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Invitation>> GetPendingInvitationsAsync()
    {
        return await _context.Invitations
            .Include(i => i.InvitedByUser)
            .Where(i => i.Status == "Pending" && i.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> RevokeInvitationAsync(Guid invitationId)
    {
        var invitation = await _context.Invitations.FindAsync(invitationId);
        
        if (invitation == null)
            return false;

        invitation.Status = "Expired";
        await _context.SaveChangesAsync();
        return true;
    }
}
