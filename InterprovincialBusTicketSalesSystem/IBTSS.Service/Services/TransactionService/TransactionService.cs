using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Repository.Repositories.TicketRepository;
using IBTSS.Repository.Repositories.TransactionRepository;
using IBTSS.Service.DTO.Request.Transaction;
using IBTSS.Service.DTO.Response.Transaction;

namespace IBTSS.Service.Services.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly ITicketRepository _ticketRepository;

        public TransactionService(ITransactionRepository repository, ITicketRepository ticketRepository, IMapper mapper)
        {
            _repository = repository;
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<List<TransactionResponse>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return _mapper.Map<List<TransactionResponse>>(data);
        }
        public async Task<List<TransactionResponse>> GetByCustomerIdAsync(string customerId)
        {
            var transactions = await _repository.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<TransactionResponse>>(transactions);
        }

        public async Task<TransactionResponse?> GetByIdAsync(string id)
        {
            var transaction = await _repository.GetByIdAsync(id);
            return transaction == null ? null : _mapper.Map<TransactionResponse>(transaction);
        }

        public async Task<TransactionResponse> AddAsync(TransactionRequest request)
        {
            // Gọi trực tiếp repo, repo đã xử lý việc check book + gán ticket
            var transaction = await _repository.AddAsync(new Transaction
            {
                CustomerId = request.CustomerId
            });

            if (transaction == null)
                throw new Exception("Transaction could not be created. No pending booking found.");

            return _mapper.Map<TransactionResponse>(transaction);
        }



        public async Task<TransactionResponse?> UpdateAsync(string id, TransactionRequest request)
        {
            var updated = await _repository.UpdateAsync(id, _mapper.Map<Transaction>(request));
            return updated == null ? null : _mapper.Map<TransactionResponse>(updated);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
