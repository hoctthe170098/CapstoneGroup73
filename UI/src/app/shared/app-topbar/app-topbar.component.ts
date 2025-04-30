import { Component, Output, EventEmitter, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-topbar',
  templateUrl: './app-topbar.component.html',
  styleUrls: ['./app-topbar.component.scss']
})
export class AppTopbarComponent {
  isDropdownOpen = false;
  searchText = ''; // Thuộc tính để lưu giá trị tìm kiếm

  @Output() seachTextEmpty = new EventEmitter<boolean>(); // Event để phát trạng thái tìm kiếm

  toggleDropdown(event: Event) {
    event.stopPropagation(); // Ngăn chặn sự kiện click lan ra ngoài
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  handleClickOutside(event: Event) {
    if (!this.isDropdownOpen) return;
    const target = event.target as HTMLElement;
    if (!target.closest('.admin-profile') && !target.closest('.dropdown-menu')) {
      this.isDropdownOpen = false;
    }
  }

  @HostListener('document:click', ['$event']) // Thêm sự kiện click trên document
  onDocumentClick(event: Event) {
    this.handleClickOutside(event);
  }

  changePassword(event: Event) {
    event.preventDefault(); 
    console.log('Change Password clicked');
    alert('Change Password');
  }

  signOut(event: Event) {
    event.preventDefault(); 
    alert('Sign out');
  }

  onSearchChange() {
    // Phát event khi giá trị tìm kiếm thay đổi
    this.seachTextEmpty.emit(this.searchText.trim() === '');
  }
}