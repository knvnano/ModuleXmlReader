using ModuleXmlReader.Domain;
namespace ModuleXmlReader.Presentation.Services;
public interface ISaveReviewService
{
 Task<bool> ConfirmSaveAsync(SaveReview review);
}
