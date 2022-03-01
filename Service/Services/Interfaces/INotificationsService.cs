using System.Threading.Tasks;
using BetSnooker.Models;

namespace BetSnooker.Services.Interfaces
{
    public interface INotificationsService
    {
        Task AddSubscription(NotificationSubscription subscription);
        Task SendNotification(string payload);
    }
}