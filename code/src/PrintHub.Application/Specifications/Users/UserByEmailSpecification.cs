using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.Users;

/// <summary>Finds a user by email (case-insensitive at the collation level).</summary>
public sealed class UserByEmailSpecification : BaseSpecification<User>
{
    public UserByEmailSpecification(string email)
        : base(u => u.Email == email)
    {
    }
}
