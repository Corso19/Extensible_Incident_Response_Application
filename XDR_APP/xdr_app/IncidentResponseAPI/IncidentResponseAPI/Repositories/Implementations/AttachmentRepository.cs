using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IncidentResponseAPI.Repositories.Implementations
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly IncidentResponseContext _context;

        public AttachmentRepository(IncidentResponseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttachmentModel>> GetAttachmentsByEventIdAsync(int eventId)
        {
            return await _context.Attachments
                .Where(a => a.EventId == eventId)
                .ToListAsync();
        }

        public async Task<AttachmentModel> GetAttachmentByIdAsync(int attachmentId)
        {
            return await _context.Attachments.FindAsync(attachmentId);
        }

        public async Task AddAttachmentAsync(AttachmentModel attachmentModel, CancellationToken cancellationToken)
        {
            await _context.Attachments.AddAsync(attachmentModel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAttachmentAsync(AttachmentModel attachmentModel)
        {
            _context.Attachments.Update(attachmentModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAttachmentAsync(int attachmentId)
        {
            var attachmentModel = await _context.Attachments.FindAsync(attachmentId);
            if (attachmentModel != null)
            {
                _context.Attachments.Remove(attachmentModel);
                await _context.SaveChangesAsync();
            }
        }
    }
}