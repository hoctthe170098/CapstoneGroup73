    import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
    import { FormBuilder, FormGroup, Validators } from '@angular/forms';
    import { CoSo } from './shared/coso.model';
    import { CoSoService } from './shared/coso.service';
    import { ToastrService } from 'ngx-toastr';
    import { Router } from '@angular/router';
    @Component({
        selector: 'app-coso',
        templateUrl: './coso.component.html',
        styleUrls: ['./coso.component.scss']
    })
    export class CosoComponent implements OnInit {
        campuses: CoSo[] = [];
        isModalOpen = false;
        isEditModalOpen = false;
        selectedCampus: CoSo | null = null;

        itemsPerPage = 6;
        currentPage = 1;
        totalItems = 0;
        searchText = '';

        addCampusForm: FormGroup;
        editCampusForm: FormGroup;

        provinces: any[] = [];  
        districts: any[] = [];  
        editDistricts: any[] = [];  

        constructor(
            private fb: FormBuilder,
            private coSoService: CoSoService,
            private cdr: ChangeDetectorRef,
            private toastr: ToastrService,
            private router: Router
        ) {
            this.addCampusForm = this.fb.group({
                ten: ['', [Validators.required, Validators.maxLength(30)]],
                diaChi: ['', [Validators.required, Validators.maxLength(50)]],
                soDienThoai: ['', [
                    Validators.required,
                    Validators.maxLength(11),
                    Validators.pattern(/^0\d{9,10}$/)
                ]],
                province: ['', Validators.required],
                district: ['', Validators.required]
            });

            this.editCampusForm = this.fb.group({
                ten: ['', [Validators.required, Validators.maxLength(30)]],
                diaChi: ['', [Validators.required, Validators.maxLength(50)]],
                soDienThoai: ['', [Validators.required, Validators.pattern(/^\d{10,11}$/)]],
                province: ['', Validators.required],
                district: ['', Validators.required],
                isActive: [false]
            });
        }

        ngOnInit(): void {
            this.loadDanhSachCoSo();
            this.loadProvinces();
        }

        getProvinceName(provinceCode: string): string {
            const province = this.provinces.find(p => p.code == provinceCode);
            return province ? province.name : '';
        }

        getDistrictName(districtCode: string): string {
            const district = this.districts.find(d => d.code == districtCode);
            return district ? district.name : '';
        }
        extractAddressParts(diaChi: string): { province: string, district: string, detail: string } {
            const parts = diaChi.split(', ').map(part => part.trim());
            return {
                province: parts[0] || '',
                district: parts[1] || '',
                detail: parts.slice(2).join(', ') || ''
            };
        }
        
        // Lấy danh sách cơ sở từ server
        loadDanhSachCoSo() {
            this.coSoService.getDanhSachCoSo(this.currentPage, this.itemsPerPage, this.searchText)
                .subscribe(
                    (res: any) => {
                        if (res.code === 404) {
                            this.router.navigate(['/pages/error'])
                            return;
                          }
                        if (!res.isError) {
                            this.campuses = res.data.items;
                            this.totalItems = res.data.totalCount;
                            this.cdr.detectChanges();
                        } else {
                            this.toastr.error(res.message);
                        }
                    }, err => {
                        this.toastr.error('Có lỗi xảy ra, vui lòng thử lại!');
                    });
        }

        // Lấy danh sách tỉnh/thành phố
        loadProvinces() {
            this.coSoService.getProvinces().subscribe(
                data => {
                    this.provinces = data;
                },
                error => {
                    console.error('Lỗi tải tỉnh/thành phố:', error);
                }
            );
        }
        

        // Khi chọn tỉnh/thành phố -> cập nhật quận/huyện
        onProvinceChange(provinceCode: string) {
        
            const selectedProvince = this.provinces.find(p => p.code == provinceCode); // Kiểm tra kiểu dữ liệu
        
            if (selectedProvince) {
                this.districts = selectedProvince.districts;
            } else {
                this.districts = [];
            }
        
            this.addCampusForm.patchValue({ district: '' }); // Reset quận/huyện khi đổi tỉnh
        }
        
        

        // Khi chọn tỉnh/thành phố trong modal chỉnh sửa -> cập nhật quận/huyện
        onProvinceChangeForEdit(provinceCode: string) {
        
            const selectedProvince = this.provinces.find(p => String(p.code) === String(provinceCode));
        
            if (selectedProvince) {
                this.editDistricts = selectedProvince.districts;
            } else {
                console.warn("Không tìm thấy tỉnh/thành phố trong danh sách Edit!");
                this.editDistricts = [];
            }
        
            this.editCampusForm.patchValue({ district: '' }); // Reset dropdown quận/huyện
        }
        

        // Mở modal thêm cơ sở
        openAddCampusModal() {
            this.addCampusForm.reset({
              ten: '',
              diaChi: '',
              soDienThoai: '',
              province: '',
              district: ''
            });
            this.isModalOpen = true;
          }
          

        // Mở modal chỉnh sửa cơ sở
        openEditCampusModal(campus: CoSo) {
            this.selectedCampus = { ...campus };
            const isActive = campus.trangThai === 'open';

            const addressParts = this.extractAddressParts(campus.diaChi);
            const province = this.provinces.find(p => p.name === addressParts.province);
            const provinceCode = province ? province.code : '';

            this.onProvinceChangeForEdit(provinceCode);

            const district = province?.districts.find(d => d.name === addressParts.district);
            const districtCode = district ? district.code : '';

            this.editCampusForm.patchValue({
                ten: campus.ten,
                diaChi: addressParts.detail,
                soDienThoai: campus.soDienThoai,
                province: provinceCode,
                district: districtCode,
                isActive: isActive
            });

            this.isEditModalOpen = true;
        }
        

        // Lưu cơ sở mới
        saveCampus() {
            if (this.addCampusForm.invalid) {
                this.addCampusForm.markAllAsTouched();
                return;
            }

            const formData = this.addCampusForm.value;
            const diaChiFormatted = `${this.getProvinceName(formData.province)}, ${this.getDistrictName(formData.district)}, ${formData.diaChi}`;

            const newCampus: CoSo = {
                id: '',
                ten: formData.ten,
                diaChi: diaChiFormatted,
                soDienThoai: formData.soDienThoai,
                province: formData.province,
                district: formData.district,
                trangThai: 'open',
                default: false
            };

            this.coSoService.createCoSo(newCampus).subscribe({
                next: (res) => {
                    if (!res.isError) {
                        this.toastr.success(res.message);
                        this.closeModal();
                        this.loadDanhSachCoSo();
                    } else {
                        this.toastr.error(res.message);
                    }
                },
                error: () => {
                    this.toastr.error('Có lỗi xảy ra, vui lòng thử lại!');
                }
            });
        }

        // Cập nhật cơ sở
        updateCampus() {
            if (this.editCampusForm.valid && this.selectedCampus) {
              const formData = this.editCampusForm.value;
          
              const selectedProvince = this.provinces.find(p => String(p.code) === String(formData.province));
              const selectedDistrict = this.editDistricts.find(d => String(d.code) === String(formData.district));
          
              const diaChiFormatted = `${selectedProvince?.name || ''}, ${selectedDistrict?.name || ''}, ${formData.diaChi}`;
          
              const updatedCampus: CoSo = {
                ...this.selectedCampus,
                ten: formData.ten,
                diaChi: diaChiFormatted,
                soDienThoai: formData.soDienThoai,
                province: formData.province,
                district: formData.district,
                trangThai: formData.isActive ? 'open' : 'close'
              };
          
              this.coSoService.updateCoSo(updatedCampus).subscribe({
                next: (res) => {
                  if (!res.isError) {
                    this.toastr.success(res.message);
                    this.closeEditModal();
                    this.loadDanhSachCoSo();
                  } else {
                    this.toastr.error(res.message);
                  }
                },
                error: () => {
                  this.toastr.error('Có lỗi xảy ra, vui lòng thử lại!');
                }
              });
            } else {
              this.editCampusForm.markAllAsTouched();
            }
          }
          

        // Đóng modal
        closeModal() { this.isModalOpen = false; }
        closeEditModal() { this.isEditModalOpen = false; }

        // Phân trang
        changePage(page: number) { this.currentPage = page; this.loadDanhSachCoSo(); }
        searchCampus() { this.currentPage = 1; this.loadDanhSachCoSo(); }
        get totalPages() { return Math.ceil(this.totalItems / this.itemsPerPage); }
        getPageNumbers(): number[] {
            const pages: number[] = [];
            for (let i = 1; i <= this.totalPages; i++) {
                pages.push(i);
            }
            return pages;
        }
    }
