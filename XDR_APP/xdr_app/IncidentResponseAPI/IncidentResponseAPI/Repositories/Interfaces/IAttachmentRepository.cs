using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Repositories.Interfaces;

public interface IAttachmentRepository
{
    Task<IEnumerable<AttachmentModel>> GetAttachmentsByEventIdAsync(int eventId);
    Task<AttachmentModel> GetAttachmentByIdAsync(int attachmentId);
    Task AddAttachmentAsync(AttachmentModel attachmentModel, CancellationToken cancellationToken);
    Task UpdateAttachmentAsync(AttachmentModel attachmentModel);
    Task DeleteAttachmentAsync(int attachmentId);
    
}