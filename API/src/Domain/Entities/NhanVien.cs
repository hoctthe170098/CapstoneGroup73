﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using StudyFlow.Domain.Interfaces;


namespace StudyFlow.Domain.Entities;
public class NhanVien
{
    public required string Code { get; set; }
    public required string Ten { get; set; }
    public required string GioiTinh { get; set; }
    public required string DiaChi { get; set; }
    public required DateOnly NgaySinh { get; set; }
    public required string Email { get; set; }
    public required string SoDienThoai { get; set; }
    public required Guid CoSoId { get; set; }
    [JsonIgnore]
    public CoSo Coso { get; set; } = null!;
    public string? UserId { get; set; }
}
