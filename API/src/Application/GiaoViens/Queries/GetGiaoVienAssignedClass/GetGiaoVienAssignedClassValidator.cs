using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyFlow.Application.GiaoViens.Queries.GetGiaoVienAssignedClass;

namespace StudyFlow.Application.GiaoViens.Commands.GetGiaoVienAssignedClass;
public class GetGiaoVienAssignedClassValidator : AbstractValidator<GetGiaoVienAssignedClassWithPaginationCommand>
{
    public GetGiaoVienAssignedClassValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        //RuleFor(x => x)
        //    .Must(x =>
        //    {
        //        if (!DateTime.TryParse(x.StartDate, out var startDate)) return false;
        //        if (!DateTime.TryParse(x.EndDate, out var endDate)) return false;

        //        return startDate <= endDate;
        //    })
        //    .WithMessage("Start date must be less than or equal to end date.");
    }
}
