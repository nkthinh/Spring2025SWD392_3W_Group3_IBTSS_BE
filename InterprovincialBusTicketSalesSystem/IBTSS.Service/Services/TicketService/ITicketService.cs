using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Ticket;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.TicketService
{
    public interface ITicketService
    {
        Task<List<TicketResponse>> GetAllAsync();
        Task<TicketResponse?> GetByIdAsync(string id);
        Task<TicketResponse> AddAsync(TicketRequest request);
        Task<TicketResponse?> UpdateAsync(string id, TicketRequest request);
        Task<bool> DeleteAsync(string id);
        Task<List<TicketResponse>> AddMultipleAsync(MultiTicketRequest request);
        Task<List<TicketResponse>> GetByCustomerIdAsync(string customerId);
        Task<TicketResponse?> CancelTicketAsync(string ticketId);
        Task<bool> ConfirmBoardingAsync(string ticketId);
        Task<bool> ChangeSeatAsync(string ticketId, string newSeatId);
        Task<(List<TicketResponse>, int)> GetFilteredAsync(QueryParameters query);

    }
}
