using System.Threading.Tasks;
using BetSnooker.Models.API;
using BetSnooker.Repositories;

namespace BetSnooker.Services
{
    public interface IAdminService
    {
        Task<Event> GetCurrentEvent();
        Task<int> GetCurrentEventId();
        Task SetCurrentEvent(Event @event);
        Task SetStartRound(int roundId);
    }

    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<Event> GetCurrentEvent()
        {
            return await Task.Run(() => _adminRepository.GetCurrentEvent());
        }

        public async Task<int> GetCurrentEventId()
        {
            return (await GetCurrentEvent()).Id;
        }

        public async Task SetCurrentEvent(Event @event)
        {
            await _adminRepository.SetCurrentEvent(@event);
        }

        public async Task SetStartRound(int roundId)
        {
            await Task.Run(() => _adminRepository.SetStartRound(roundId));
        }
    }
}