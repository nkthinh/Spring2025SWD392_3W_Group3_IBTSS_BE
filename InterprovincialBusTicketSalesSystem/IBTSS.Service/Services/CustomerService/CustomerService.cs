    using AutoMapper;
    using IBTSS.Repository.Entities;
    using IBTSS.Repository.UnitOfWork;
    using IBTSS.Service.DTO.Request.Customer;
    using IBTSS.Service.DTO.Response.Customer;
    using Microsoft.Extensions.Logging;
    using System.Security.Cryptography;
    using System.Text;

    namespace IBTSS.Service.Services.CustomerService
    {
        public class CustomerService : ICustomerService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<CustomerService> _logger;
            private readonly IMapper _mapper;

            public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<IEnumerable<Customer>> GetAllAsync()
            {
                try
                {
                    _logger.LogInformation("Fetching all Customers (including deleted)");
                    return await _unitOfWork.Customers.GetAllAsync(); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching Customers.");
                    throw new Exception("An error occurred while retrieving Customers.", ex);
                }
            }


            public async Task<Customer?> GetByIdAsync(string id)
            {
                try
                {
                    return await _unitOfWork.Customers.GetByIdAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error fetching Customer with id: {id}");
                    throw;
                }
            }

            public async Task AddAsync(Customer c)
            {
                try
                {
                    _logger.LogInformation("Adding a new Customer");
                    await _unitOfWork.Customers.AddAsync(c);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding Customer");
                    throw;
                }
            }

            public async Task UpdateAsync(Customer c)
            {
                try
                {
                    // ✅ Tính tổng số vé đã hoàn thành (không huỷ)
                    var tickets = await _unitOfWork.Tickets.GetAllAsync();
                    var completedTickets = tickets.Count(t =>
                        t.Book != null &&
                        t.Book.CustomerId == c.CustomerId &&
                        !t.isCancelled);

                    // ✅ Lấy tất cả Membership
                    var memberships = await _unitOfWork.Memberships.GetAllAsync();

                    // ✅ Sắp xếp theo độ ưu tiên rank cao trước
                    var eligibleMembership = memberships
                        .OrderByDescending(m => m.MinTicketsRequired)
                        .FirstOrDefault(m => completedTickets >= m.MinTicketsRequired);

                    if (eligibleMembership != null)
                    {
                        // Nếu khách chưa có hạng hoặc hạng mới cao hơn → lên hạng
                        if (c.MembershipId == null || c.MembershipId != eligibleMembership.MembershipId)
                        {
                            c.MembershipId = eligibleMembership.MembershipId;
                            c.DiscountQuotaLeft = 5;
                            _logger.LogInformation($"Customer {c.CustomerId} promoted to {eligibleMembership.RankName} with 5 discount quota.");
                        }
                        // Nếu đã có hạng nhưng chưa có quota, thì bổ sung quota
                        else if (c.DiscountQuotaLeft == null)
                        {
                            c.DiscountQuotaLeft = 5;
                            _logger.LogInformation($"Customer {c.CustomerId} already has rank {eligibleMembership.RankName}, quota initialized.");
                        }
                    }

                    _logger.LogInformation($"Updating Customer {c.CustomerId}");
                    await _unitOfWork.Customers.UpdateAsync(c);
                    await _unitOfWork.CompleteAsync(); // ✅ QUAN TRỌNG: Save thay đổi
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating Customer {c.CustomerId}");
                    throw;
                }
            }

            public async Task DeleteAsync(string id)
            {
                try
                {
                    _logger.LogInformation($"Soft deleting Customer {id}");
                    var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                    if (customer == null)
                        throw new Exception($"Customer with id {id} not found.");

                    customer.IsDelete = true;
                    await _unitOfWork.Customers.UpdateAsync(customer);
                    await _unitOfWork.CompleteAsync(); // save thay đổi
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error soft deleting Customer {id}");
                    throw;
                }
            }


            public async Task<bool> GetByPhoneNumberAsync(string phoneNumber)
            {
                try
                {
                    return await _unitOfWork.Customers.GetByPhoneNumberAsync(phoneNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking phone number");
                    throw;
                }
            }

            public async Task<Customer?> LoginByPhoneAsync(string phoneNumber, string password)
            {
                try
                {
                    return await _unitOfWork.Customers.LoginByPhoneAsync(phoneNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "PhoneNumber is not exist!");
                    throw;
                }
            }
        public async Task<Customer?> AuthenticateAsync(string phoneNumber, string password)
        {
            try
            {
                var customer = await _unitOfWork.Customers.LoginByPhoneAsync(phoneNumber);
                if (customer == null || customer.IsDelete)
                    return null;

                var hash = HashPassword(password);
                return customer.PasswordHash == hash ? customer : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication failed");
                throw;
            }
        }

        public string HashPassword(string password)
            {
                using var sha256 = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }

            public async Task<(List<CustomerResponse>, int)> GetFilteredAsync(CustomerQueryParameters query)
            {
                var customers = await _unitOfWork.Customers.GetAllAsync();
                var filtered = customers.AsQueryable(); //Không Where IsDelete

                if (!string.IsNullOrEmpty(query.Keyword))
                {
                    filtered = filtered.Where(c =>
                        (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(query.Keyword))
                    );
                }

                filtered = query.SortBy switch
                {
                    "name_desc" => filtered.OrderByDescending(c => c.Name),
                    "score_desc" => filtered.OrderByDescending(c => c.Score),
                    "membership_asc" => filtered.OrderBy(c => c.MembershipId),
                    "membership_desc" => filtered.OrderByDescending(c => c.MembershipId),
                    _ => filtered.OrderBy(c => c.Name)
                };

                var total = filtered.Count();

                if (query.PageSize == -1)
                {
                    var allMapped = _mapper.Map<List<CustomerResponse>>(filtered.ToList());
                    return (allMapped, allMapped.Count);
                }

                var result = filtered
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                var mapped = _mapper.Map<List<CustomerResponse>>(result);
                return (mapped, total);
            }

        }
    }
