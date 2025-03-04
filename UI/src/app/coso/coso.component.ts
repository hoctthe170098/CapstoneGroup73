import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
@Component({
  selector: 'app-coso',
  templateUrl: './coso.component.html',
  styleUrls: ['./coso.component.scss']
})
export class CosoComponent {
  campuses = [
    { id: 1, name: 'Hoàng Văn Thái', address: '24 Hoàng Văn Thái, phường Thanh Xuân, TP Hà Nội', phone: '0123-456-789', createdAt: '23-11-2012', isActive: false, isDefault: false },
    { id: 2, name: 'A', address: '24 Hoàng Văn Thái, phường Thanh Xuân, TP Hà Nội', phone: '0123-456-789', createdAt: '23-11-2012', isActive: false, isDefault: false },
    { id: 3, name: 'B', address: '24 Hoàng Văn Thái, phường Thanh Xuân, TP Hà Nội', phone: '0123-456-789', createdAt: '23-11-2012', isActive: false, isDefault: false },
    { id: 4, name: 'C', address: '24 Hoàng Văn Thái, phường Thanh Xuân, TP Hà Nội', phone: '0123-456-789', createdAt: '23-11-2012', isActive: false, isDefault: false },
    { id: 5, name: 'D', address: '24 Hoàng Văn Thái, phường Thanh Xuân, TP Hà Nội', phone: '0123-456-789', createdAt: '23-11-2012', isActive: false, isDefault: false },
    { id: 6, name: 'Nguyễn Trãi', address: '55 Nguyễn Trãi, TP Hà Nội', phone: '0987-654-321', createdAt: '10-05-2015', isActive: false, isDefault: false }
  ];
  isModalOpen = false;
  isEditModalOpen = false;
  selectedCampus: any = null;
  itemsPerPage = 6;
  currentPage = 1;

  addCampusForm: FormGroup;
  editCampusForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.addCampusForm = this.fb.group({
      name: ['', Validators.required],
      address: ['', Validators.required],
      phone: ['', [Validators.required, Validators.pattern(/^\d{10,11}$/)]],
      isActive: [false]
    });

    this.editCampusForm = this.fb.group({
      name: ['', Validators.required],
      address: ['', Validators.required],
      phone: ['', [Validators.required, Validators.pattern(/^\d{10,11}$/)]],
      isActive: [false]
    });
  }

  openModal() {
    this.isModalOpen = true;
  }

  openAddCampusModal() {
    this.addCampusForm.reset();
    this.openModal();
  }

  openEditCampusModal(campus: any) {
    this.selectedCampus = { ...campus };
    this.editCampusForm.patchValue(this.selectedCampus);
    this.isEditModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  closeEditModal() {
    this.isEditModalOpen = false;
  }

  saveCampus() {
    if (this.addCampusForm.valid) {
      const newCampus = {
        id: this.campuses.length + 1,
        ...this.addCampusForm.value,
        createdAt: new Date().toLocaleDateString(),
        isDefault: false
      };
      this.campuses.push(newCampus);
      this.closeModal();
    }
  }

  updateCampus(updatedCampus: any) {
    if (this.editCampusForm.valid && this.selectedCampus) {
      const index = this.campuses.findIndex(c => c.id === this.selectedCampus.id);
      if (index !== -1) {
        this.campuses[index] = { ...this.selectedCampus, ...updatedCampus }; // Cập nhật toàn bộ dữ liệu, bao gồm isActive
        this.closeEditModal();
      }
    }
  }

  updateDefaultCampus(campus: any) {
    this.campuses.forEach(c => c.isDefault = c === campus);
  }

  get paginatedCampuses() {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.campuses.slice(startIndex, endIndex);
  }

  changePage(page: number) {
    this.currentPage = page;
  }

  get totalPages() {
    return Math.ceil(this.campuses.length / this.itemsPerPage);
  }
}
