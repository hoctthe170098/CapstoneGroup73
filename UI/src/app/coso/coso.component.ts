import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CoSo } from './shared/coso.model';
import { CoSoService } from './shared/coso.service';
import { ChangeDetectorRef } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
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

    constructor(
        private fb: FormBuilder,
        private coSoService: CoSoService,
        private cdr: ChangeDetectorRef,
        private toastr: ToastrService
    ) {
        this.addCampusForm = this.fb.group({
            ten: ['', [Validators.required, Validators.maxLength(30)]],
            diaChi: ['', [Validators.required, Validators.maxLength(50)]],
            soDienThoai: ['', [
                Validators.required,
                Validators.maxLength(11),
                Validators.pattern(/^0\d{9,10}$/)  // Bắt đầu bằng 0 và đủ độ dài 10-11 số
            ]]
        });

        this.editCampusForm = this.fb.group({
            ten: ['', Validators.required],
            diaChi: ['', Validators.required],
            soDienThoai: ['', [Validators.required, Validators.pattern(/^\d{10,11}$/)]],
            isActive: [false]  // dùng để bind với toggle switch
        });
    }

    ngOnInit(): void {
        this.loadDanhSachCoSo();
    }

    loadDanhSachCoSo() {
        this.coSoService.getDanhSachCoSo(this.currentPage, this.itemsPerPage, this.searchText)
            .subscribe(
                (res: any) => {
                    if (!res.isError) {
                        console.log('Dữ liệu nhận từ BE:', res.data.items);
                        this.campuses = res.data.items;
                        this.totalItems = res.data.totalCount;
                        this.cdr.detectChanges();
                    } else {
                        this.toastr.error(res.message)
                    }
                }, err => {
                    this.toastr.error('Có lỗi xảy ra, vui lòng thử lại!');
                });
    }

    openAddCampusModal() {
        this.addCampusForm.reset();
        this.isModalOpen = true;
        this.addCampusForm.markAllAsTouched();
    }
    openEditCampusModal(campus: CoSo) {
        this.selectedCampus = { ...campus };

        const isActive = campus.trangThai === 'open';  // map trạng thái thành boolean

        this.editCampusForm.patchValue({
            ten: campus.ten,
            diaChi: campus.diaChi,
            soDienThoai: campus.soDienThoai,
            isActive: isActive
        });

        this.isEditModalOpen = true;
    }

    saveCampus() {
        if (this.addCampusForm.invalid) {
            this.addCampusForm.markAllAsTouched();  // Đánh dấu tất cả field để hiển thị lỗi
            return;
        }

        const newCampus = this.addCampusForm.value;

        this.coSoService.createCoSo(newCampus).subscribe({
            next: (res) => {
                if (!res.isError) {
                    this.toastr.success(res.message)
                    this.closeModal();
                    this.loadDanhSachCoSo();
                }else
                this.toastr.error(res.message)
            },
            error: (err) => {
                console.error('Lỗi khi tạo cơ sở:', err);
                this.toastr.error('Có lỗi xảy, vui lòng thử lại!');
            }
        });
    }
    updateCampus() {
        if (this.editCampusForm.valid && this.selectedCampus) {
            const updatedCampus = {
                ...this.selectedCampus,
                ten: this.editCampusForm.value.ten,
                diaChi: this.editCampusForm.value.diaChi,
                soDienThoai: this.editCampusForm.value.soDienThoai,
                trangThai: this.editCampusForm.value.isActive ? 'open' : 'close'  // map ngược lại khi gửi lên
            };
            this.coSoService.updateCoSo(updatedCampus).subscribe({
                next: (res) => {
                    if (!res.isError) {
                        this.toastr.success(res.message)
                        this.closeEditModal();
                        this.loadDanhSachCoSo();
                    }else
                    this.toastr.error(res.message)
                },
                error: (err) => {
                    this.toastr.error('Có lỗi xảy, vui lòng thử lại!');
                }
            });
        }
    }

    updateDefaultCampus(campus: CoSo, isChecked: boolean) {
        campus.default = isChecked;

        if (isChecked) {
            this.campuses.forEach(c => {
                if (c.id !== campus.id) {
                    c.default = false;
                }
            });
        }
        this.coSoService.updateCoSo(campus).subscribe(() => {
            this.loadDanhSachCoSo();
        });
    }

    closeModal() { this.isModalOpen = false; }
    closeEditModal() { this.isEditModalOpen = false; }
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
    get f() {
        return this.addCampusForm.controls;
    }

}
