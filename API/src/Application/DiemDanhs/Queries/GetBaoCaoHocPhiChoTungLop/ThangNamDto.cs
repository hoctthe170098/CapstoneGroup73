using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyFlow.Application.DiemDanhs.Queries.GetBaoCaoHocPhiChoTungLop;
public class ThangNamDto
{
    public required int Thang {  get; set; }
    public required int Nam {  get; set; }
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        ThangNamDto other = (ThangNamDto)obj;
        return (Thang == other.Thang) && (Nam == other.Nam);
    }

    // Ghi đè phương thức GetHashCode() - Rất quan trọng để Dictionary hoạt động đúng
    public override int GetHashCode()
    {
        return HashCode.Combine(Thang, Nam);
    }
}
