using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace StudyFlow.Application.ThamGiaLopHocs.Queries.GetHocSinhAssignedClass;
public class GetHocSinhAssignedClassValidator : AbstractValidator<GetHocSinhAssignedClassWithPaginationQuery>
{
    public GetHocSinhAssignedClassValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .NotNull();

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .NotNull();
    }
}
