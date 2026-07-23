using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Users;

public sealed class UserSearchSpecification : BaseSpecification<User>
{
    public UserSearchSpecification(string? q, UserRole? role, UserStatus? status, int skip, int take)
        : base(u => (string.IsNullOrEmpty(q) || u.FullName.Contains(q) || u.Email.Contains(q))
                    && (role == null || u.Role == role)
                    && (status == null || u.Status == status))
    {
        ApplyOrderBy(u => u.Id);
        ApplyPaging(skip, take);
    }
}

public sealed class UserSearchCountSpecification : BaseSpecification<User>
{
    public UserSearchCountSpecification(string? q, UserRole? role, UserStatus? status)
        : base(u => (string.IsNullOrEmpty(q) || u.FullName.Contains(q) || u.Email.Contains(q))
                    && (role == null || u.Role == role)
                    && (status == null || u.Status == status))
    {
    }
}
