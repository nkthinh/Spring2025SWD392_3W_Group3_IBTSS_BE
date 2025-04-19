using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Repository.Repositories.TransactionRepository;
using IBTSS.Service.DTO.Request.Transaction;
using IBTSS.Service.DTO.Response.Transaction;

namespace IBTSS.Service.Services.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<TransactionResponse>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return _mapper.Map<List<TransactionResponse>>(data);
        }

        public async Task<TransactionResponse?> GetByIdAsync(string id)
        {
            var transaction = await _repository.GetByIdAsync(id);
            return transaction == null ? null : _mapper.Map<TransactionResponse>(transaction);
        }

        public async Task<TransactionResponse> AddAsync(TransactionRequest request)
        {
            var entity = _mapper.Map<Transaction>(request);
            var created = await _repository.AddAsync(entity);
            return _mapper.Map<TransactionResponse>(created);
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
