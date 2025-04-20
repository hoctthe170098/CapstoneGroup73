import { Component, OnInit, ChangeDetectorRef } from "@angular/core";
import { LophocService } from "./shared/lophoc.service";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { NgxSpinnerService } from "ngx-spinner";
@Component({
  selector: "app-lophoc",
  templateUrl: "./lophoc.component.html",
  styleUrls: ["./lophoc.component.scss"],
})
export class LophocComponent implements OnInit {
  thuTrongTuan = {
    thu2: false,
    thu3: false,
    thu4: false,
    thu5: false,
    thu6: false,
    thu7: false,
    cn: false,
  };

  chuongTrinh: string = "";
  trangThai: string = "";
  timeStart: string = "";
  timeEnd: string = "";
  dateStart: string = "";
  dateEnd: string = "";
  searchTerm: string = "";
  chuongTrinhOptions: any[] = [];
  lophocs: any[] = [];
  chuongTrinhId: number = 0;
  currentPage: number = 1;
  totalPages: number = 1;
  pageSize: number = 3;

  giaoVienSearch: string = "";
  giaoVienOptions: { code: string; codeTen: string }[] = [];
  filteredGiaoVienOptions: { code: string; codeTen: string }[] = [];
  selectedGiaoVien: string = "";
  isGiaoVienDropdownOpen: boolean = false;
  phongOptions: { id: string; ten: string }[] = [];
  popupGiaoVienOptions: { code: string; codeTen: string }[] = [];
  popupFilteredGiaoVienOptions: { code: string; codeTen: string }[] = [];
  popupGiaoVienSearch: string = "";
  isPopupGiaoVienDropdownOpen: boolean = false;
  isAddScheduleModalOpen: boolean = false;
  showExtraFields: boolean = false;
  isEditScheduleModalOpen: boolean = false;
editingLichDayThay: any = null; 
isPopupGiaoVienDropdownOpen_Edit: boolean = false;
popupFilteredGiaoVienOptions_Edit: { code: string, codeTen: string }[] = [];
newScheduleEdit: any = {
  tenLop: '',
  giaoVienCode: '',
  ngayBatDau: ''
};
isEditGiaoVienDropdownOpen = false;
editGiaoVienSearch = '';
editSelectedGiaoVien: any = null;
editFilteredGiaoVienOptions: any[] = [];

isEditDayBuModalOpen = false;
editScheduleDayBu: any = {
  tenLop: '',
  ngayNghi: '',
  ngay: '',
  phong: '',
  batDau: '',
  ketThuc: ''
};
  constructor(
    private lophocService: LophocService,
    private cdr: ChangeDetectorRef,
    private router: Router,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnInit(): void {
    this.getChuongTrinhs();
    this.loadLopHocs();
    this.getGiaoVienOptions();
    this.loadPhongs(); 
    document.addEventListener("click", (event: any) => {
      const isInside = event.target.closest(".action-plus-wrapper");
      if (!isInside) {
        this.closeAllActionMenus();
      }
    });
    document.addEventListener("click", this.handleClickOutsideEditDropdown.bind(this));
  }
  handleClickOutsideEditDropdown(event: any) {
    const dropdown = document.querySelector('.edit-giaovien-dropdown-wrapper');
    if (dropdown && !dropdown.contains(event.target as Node)) {
      this.isEditGiaoVienDropdownOpen = false;
    }
  }
  toggleActionMenu(index: number, event: MouseEvent) {
    event.stopPropagation();
    this.lophocs.forEach((_, i) => {
      this.lophocs[i].showActionMenu =
        i === index ? !this.lophocs[i].showActionMenu : false;
    });
  }
  handleClickOutsidePopupDropdown(event: any) {
    const dropdown = document.querySelector('.popup-giaovien-dropdown-wrapper');
    if (dropdown && !dropdown.contains(event.target as Node)) {
      this.isPopupGiaoVienDropdownOpen = false;
    }
  }
  closeAllActionMenus() {
    this.lophocs.forEach((l) => (l.showActionMenu = false));
  }

  onActionClick(action: string, lop: any) {
    this.closeAllActionMenus();
    alert(`Bạn đã chọn: ${action} cho lớp ${lop.tenLop}`);
    // TODO: điều hướng hoặc mở form tương ứng
  }

  newSchedule = {
    loaiLich: "",
    ngay: "",
    ngayNghi: "", 
    phong: "",
    batDau: "",
    ketThuc: "",
    giaoVienCode: "",
    lop: null,
  };
  openAddScheduleModal(lop: any, loai: string) {
    this.newSchedule = {
      loaiLich: loai,
      lop: lop,
      ngay: '',
      ngayNghi: '',
      phong: '',
      batDau: '',
      ketThuc: '',
       giaoVienCode: ''
    };
    this.isAddScheduleModalOpen = true;
    this.showExtraFields = false;
  
    if (loai === 'Dạy bù') {
      this.loadPhongs();
    } else if (loai === 'Dạy thay') {
      this.loadPopupGiaoViens(); 
    }
  }

  closeAddScheduleModal() {
    this.isAddScheduleModalOpen = false;
  }

  loadPhongs() {
    this.lophocService.getPhongs().subscribe({
      next: (res) => {
        this.phongOptions = !res.isError && res.data ? res.data : [];
      },
      error: () => {
        this.phongOptions = [];
      },
    });
  }
  // Giao viên trong popup
  loadPopupGiaoViens() {
    this.lophocService.getGiaoViens({ searchTen: "" }).subscribe({
      next: (res) => {
        this.popupGiaoVienOptions = res.data || [];
        this.popupFilteredGiaoVienOptions = res.data.slice();
      },
      error: () => {
        this.popupGiaoVienOptions = [];
        this.popupFilteredGiaoVienOptions = [];
      },
    });
  }

  togglePopupGiaoVienDropdown(event: MouseEvent) {
    event.stopPropagation();
    this.isPopupGiaoVienDropdownOpen = !this.isPopupGiaoVienDropdownOpen;
  }
  togglePopupGiaoVienDropdown_Edit(event: Event) {
    event.stopPropagation();
    this.isPopupGiaoVienDropdownOpen_Edit = !this.isPopupGiaoVienDropdownOpen_Edit;
  }

  getSelectedPopupGiaoVienText(): string {
    const selected = this.popupGiaoVienOptions.find(
      (gv) => gv.code === this.newSchedule.giaoVienCode
    );
    return selected ? selected.codeTen : "";
  }

  onPopupGiaoVienSearch() {
    const search = this.popupGiaoVienSearch.toLowerCase();
    this.popupFilteredGiaoVienOptions = this.popupGiaoVienOptions.filter((gv) =>
      gv.codeTen.toLowerCase().includes(search)
    );
  }
  onPopupGiaoVienSearch_Edit() {
    const search = this.popupGiaoVienSearch.toLowerCase();
    this.popupFilteredGiaoVienOptions_Edit = this.giaoVienOptions.filter(gv =>
      gv.codeTen.toLowerCase().includes(search)
    );
  }
  selectPopupGiaoVien_Edit(gv: { code: string, codeTen: string }) {
    this.editingLichDayThay.giaoVienCode = gv.code;
    this.popupGiaoVienSearch = '';
    this.popupFilteredGiaoVienOptions_Edit = this.giaoVienOptions.slice();
    this.isPopupGiaoVienDropdownOpen_Edit = false;
  }
  selectPopupGiaoVien(gv: { code: string, codeTen: string }) {
    this.newSchedule.giaoVienCode = gv.code;
    this.popupGiaoVienSearch = ''; 
    this.popupFilteredGiaoVienOptions = this.popupGiaoVienOptions.slice(); 
    this.isPopupGiaoVienDropdownOpen = false; 
  }
  

  submitNewSchedule() {
    if (this.newSchedule.loaiLich === 'Dạy thay') {
      const payload = {
        tenLop: this.newSchedule.lop?.tenLop,
        ngayDay: this.newSchedule.ngay,
        giaoVienCode: this.newSchedule.giaoVienCode
      };
  
      this.lophocService.createLichDayThay(payload).subscribe({
        next: (res) => {
          if (!res.isError) {
            this.toastr.success(res.message || 'Thêm lịch dạy thay thành công!');
            this.closeAddScheduleModal();
            this.loadLopHocs(); 
          } else {
            this.toastr.error(res.message || 'Thêm lịch dạy thay thất bại!');
          }
        },
        error: (err) => {
          console.error('Lỗi khi tạo lịch dạy thay:', err);
          this.toastr.error('Không thể tạo lịch dạy thay!');
        }
      });
    } else if (this.newSchedule.loaiLich === 'Dạy bù') {
      const payload: any = {
        tenLop: this.newSchedule.lop?.tenLop,
        ngayNghi: this.newSchedule.ngayNghi
      };
    
      // Nếu người dùng nhập thêm lịch học bù
      if (this.newSchedule.ngay && this.newSchedule.phong && this.newSchedule.batDau && this.newSchedule.ketThuc) {
        const [startHour, startMinute] = this.newSchedule.batDau.split(":").map(Number);
        const [endHour, endMinute] = this.newSchedule.ketThuc.split(":").map(Number);
    
        const startTime = startHour * 60 + startMinute;
        const endTime = endHour * 60 + endMinute;
    
        const diffMinutes = endTime - startTime;
    
        if (diffMinutes < 120) {
          this.toastr.error("Thời lượng buổi học bù phải ít nhất 2 tiếng!");
          return;
        }
        payload.lichDayBu = {
          ngayHocBu: this.newSchedule.ngay,
          phongId: this.newSchedule.phong,
          gioBatDau: this.newSchedule.batDau,
          gioKetThuc: this.newSchedule.ketThuc
        };
      }
    
      this.lophocService.createLichDayBu(payload).subscribe({
        next: (res) => {
          if (!res.isError) {
            this.toastr.success(res.message || 'Tạo lịch dạy bù thành công!');
            this.closeAddScheduleModal();
            this.loadLopHocs();
          } else {
            this.toastr.error(res.message || 'Tạo lịch dạy bù thất bại!');
          }
        },
        error: (err) => {
          console.error('Lỗi khi tạo lịch dạy bù:', err);
          this.toastr.error('Không thể tạo lịch dạy bù!');
        }
      });
    }
  }

  getChuongTrinhs(): void {
    this.lophocService.getChuongTrinhs().subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          this.chuongTrinhOptions = res.data;
        } else {
          this.chuongTrinhOptions = [];
        }
      },
      error: (err) => {
        console.error("Lỗi khi lấy chương trình:", err);
        this.chuongTrinhOptions = [];
      },
    });
  }
  getGiaoVienOptions() {
    const payload = { searchTen: "" };
    this.lophocService.getGiaoViens(payload).subscribe({
      next: (res) => {
        if (!res.isError && res.data) {
          this.giaoVienOptions = res.data;
          this.filteredGiaoVienOptions = res.data.slice();
        } else {
          this.giaoVienOptions = [];
          this.filteredGiaoVienOptions = [];
        }
      },
      error: (err) => {
        console.error("Lỗi khi lấy giáo viên:", err);
      },
    });
  }
  getThuHienThi(thu: number): string {
    const thuMap: { [key: number]: string } = {
      2: "Thứ 2",
      3: "Thứ 3",
      4: "Thứ 4",
      5: "Thứ 5",
      6: "Thứ 6",
      7: "Thứ 7",
      8: "Chủ Nhật",
    };
    return thuMap[thu] || `Thứ ${thu}`;
  }

  getSelectedGiaoVienText(): string {
    const selected = this.giaoVienOptions.find(
      (gv) => gv.code === this.selectedGiaoVien
    );
    return selected ? selected.codeTen : "";
  }
  clearGiaoVienSelection(event: MouseEvent) {
    event.stopPropagation();
    this.selectedGiaoVien = "";
    this.giaoVienSearch = "";
    this.filteredGiaoVienOptions = this.giaoVienOptions.slice();
    this.currentPage = 1;
    this.loadLopHocs();
  }

  onGiaoVienSearch() {
    const searchTerm = this.giaoVienSearch.toLowerCase();
    this.filteredGiaoVienOptions = this.giaoVienOptions.filter((gv) =>
      gv.codeTen.toLowerCase().includes(searchTerm)
    );
  }

  selectGiaoVien(gv: { code: string; codeTen: string }) {
    this.selectedGiaoVien = gv.code;
    this.giaoVienSearch = "";
    this.isGiaoVienDropdownOpen = false;
    this.currentPage = 1;
    this.loadLopHocs();
  }

  toggleGiaoVienDropdown() {
    this.isGiaoVienDropdownOpen = !this.isGiaoVienDropdownOpen;
  }

  loadLopHocs(page: number = this.currentPage) {
    const thus: number[] = [];
    if (this.thuTrongTuan.thu2) thus.push(2);
    if (this.thuTrongTuan.thu3) thus.push(3);
    if (this.thuTrongTuan.thu4) thus.push(4);
    if (this.thuTrongTuan.thu5) thus.push(5);
    if (this.thuTrongTuan.thu6) thus.push(6);
    if (this.thuTrongTuan.thu7) thus.push(7);
    if (this.thuTrongTuan.cn)  thus.push(8);
  
    const payload = {
      pageNumber: page,
      pageSize: this.pageSize,
      tenLop: this.searchTerm,
      thus: thus,
      giaoVienCode: this.selectedGiaoVien || "all",
      phongId: 0,
      chuongTrinhId: this.chuongTrinhId || 0,
      trangThai: this.trangThai || "all",
      thoiGianBatDau: this.timeStart || "",
      thoiGianKetThuc: this.timeEnd || "",
      ngayBatDau: this.dateStart || "0001-01-01",
      ngayKetThuc: this.dateEnd || "0001-01-01",
    };
  
    this.spinner.show();
    this.lophocService.getDanhSachLopHoc(payload).subscribe({
      next: (response) => {
        this.spinner.hide();
        if (!response.isError && response.data) {
          const data = response.data;
  
          this.lophocs = data.lopHocs.map((item: any) => {
            const coDinh = item.loaiLichHocs.find((l: any) => l.trangThai === 'Cố định')?.lichHocs || [];
            const dayBu = item.loaiLichHocs.find((l: any) => l.trangThai === 'Dạy bù')?.lichHocs || [];
            const dayThay = item.loaiLichHocs.find((l: any) => l.trangThai === 'Dạy thay')?.lichHocs || [];
            const daNghi = item.loaiLichHocs.find((l: any) => l.trangThai === 'Đã nghỉ')?.lichHocs || [];
            const firstLich = coDinh[0];
  
            return {
              ...item,
              chuongTrinh: item.tenChuongTrinh,
              phong: "",
              giaoVien: item.tenGiaoVien,
              ngayBatDau: firstLich?.ngayBatDau,
              ngayKetThuc: firstLich?.ngayKetThuc,
              hocPhi: item.hocPhi,
              trangThai: item.loaiLichHocs.find((l: any) => l.lichHocs.length > 0)?.trangThai || "Không xác định",
  
              lichCoDinh: coDinh.map((lh: any) => ({ ...lh })), 
              lichDayBu: dayBu.map((lh: any) => ({ ...lh })),
              lichDayThay: dayThay.map((lh: any) => ({
                ...lh,
                giaoVienCode: lh.giaoVienCode || '',
                tenGiaoVien: lh.tenGiaoVien || ''
              })),
              lichNghi: daNghi.map((lh: any) => ({ ...lh })),
  ngayNghis: item.ngayNghis || []
            };
          });
  
          this.currentPage = data.pageNumber;
          this.totalPages = Math.ceil(data.totalCount / this.pageSize);
          this.cdr.detectChanges();
        } else {
          this.lophocs = [];
          this.totalPages = 1;
        }
      },
      error: (err) => {
        this.spinner.hide();
        console.error("Lỗi khi lấy danh sách lớp học:", err);
        this.lophocs = [];
        this.totalPages = 1;
      },
    });
  }
  

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadLopHocs(page);
  }

  onAddClass() {
    this.router.navigate(["/lophoc/add"]);
  }

  onEditClass(index: number): void {
    const tenLop = this.lophocs[index].tenLop;
    console.log('🚀 Tên lớp từ UI:', tenLop);
  
    this.lophocService.getLopHocByTenLop(tenLop).subscribe(
      (res) => {
        console.log('✅ Kết quả API:',tenLop);
        this.router.navigate(['/lophoc/edit', tenLop]);

      },
      (err) => {
        console.error('❌ Lỗi gọi API:', err);
      }
    );
  }
  
  
  onFilterChange() {
    this.currentPage = 1;
    this.loadLopHocs();
  }
  
  
  onDeleteLichDayThay(lich: any) {
    if (!lich?.id) {
      this.toastr.error("Không tìm thấy ID của lịch học để xóa!");
      return;
    }
  
    if (!confirm("Bạn có chắc chắn muốn xóa lịch dạy thay này?")) {
      return;
    }
  
    this.lophocService.deleteLichDayThay(lich.id).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Xóa lịch dạy thay thành công!');
          this.loadLopHocs(); 
        } else {
          this.toastr.error(res.message || 'Xóa lịch dạy thay thất bại!');
        }
      },
      error: (err) => {
        console.error("Lỗi khi xóa lịch dạy thay:", err);
        this.toastr.error("Đã xảy ra lỗi khi xóa lịch dạy thay.");
      }
    });
  }
  
  onEditLichDayThay(lich: any, lop: any) {
    console.log('GVCODE:', lich.giaoVienCode);
  
    this.newScheduleEdit = {
      ...lich,
      tenLop: lop.tenLop,
      giaoVienCode: lich.giaoVienCode || ''
    };
  
    this.editGiaoVienSearch = '';
    this.editFilteredGiaoVienOptions = this.giaoVienOptions.slice();
  
    this.isEditScheduleModalOpen = true;
  }
 
  toggleEditGiaoVienDropdown(event: Event) {
    event.stopPropagation();
    this.isEditGiaoVienDropdownOpen = !this.isEditGiaoVienDropdownOpen;
  }
  
  onEditGiaoVienSearch() {
    const search = this.editGiaoVienSearch.toLowerCase();
    this.editFilteredGiaoVienOptions = this.giaoVienOptions.filter(
      (gv: any) =>
        gv.code?.toLowerCase().includes(search) ||
        gv.ten?.toLowerCase().includes(search) ||
        gv.codeTen?.toLowerCase().includes(search)
    );
  }
  
  getSelectedEditGiaoVienText(): string {
    const gvCode = this.newScheduleEdit?.giaoVienCode;
    const selected = this.giaoVienOptions.find(gv => gv.code === gvCode);
    return selected ? selected.codeTen : '';
  }
  
  selectEditGiaoVien(gv: any) {
    this.editSelectedGiaoVien = gv;
    this.newScheduleEdit.giaoVienCode = gv.code;
    this.isEditGiaoVienDropdownOpen = false;
  }
  
  closeEditScheduleModal() {
    this.isEditScheduleModalOpen = false;
    this.editingLichDayThay = null;
  }
  submitEditLichDayThay() {
    const payload = {
      id: this.newScheduleEdit.id,
      tenLop: this.newScheduleEdit.tenLop,
      ngayDay: this.newScheduleEdit.ngayBatDau,
      giaoVienCode: this.newScheduleEdit.giaoVienCode
    };
  
    this.lophocService.updateLichDayThay(payload).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Cập nhật lịch dạy thay thành công!');
          this.closeEditScheduleModal();
          this.loadLopHocs();
        } else {
          this.toastr.error(res.message || 'Cập nhật thất bại!');
        }
      },
      error: (err) => {
        console.error('Lỗi khi cập nhật lịch dạy thay:', err);
        this.toastr.error('Không thể cập nhật lịch dạy thay!');
      }
    });
  }
  
  formatDateForInput(dateStr: string): string {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    const year = d.getFullYear();
    const month = ('0' + (d.getMonth() + 1)).slice(-2);
    const day = ('0' + d.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }
  onDeleteLichDayBu(lich: any) {
    if (!lich?.id) {
      this.toastr.error('Lịch học không hợp lệ!');
      return;
    }
  
    const confirmed = confirm('Bạn có chắc chắn muốn xóa lịch dạy bù này?');
    if (!confirmed) return;
  
    this.lophocService.deleteLichDayBu(lich.id).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Xóa lịch dạy bù thành công!');
          this.loadLopHocs(); 
        } else {
          this.toastr.error(res.message || 'Xóa thất bại!');
        }
      },
      error: (err) => {
        console.error("Lỗi khi xóa lịch dạy bù:", err);
        this.toastr.error("Không thể xóa lịch dạy bù!");
      }
    });
  }
  onEditLichDayBu(lich: any, lop: any) {
    console.log('📋 Lịch dạy bù được chọn để sửa:', lich);
    console.log('🏫 Lớp:', lop.tenLop);
  
    // Dùng phongOptions đã load sẵn ở ngOnInit
    const matchedPhong = this.phongOptions.find(p => p.ten === lich.tenPhong);
  
    this.editScheduleDayBu = {
      id: lich.id,
      tenLop: lop.tenLop,
      ngayNghi: lich.ngayGoc || '',
      ngay: lich.ngayBatDau || '',
      phong: matchedPhong?.id || '', // lấy đúng id phòng từ tên
      batDau: lich.gioBatDau?.substring(0,5),
      ketThuc: lich.gioKetThuc?.substring(0,5)
    };
  
    
    this.isEditDayBuModalOpen = true;
  }
  submitEditLichDayBu() {
    if (!this.editScheduleDayBu?.id) {
      this.toastr.error("Không tìm thấy ID lịch học!");
      return;
    }
  
   
    const start = this.editScheduleDayBu.batDau;
    const end = this.editScheduleDayBu.ketThuc;
  
    const [startHour, startMinute] = start.split(":").map(Number);
    const [endHour, endMinute] = end.split(":").map(Number);
  
    const startTime = startHour * 60 + startMinute;
    const endTime = endHour * 60 + endMinute;
  
    const diffMinutes = endTime - startTime;
  
    if (diffMinutes < 120) {
      this.toastr.error("Thời lượng buổi học phải ít nhất 2 tiếng!");
      return;
    }
  
    const payload = {
      id: this.editScheduleDayBu.id,
      tenLop: this.editScheduleDayBu.tenLop,
      ngayNghi: this.editScheduleDayBu.ngayNghi,
      lichDayBu: {
        ngayHocBu: this.editScheduleDayBu.ngay,
        phongId: this.editScheduleDayBu.phong,
        gioBatDau: this.editScheduleDayBu.batDau,
        gioKetThuc: this.editScheduleDayBu.ketThuc
      }
    };
  
    this.lophocService.updateLichDayBu(payload).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Cập nhật lịch dạy bù thành công!');
          this.isEditDayBuModalOpen = false;
          this.loadLopHocs();
        } else {
          this.toastr.error(res.message || 'Cập nhật lịch dạy bù thất bại!');
        }
      },
      error: (err) => {
        console.error(' Lỗi khi cập nhật lịch dạy bù:', err);
        this.toastr.error('Không thể cập nhật lịch dạy bù!');
      }
    });
  }
  onDeleteLopHoc(tenLop: string): void {
    if (!confirm(`Bạn có chắc chắn muốn xóa/đóng lớp "${tenLop}" không?`)) return;
  
    this.lophocService.deleteLopHocCoDinh(tenLop).subscribe({
      next: (res) => {
        if (!res.isError) {
          this.toastr.success(res.message || 'Xóa/đóng lớp học thành công.');
          this.loadLopHocs(); // gọi lại danh sách lớp học
        } else {
          this.toastr.error(res.message || 'Không thể xóa lớp học.');
        }
      },
      error: (err) => {
        console.error('Lỗi khi xóa lớp học:', err);
        this.toastr.error(err?.error?.message || 'Đã xảy ra lỗi khi xóa lớp học.');
      }
    });
  }
  diDenBaoCaoDiemDanh(tenLop: string) {
    this.router.navigate(['/baocaodiemdanhquanlycoso'], { queryParams: { TenLop: tenLop } });
  }
  diDenBaoCaoHocphi(tenLop: string) {
    this.router.navigate(['/baocaohocphi'], { queryParams: { TenLop: tenLop } });
  }
  
  
  
  
  
}
